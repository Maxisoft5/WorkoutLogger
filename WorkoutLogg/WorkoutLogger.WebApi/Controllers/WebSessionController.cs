using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Modules.Common.Infrastructure.RateLimiting;
using Modules.Common.Infrastructure.Tenancy;
using Modules.Users.Domain.Authentication;
using Modules.Users.DTO.Auth;
using Modules.Users.Infrastructure.Database;
using WorkoutLogger.WebApi.Extensions;

namespace WorkoutLogger.WebApi.Controllers;

public class WebLoginRequest : IValidatableObject
{
    [EmailAddress, StringLength(256)] public string? Email { get; set; }
    [StringLength(40)] public string? PhoneNumber { get; set; }
    [Required, StringLength(128, MinimumLength = 8)] public string Password { get; set; } = "";
    public string Identifier => LoginIdentifier.Normalize(!string.IsNullOrWhiteSpace(Email) ? Email : PhoneNumber) ?? "";
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(Email) == string.IsNullOrWhiteSpace(PhoneNumber)
            || (!string.IsNullOrWhiteSpace(PhoneNumber) && LoginIdentifier.NormalizePhone(PhoneNumber) is null))
            yield return new("Укажите email или телефон с кодом страны, например +79991234567.", [nameof(Email), nameof(PhoneNumber)]);
    }
}
public sealed class WebRegisterRequest : WebLoginRequest
{
    [Required, StringLength(80)] public string FullName { get; set; } = "";
}

[ApiController, Route("api/session"), EnableRateLimiting("auth")]
public sealed class WebSessionController(IAuthService auth, IDataProtectionProvider protection, TenantContext tenant,
    ILoginRateLimiter limiter, UsersDbContext db, IWebHostEnvironment environment) : ControllerBase
{
    private const string CookieName = "workout_session";
    private IDataProtector Protector => protection.CreateProtector("WorkoutLogg.WebSession.v1", tenant.Current.Id);
    private sealed record Session(string Token, string RefreshToken);

    // A same-origin POST is required even for login/logout to prevent login CSRF.
    private bool SameOrigin() => Uri.TryCreate(Request.Headers.Origin.ToString(), UriKind.Absolute, out var origin)
        && origin.Authority.Equals(Request.Host.Value, StringComparison.OrdinalIgnoreCase)
        && (origin.Scheme == Request.Scheme || environment.IsDevelopment() && origin.Scheme == "http");

    [HttpPost("login")]
    public async Task<IActionResult> Login(WebLoginRequest request, CancellationToken ct)
    {
        if (!SameOrigin()) return Forbid();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var limit = await limiter.CheckAsync(request.Identifier, ip, ct);
        if (limit.IsBlocked) return StatusCode(429, new { message = "Слишком много попыток входа. Попробуйте позже." });
        var result = await auth.LoginAsync(request.Identifier, request.Password, ct);
        if (!result.IsSuccess) { await limiter.RecordFailureAsync(request.Identifier, ip, ct); return result.ToActionResult(); }
        await limiter.ResetAsync(request.Identifier, ip, ct);
        return Store(result.Value!.Token!, result.Value.RefreshToken!);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(WebRegisterRequest request, CancellationToken ct)
    {
        if (!SameOrigin()) return Forbid();
        var result = await auth.RegisterAsync(new UserDto { Email = request.Email, PhoneNumber = request.PhoneNumber, Password = request.Password, FullName = request.FullName }, ct);
        return result.IsSuccess ? Store(result.Value!.Token, result.Value.RefreshToken) : result.ToActionResult();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        if (!SameOrigin()) return Forbid();
        var session = Read();
        if (session is null) return Unauthorized();
        var result = await auth.RefreshTokenAsync(session.Token, session.RefreshToken, ct);
        if (!result.IsSuccess) { Response.Cookies.Delete(CookieName, CookieOptions()); return Unauthorized(); }
        return Store(result.Value!.Token, result.Value.RefreshToken);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        if (!SameOrigin()) return Forbid();
        var session = Read();
        if (session is not null)
        {
            var refresh = await db.RefreshTokens.SingleOrDefaultAsync(x => x.Token == session.RefreshToken, ct);
            if (refresh is not null) { refresh.Invalidated = true; await db.SaveChangesAsync(ct); }
        }
        Response.Cookies.Delete(CookieName, CookieOptions());
        return NoContent();
    }

    private IActionResult Store(string token, string refresh)
    {
        Response.Headers.CacheControl = "no-store";
        Response.Cookies.Append(CookieName, Protector.Protect(JsonSerializer.Serialize(new Session(token, refresh))), CookieOptions());
        return Ok(new { token });
    }
    private CookieOptions CookieOptions() => new()
    {
        HttpOnly = true, Secure = !environment.IsDevelopment() || Request.IsHttps,
        SameSite = SameSiteMode.Strict, Path = "/api/session", MaxAge = TimeSpan.FromDays(7), IsEssential = true
    };
    private Session? Read()
    {
        if (!Request.Cookies.TryGetValue(CookieName, out var value)) return null;
        try { return JsonSerializer.Deserialize<Session>(Protector.Unprotect(value)); }
        catch (Exception ex) when (ex is CryptographicException or JsonException) { return null; }
    }
}

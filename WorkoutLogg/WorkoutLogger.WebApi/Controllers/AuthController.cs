using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Modules.Common.Domain.Events;
using Modules.Common.Infrastructure.Messaging;
using Modules.Users.Domain.Authentication;
using Modules.Users.Domain.Mappers;
using Modules.Users.DTO.Auth;
using WorkoutLogger.WebApi.Extensions;
using WorkoutLogger.WebApi.Services;

namespace WorkoutLogger.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[EnableRateLimiting("auth")]
public class AuthController(IAuthService authService,
    IUserService userService,
    IHttpContextAccessor httpContextAccessor,
    IEventPublisher eventPublisher,
    KafkaSettings kafkaSettings) : ControllerBase
{

    [Authorize]
    [HttpGet("CurrentUser")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var login = await authService.GetCurrent();
        if (login.IsSuccess && login.Value != null)
        {
            return Ok(UserMapper.MapUser(login.Value));
        }
        return Forbid();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserDto user)
    {
        var login = await authService.LoginAsync(user.Email, user.Password);

        var ctx = httpContextAccessor.HttpContext;
        if (login.IsSuccess)
        {
            var logined = await userService.GetUserByEmail(user.Email);
            if (logined.IsSuccess)
            {
                await eventPublisher.PublishAsync(kafkaSettings.Topics.AuthEvents, new AuthEvent
                {
                    EventType = "user.login",
                    Email = user.Email ?? "unknown",
                    UserId = logined.Value?.Id ?? "",
                    IpAddress = ctx?.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = ctx?.Request.Headers.UserAgent.ToString()
                });
            }
        }
        else
        {
            await eventPublisher.PublishAsync(kafkaSettings.Topics.AuthEvents, new AuthEvent
            {
                EventType = "user.login_failed",
                Email = user.Email ?? "unknown",
                IpAddress = ctx?.Connection.RemoteIpAddress?.ToString(),
                UserAgent = ctx?.Request.Headers.UserAgent.ToString()
            });
        }
        // The response contains only the token pair; the Result wrapper
        // (and its empty-errors pitfall) never crosses the API boundary.
        return login.ToActionResult(v => new RegisterUserResponse(v.Token!, v.RefreshToken!));
    }

    // No [Authorize] here on purpose: the access token is expired by the time
    // a client refreshes it. RefreshTokenAsync validates the token pair itself
    // (signature, JTI match, stored refresh token, expiry, invalidation).
    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest token)
    {
        var refreshed = await authService.RefreshTokenAsync(token.Token, token.RefreshToken);
        return refreshed.ToActionResult();
    }

    [HttpPost("CreateAccount")]
    public async Task<IActionResult> CreateAccount([FromBody] UserDto user)
    {
        var created = await authService.RegisterAsync(user, default);
        return created.ToActionResult();
    }

    [Authorize]
    [HttpPut("UpdateAccount")]
    public async Task<IActionResult> UpdateAccount([FromBody] UserDto user)
    {
        var upd = await authService.UpdateUser(user);
        return upd.ToActionResult(UserMapper.MapUser);
    }

    /// <summary>
    /// Выбор активной роли аккаунта (ученик/тренер) — шаг регистрации 01 модуля «Тренеры».
    /// Роль можно сменить позже в профиле этим же эндпоинтом.
    /// </summary>
    [Authorize]
    [HttpPost("SelectRole")]
    public async Task<IActionResult> SelectRole([FromBody] SelectRoleRequest request, [FromServices] ICurrentUser currentUser)
    {
        if (currentUser.UserId is null)
            return Unauthorized();

        var result = await userService.SetActiveRoleAsync(currentUser.UserId, request.Role);
        return result.ToActionResult();
    }

    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await authService.SendPasswordResetCodeAsync(request.Email);
        return result.ToActionResult();
    }

    [HttpPost("VerifyResetCode")]
    public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeRequest request)
    {
        var result = await authService.VerifyResetCodeAsync(request.Email, request.Code);
        return result.ToActionResult();
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await authService.ResetPasswordAsync(request.Email, request.Code, request.NewPassword);
        return result.ToActionResult();
    }
}

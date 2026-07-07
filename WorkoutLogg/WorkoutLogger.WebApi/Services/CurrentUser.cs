using System.Security.Claims;

namespace WorkoutLogger.WebApi.Services;

/// <summary>
/// Single source of truth for identifying the authenticated caller.
/// The JWT is issued with a custom "userid" claim (the user's Id) and "sub"
/// set to the email. Controllers previously read these inconsistently
/// (some "userid", some ClaimTypes.NameIdentifier which maps to "sub"/email),
/// which caused subscriptions and premium to be keyed by email instead of Id.
/// </summary>
public interface ICurrentUser
{
    /// <summary>The user's Id (from the "userid" claim), or null if unauthenticated.</summary>
    string? UserId { get; }

    /// <summary>The user's email (from the "sub"/email claim), or null if unauthenticated.</summary>
    string? Email { get; }
}

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public string? UserId => Principal?.FindFirst("userid")?.Value;

    public string? Email =>
        Principal?.FindFirst(ClaimTypes.Email)?.Value
        ?? Principal?.FindFirst("sub")?.Value
        ?? Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}

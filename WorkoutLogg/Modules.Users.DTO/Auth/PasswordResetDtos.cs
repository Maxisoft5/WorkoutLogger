using System.ComponentModel.DataAnnotations;

namespace Modules.Users.DTO.Auth;

public record ForgotPasswordRequest(
    [property: Required, EmailAddress] string Email);

public record VerifyResetCodeRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required, StringLength(6, MinimumLength = 6)] string Code);

public record ResetPasswordRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required, StringLength(6, MinimumLength = 6)] string Code,
    [property: Required, MinLength(8), MaxLength(128)] string NewPassword);

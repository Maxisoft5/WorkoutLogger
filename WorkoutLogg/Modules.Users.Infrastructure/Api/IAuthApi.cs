using Modules.Users.Domain.Authentication;
using Modules.Users.DTO.Auth;
using Refit;

namespace Modules.Users.Infrastructure.Api
{
    public interface IAuthApi
    {
        [Get("/Auth/CurrentUser")]
        public Task<IApiResponse<UserDto>> GetCurrentUser([Header("Authorization")] string token);

        [Post("/Auth/CreateAccount")]
        public Task<IApiResponse<RegisterUserResponse>> CreateAccount([Body] UserDto user);

        [Post("/Auth/Login")]
        public Task<IApiResponse<RegisterUserResponse>> Login([Body] UserDto user);

        [Put("/Auth/UpdateAccount")]
        public Task<IApiResponse<UserDto>> UpdateAccount([Header("Authorization")] string token, [Body] UserDto user);

        [Post("/Auth/SelectRole")]
        public Task<IApiResponse<UserDto>> SelectRole([Header("Authorization")] string token, [Body] SelectRoleRequest request);

        [Post("/Auth/ForgotPassword")]
        public Task<IApiResponse> ForgotPassword([Body] ForgotPasswordRequest request);

        [Post("/Auth/VerifyResetCode")]
        public Task<IApiResponse> VerifyResetCode([Body] VerifyResetCodeRequest request);

        [Post("/Auth/ResetPassword")]
        public Task<IApiResponse> ResetPassword([Body] ResetPasswordRequest request);
    }
}

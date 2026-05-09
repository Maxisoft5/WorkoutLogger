using Modules.Common.Domain.Results;
using Modules.Users.Domain.Users;
using Modules.Users.DTO.Auth;

namespace Modules.Users.Domain.Authentication
{
    public interface IAuthService
    {
        Task<Result<User>> UpdateUser(UserDto user);
        Task<Result<User>> GetCurrent();
        Task<Result<UpdateRoleResponse>> UpdateUserRoleAsync(string userId, string newRole, CancellationToken cancellationToken);
        Task<Result<LoginUserResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<Result<RegisterUserResponse>> RegisterAsync(UserDto user, CancellationToken cancellationToken);
        Task<Result<RefreshTokenResponse>> RefreshTokenAsync(string token, string refreshToken, 
            CancellationToken cancellationToken=default);
    }
}

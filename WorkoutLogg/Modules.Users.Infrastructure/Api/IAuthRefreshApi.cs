using Modules.Users.Domain.Authentication;
using Refit;

namespace Modules.Users.Infrastructure.Api
{
    public interface IAuthRefreshApi
    {
        [Post("/Auth/Refresh")]
        public Task<IApiResponse<RefreshTokenResponse>> Refresh([Header("Authorization")] string token,
          [Body] RefreshTokenRequest request);
    }
}

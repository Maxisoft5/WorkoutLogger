using Modules.Common.Domain.Results;
using Modules.Users.Domain.Authentication;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Users.Infrastructure.Api
{
    public interface IAuthRefreshApi
    {
        [Post("/Auth/Refresh")]
        public Task<IApiResponse<Result<RefreshTokenResponse>>> Refresh([Header("Authorization")] string token,
          [Body] RefreshTokenRequest request);
    }
}

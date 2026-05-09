using Microsoft.AspNetCore.Mvc;
using Modules.Common.Domain.Results;
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
        public Task<IApiResponse<Result<RegisterUserResponse>>> CreateAccount([Body] UserDto user);

        [Post("/Auth/Login")]
        public Task<IApiResponse<Result<RegisterUserResponse>>> Login([Body] UserDto user);
  
        [Put("/Auth/UpdateAccount")]
        public Task<IApiResponse<UserDto>> UpdateAccount([Header("Authorization")] string token, [Body] UserDto user);

    }
}

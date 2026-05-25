using Modules.Common.Domain.Results;
using Modules.Users.Domain.Users;
using Modules.Users.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Users.Domain.Authentication
{
    public interface IUserService
    {
        public Task<Result<UserDto>> GetUserByEmail(string email);
    }
}

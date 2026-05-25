using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Results;
using Modules.Users.Domain.Authentication;
using Modules.Users.Domain.Mappers;
using Modules.Users.DTO.Auth;
using Modules.Users.Infrastructure.Database;

namespace Modules.Users.Infrastructure.Authorization
{
    public class UserService(UsersDbContext dbContext) : IUserService
    {
        public async Task<Result<UserDto>> GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new Result<UserDto>(new Error("400", "email not valid", ErrorType.Validation));
            }
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return new Result<UserDto>(new Error("404", "user not found", ErrorType.NotFound));
            }
            var dto = user.MapUser();
            return new Result<UserDto>(dto);
        }
    }
}

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
                return new Result<UserDto>(new Error("400", "email not valid", ErrorType.Validation));

            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
                return new Result<UserDto>(new Error("404", "user not found", ErrorType.NotFound));

            return new Result<UserDto>(user.MapUser());
        }

        public async Task<Result<UserDto>> SetActiveRoleAsync(string userId, DTO.Users.AccountRole role)
        {
            if (!Enum.IsDefined(role))
                return new Result<UserDto>(Error.Validation("Users.InvalidRole", $"Unknown account role '{role}'"));

            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new Result<UserDto>(new Error("404", "user not found", ErrorType.NotFound));

            user.ActiveRole = role;
            await dbContext.SaveChangesAsync();
            return new Result<UserDto>(user.MapUser());
        }

        public async Task SetPremiumAsync(string userId, bool isPremium)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null) return;
            user.IsPremium = isPremium;
            await dbContext.SaveChangesAsync();
        }
    }
}

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
        public Task SetPremiumAsync(string userId, bool isPremium);
        public Task<Result<UserDto>> SetActiveRoleAsync(string userId, DTO.Users.AccountRole role);

        /// <summary>Даты начала тренировок пользователя с указанного момента (для бонуса за серию).</summary>
        public Task<List<DateTime>> GetWorkoutDatesAsync(string userId, DateTime sinceUtc);
    }
}

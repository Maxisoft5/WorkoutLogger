using Modules.Users.Domain.Users;
using Modules.Users.DTO.Auth;

namespace Modules.Users.Domain.Mappers
{
    public static class UserMapper
    {
        public static UserDto MapUser(this User user)
        {
            return new UserDto()
            {
                Email = user.Email,
                FullName = user.UserName,
                BodyStats = new UserBodyStatsDto()
                {
                    Kg = user.BodyStats?.Kg ?? 0,
                    Cm = user.BodyStats?.Cm ?? 0,
                    Fat = user.BodyStats?.Fat ?? 0
                },
                DateOfBirth = user.DateOfBirth,
                Goals = user.UserGoals == null ? new List<DTO.Users.UserGoalDto>() 
                : user.UserGoals.Select(g => new DTO.Users.UserGoalDto()
                {
                    Id = g.Id,
                    Goal = g.Goal
                }).ToList(),
                WorkOutCount = user.WorkOutCount,
                IsPremium = user.IsPremium,
                UserRegistrationStep = user.UserRegistrationStep,
                Identity = user.Identity
            };
        }
    }
}

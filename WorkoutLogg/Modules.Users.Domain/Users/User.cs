using Microsoft.AspNetCore.Identity;
using Modules.Common.Domain;
using Modules.Users.Domain.Workout;
using Modules.Users.DTO.Auth;
using Modules.Users.DTO.Users;

namespace Modules.Users.Domain.Users
{
    public class User : IdentityUser, IAuditableEntity
    {
        public DateTime DateOfBirth { get; set; }
        public UserSex Identity { get; set; }
        public BodyStats BodyStats { get; set; } = new();
        public ICollection<UserClaim> Claims { get; set; } = null!;

        public ICollection<UserRole> UserRoles { get; set; } = null!;

        public ICollection<UserLogin> UserLogins { get; set; } = null!;

        public ICollection<UserToken> UserTokens { get; set; } = null!;
        public ICollection<UserGoal> UserGoals { get; set; } = null!;
        public ICollection<WorkoutModel> Workouts { get; set; } = null!;
        public WorkOutCountVariant WorkOutCount { get; set; }
        public UserRegistrationStep UserRegistrationStep { get; set; }
        public bool IsPremium { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}

using Modules.Common.Domain;
using Modules.Users.DTO.Users;

namespace Modules.Users.Domain.Users
{
    public class UserGoal : IHasGuidId, IAuditableEntity
    {
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public UserGoalVariant Goal { get; set; }
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public Guid Id { get; set; }
    }
}

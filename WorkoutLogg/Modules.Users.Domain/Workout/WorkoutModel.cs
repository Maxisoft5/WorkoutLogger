using Moduels.Workouts.DTO.Enums;
using Modules.Common.Domain;
using Modules.Users.Domain.Exercies;
using Modules.Users.Domain.Logs;
using Modules.Users.Domain.Users;


namespace Modules.Users.Domain.Workout
{
    public class WorkoutModel : IHasGuidId, IAuditableEntity
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public WorkoutType WorkoutType { get; set; }
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public ICollection<Exercise> Exercises { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}

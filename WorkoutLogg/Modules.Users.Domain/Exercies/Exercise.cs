using Moduels.Workouts.DTO.Enums;
using Modules.Common.Domain;
using Modules.Users.Domain.Logs;
using Modules.Users.Domain.Workout;

namespace Modules.Users.Domain.Exercies
{
    public class Exercise : IHasGuidId, IAuditableEntity
    {
        public Guid Id { get; set; }
        public ExerciesComplexity ExerciesComplexity { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid WorkoutId { get; set; }
        public WorkoutModel Workout { get; set; } = null!;
        public ICollection<ExerciseSet> Sets { get; set; } = null!;
        public ICollection<WorkoutLog> Logs { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}

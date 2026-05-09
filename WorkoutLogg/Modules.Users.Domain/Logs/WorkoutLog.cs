using Modules.Common.Domain;
using Modules.Users.Domain.Exercies;
using Modules.Users.Domain.Workout;

namespace Modules.Users.Domain.Logs
{
    public class WorkoutLog : IHasGuidId, IAuditableEntity
    {
        public Guid Id { get; set; }
        public DateTime DateLog { get; set; }
        public Guid ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;
        public int SetNumber { get; set; }
        public int Kg { get; set; }
        public int RepNumber { get; set; }
        public int RestSeconds { get; set; }
        public bool IsHistoryRecord { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}

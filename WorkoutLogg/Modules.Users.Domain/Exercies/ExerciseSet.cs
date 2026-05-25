using Modules.Common.Domain;

namespace Modules.Users.Domain.Exercies
{
    public class ExerciseSet : IHasGuidId, IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public double WeightKg { get; set; }
        public int RestSeconds { get; set; }
        public bool IsWarmup { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}

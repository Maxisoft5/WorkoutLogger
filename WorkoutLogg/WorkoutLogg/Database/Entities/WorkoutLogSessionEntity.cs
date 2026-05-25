using SQLite;

namespace WorkoutLogg.Database.Entities
{
    [Table("log_sessions")]
    public class WorkoutLogSessionEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [Indexed]
        public DateTime Date { get; set; }

        public Guid WorkoutId { get; set; }      // Guid.Empty = not linked to a plan

        public string WorkoutLabel { get; set; } = "";

        public bool IsCustom { get; set; }        // true = no workout plan linked

        public string? Notes { get; set; }

        public bool IsSynced { get; set; }

        public Guid RemoteId { get; set; }

        [Ignore]
        public List<LogExerciseEntity> Exercises { get; set; } = [];
    }
}

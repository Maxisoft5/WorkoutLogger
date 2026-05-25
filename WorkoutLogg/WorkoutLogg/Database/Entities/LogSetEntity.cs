using SQLite;

namespace WorkoutLogg.Database.Entities
{
    [Table("log_sets")]
    public class LogSetEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [Indexed]
        public Guid ExerciseLogId { get; set; }

        public int SetNumber { get; set; }

        public int Reps { get; set; }

        public double WeightKg { get; set; }

        public int RestSeconds { get; set; }

        public bool IsWarmup { get; set; }
    }
}

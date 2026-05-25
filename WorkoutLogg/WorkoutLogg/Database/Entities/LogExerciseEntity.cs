using Moduels.Workouts.DTO.Enums;
using SQLite;

namespace WorkoutLogg.Database.Entities
{
    [Table("log_exercises")]
    public class LogExerciseEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [Indexed]
        public Guid SessionId { get; set; }

        public Guid WorkoutSetId { get; set; }   // Guid.Empty = custom exercise

        public string ExerciseName { get; set; } = "";

        public bool IsCustom { get; set; }        // true = not from a workout plan

        public ExerciesComplexity Complexity { get; set; }

        [Ignore]
        public List<LogSetEntity> Sets { get; set; } = [];
    }
}

using Moduels.Workouts.DTO.Enums;
using SQLite;

namespace WorkoutLogg.Database.Entities
{
    public class WorkoutSetEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [Indexed]
        public Guid WorkoutId { get; set; }

        public ExerciesComplexity ExerciesComplexity { get; set; }

        [MaxLength(100)]
        public string ExerciseName { get; set; } = "";

        [MaxLength(300)]
        public string Description { get; set; } = "";

        [Ignore]
        public List<WorkoutSetDetailEntity> Sets { get; set; } = [];
    }
}

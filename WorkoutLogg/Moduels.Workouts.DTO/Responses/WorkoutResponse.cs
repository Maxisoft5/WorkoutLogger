using Moduels.Workouts.DTO.Enums;

namespace Moduels.Workouts.DTO.Responses
{
    public class WorkoutResponse
    {
        public Guid Id { get; set; }
        public Guid LocalId { get; set; }
        public WorkoutType WorkoutType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ExerciseCount { get; set; }
    }
}

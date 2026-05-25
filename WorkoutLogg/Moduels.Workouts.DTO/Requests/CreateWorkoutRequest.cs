using Moduels.Workouts.DTO.Enums;

namespace Moduels.Workouts.DTO.Requests
{
    public class CreateWorkoutRequest
    {
        public Guid LocalId { get; set; }
        public WorkoutType WorkoutType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<CreateExerciseRequest> Exercises { get; set; } = [];
    }
}

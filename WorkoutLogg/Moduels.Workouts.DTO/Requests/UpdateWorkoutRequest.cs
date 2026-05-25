using Moduels.Workouts.DTO.Enums;

namespace Moduels.Workouts.DTO.Requests
{
    public class UpdateWorkoutRequest
    {
        public WorkoutType WorkoutType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<CreateExerciseRequest> Exercises { get; set; } = [];
    }
}

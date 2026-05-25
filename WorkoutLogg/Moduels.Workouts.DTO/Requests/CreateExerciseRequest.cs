using Moduels.Workouts.DTO.Enums;

namespace Moduels.Workouts.DTO.Requests
{
    public class CreateExerciseRequest
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public ExerciesComplexity Complexity { get; set; }
        public List<CreateSetRequest> Sets { get; set; } = [];
    }
}

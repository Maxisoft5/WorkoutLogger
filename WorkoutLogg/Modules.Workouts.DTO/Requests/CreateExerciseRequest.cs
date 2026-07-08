using Modules.Workouts.DTO.Enums;
using System.ComponentModel.DataAnnotations;

namespace Modules.Workouts.DTO.Requests
{
    public class CreateExerciseRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = "";

        [MaxLength(2000)]
        public string? Description { get; set; }

        public ExerciseComplexity Complexity { get; set; }

        public List<CreateSetRequest> Sets { get; set; } = [];
    }
}

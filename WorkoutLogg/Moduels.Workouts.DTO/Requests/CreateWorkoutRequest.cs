using Moduels.Workouts.DTO.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moduels.Workouts.DTO.Requests
{
    public class CreateWorkoutRequest : IValidatableObject
    {
        public Guid LocalId { get; set; }
        public WorkoutType WorkoutType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<CreateExerciseRequest> Exercises { get; set; } = [];

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate < StartDate)
                yield return new ValidationResult(
                    "EndDate must be greater than or equal to StartDate.",
                    [nameof(EndDate)]);
        }
    }
}

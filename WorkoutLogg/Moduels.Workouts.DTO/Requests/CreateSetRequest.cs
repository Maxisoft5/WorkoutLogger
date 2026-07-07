using System.ComponentModel.DataAnnotations;

namespace Moduels.Workouts.DTO.Requests
{
    public class CreateSetRequest
    {
        [Range(0, 1000)]
        public int SetNumber { get; set; }

        [Range(0, 10000)]
        public int Reps { get; set; }

        [Range(0, 2000)]
        public double WeightKg { get; set; }

        [Range(0, 86400)]
        public int RestSeconds { get; set; }

        public bool IsWarmup { get; set; }
    }
}

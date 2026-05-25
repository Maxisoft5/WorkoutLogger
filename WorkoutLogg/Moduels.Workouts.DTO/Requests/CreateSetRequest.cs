namespace Moduels.Workouts.DTO.Requests
{
    public class CreateSetRequest
    {
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public double WeightKg { get; set; }
        public int RestSeconds { get; set; }
        public bool IsWarmup { get; set; }
    }
}

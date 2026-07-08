using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Modules.Trainers.Infrastructure.Database
{
    public class TrainersDbContextFactory : IDesignTimeDbContextFactory<TrainersDbContext>
    {
        public TrainersDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("TRAINERS_DB")
                ?? "Host=localhost;Port=5432;Database=workoutLogger;Username=postgres;Password=postgres";

            var options = new DbContextOptionsBuilder<TrainersDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new TrainersDbContext(options);
        }
    }
}

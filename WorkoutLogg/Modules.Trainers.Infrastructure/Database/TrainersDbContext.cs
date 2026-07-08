using Microsoft.EntityFrameworkCore;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Database
{
    public class TrainersDbContext : DbContext
    {
        public DbSet<TrainerProfile> TrainerProfiles { get; set; } = null!;

        public TrainersDbContext(DbContextOptions<TrainersDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("trainers");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrainersDbContext).Assembly);
        }
    }
}

using Microsoft.EntityFrameworkCore.Infrastructure;
using Modules.Common.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Outbox;
using Modules.Users.Domain.Tokens;
using Modules.Users.Domain.Users;

namespace Modules.Users.Infrastructure.Database
{
    public class UsersDbContext : IdentityDbContext<User, Role, string,
        UserClaim, UserRole, UserLogin,
        RoleClaim, UserToken>, ITenantDbContext
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
        public DbSet<UserGoal> UserGoals { get; set; } = null!;
        public DbSet<Modules.Users.Domain.Workout.WorkoutModel> Workouts { get; set; } = null!;
        public DbSet<Modules.Users.Domain.Exercises.Exercise> Exercises { get; set; } = null!;
        public DbSet<Modules.Users.Domain.Exercises.ExerciseSet> ExerciseSets { get; set; } = null!;
        public DbSet<Modules.Users.Domain.Logs.WorkoutLog> WorkoutLogs { get; set; } = null!;

        public UsersDbContext(DbContextOptions<UsersDbContext> options, TenantContext? tenant = null) : base(options)
        {
            Schema = tenant?.Current.Schema ?? "users";
        }

        public string Schema { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
        }
    }
}

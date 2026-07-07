using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Modules.Users.Infrastructure.Database
{
    public class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
    {
        public UsersDbContext CreateDbContext(string[] args)
        {
            // Design-time only (EF migrations). Real connection strings come from
            // configuration/environment at runtime; the local fallback is for a
            // developer's own Postgres and must not contain shared credentials.
            var connectionString = Environment.GetEnvironmentVariable("USERS_DB")
                ?? "Host=localhost;Port=5432;Database=workoutLogger;Username=postgres;Password=postgres";

            var options = new DbContextOptionsBuilder<UsersDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new UsersDbContext(options);
        }
    }
}

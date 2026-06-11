using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Modules.Users.Infrastructure.Database
{
    public class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
    {
        public UsersDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("USERS_DB")
                ?? "Host=202.148.55.20;Port=5432;Database=workoutLogger;Username=postgres;Password=051099";

            var options = new DbContextOptionsBuilder<UsersDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new UsersDbContext(options);
        }
    }
}

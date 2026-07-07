using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Modules.Subscriptions.Infrastructure.Database
{
    public class SubscriptionsDbContextFactory : IDesignTimeDbContextFactory<SubscriptionsDbContext>
    {
        public SubscriptionsDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("SUBSCRIPTIONS_DB")
                ?? "Host=localhost;Port=5432;Database=workoutLogger;Username=postgres;Password=postgres";

            var options = new DbContextOptionsBuilder<SubscriptionsDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new SubscriptionsDbContext(options);
        }
    }
}

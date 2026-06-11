using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Modules.Subscriptions.Infrastructure.Database
{
    public class SubscriptionsDbContextFactory : IDesignTimeDbContextFactory<SubscriptionsDbContext>
    {
        public SubscriptionsDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("SUBSCRIPTIONS_DB")
                ?? "Host=202.148.55.20;Port=5432;Database=workoutLogger;Username=postgres;Password=051099";

            var options = new DbContextOptionsBuilder<SubscriptionsDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new SubscriptionsDbContext(options);
        }
    }
}

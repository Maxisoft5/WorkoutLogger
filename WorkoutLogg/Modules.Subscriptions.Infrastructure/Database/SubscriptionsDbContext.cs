using Microsoft.EntityFrameworkCore;
using Modules.Subscriptions.Infrastructure.Domain;

namespace Modules.Subscriptions.Infrastructure.Database
{
    public class SubscriptionsDbContext : DbContext
    {
        public DbSet<Subscription> Subscriptions { get; set; } = null!;

        public SubscriptionsDbContext(DbContextOptions<SubscriptionsDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("subscriptions");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SubscriptionsDbContext).Assembly);
        }
    }
}

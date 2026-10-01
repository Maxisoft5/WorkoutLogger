using Microsoft.EntityFrameworkCore.Infrastructure;
using Modules.Common.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Modules.Subscriptions.Infrastructure.Domain;

namespace Modules.Subscriptions.Infrastructure.Database
{
    public class SubscriptionsDbContext : DbContext, ITenantDbContext
    {
        public DbSet<Subscription> Subscriptions { get; set; } = null!;

        public SubscriptionsDbContext(DbContextOptions<SubscriptionsDbContext> options, TenantContext? tenant = null) : base(options) { Schema = tenant?.Current.Schema ?? "subscriptions"; }

        public string Schema { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema(Schema);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SubscriptionsDbContext).Assembly);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Subscriptions.Infrastructure.Domain;

namespace Modules.Subscriptions.Infrastructure.Database.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.UserId).IsRequired().HasMaxLength(450);
            builder.Property(s => s.ExternalPaymentId).HasMaxLength(256);
            builder.Property(s => s.ExternalSubscriptionId).HasMaxLength(256);
            builder.Property(s => s.Plan).HasConversion<string>();
            builder.Property(s => s.Status).HasConversion<string>();
            builder.Property(s => s.Provider).HasConversion<string>();
            builder.HasIndex(s => s.UserId);
            builder.HasIndex(s => new { s.UserId, s.Status });
        }
    }
}

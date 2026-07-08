using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Database.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(w => w.Id);
            builder.Property(w => w.UserId).IsRequired().HasMaxLength(450);
            builder.HasIndex(w => w.UserId).IsUnique();
        }
    }

    public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.UserId).IsRequired().HasMaxLength(450);
            builder.Property(t => t.Description).HasMaxLength(512);
            builder.Property(t => t.IdempotencyKey).HasMaxLength(256);
            builder.Property(t => t.Type).HasConversion<string>().HasMaxLength(32);
            builder.HasIndex(t => new { t.UserId, t.CreatedAtUtc });
            builder.HasIndex(t => t.IdempotencyKey).IsUnique();
        }
    }
}

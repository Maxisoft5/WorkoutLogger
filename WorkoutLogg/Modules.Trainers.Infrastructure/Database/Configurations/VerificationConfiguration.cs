using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Database.Configurations
{
    public class TrainerVerificationConfiguration : IEntityTypeConfiguration<TrainerVerification>
    {
        public void Configure(EntityTypeBuilder<TrainerVerification> builder)
        {
            builder.HasKey(v => v.Id);
            builder.Property(v => v.TrainerUserId).IsRequired().HasMaxLength(450);
            builder.Property(v => v.Status).HasConversion<string>().HasMaxLength(32);
            builder.Property(v => v.Badge).HasConversion<string?>().HasMaxLength(32);
            builder.Property(v => v.ModeratorComment).HasMaxLength(2000);
            builder.Property(v => v.ReviewedByUserId).HasMaxLength(450);
            // Один pending/approved на тренера (нельзя подать несколько заявок).
            builder.HasIndex(v => v.TrainerUserId).IsUnique();
        }
    }

    public class VerificationDocumentConfiguration : IEntityTypeConfiguration<VerificationDocument>
    {
        public void Configure(EntityTypeBuilder<VerificationDocument> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.FileName).IsRequired().HasMaxLength(500);
            builder.Property(d => d.FileUrl).IsRequired().HasMaxLength(2048);
            builder.Property(d => d.Type).HasConversion<string>().HasMaxLength(32);
            builder.HasOne(d => d.Verification)
                .WithMany()
                .HasForeignKey(d => d.VerificationId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(d => d.VerificationId);
        }
    }
}

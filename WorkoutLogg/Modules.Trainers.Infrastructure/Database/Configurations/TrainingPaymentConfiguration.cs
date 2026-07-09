using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Database.Configurations
{
    public class TrainingPaymentConfiguration : IEntityTypeConfiguration<TrainingPayment>
    {
        public void Configure(EntityTypeBuilder<TrainingPayment> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.StudentUserId).IsRequired().HasMaxLength(450);
            builder.Property(p => p.TrainerUserId).IsRequired().HasMaxLength(450);
            builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(32);
            builder.HasIndex(p => new { p.StudentUserId, p.Status });
            builder.HasIndex(p => new { p.TrainerUserId, p.Status });
        }
    }
}

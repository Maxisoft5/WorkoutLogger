using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Database.Configurations
{
    public class TrainingRequestConfiguration : IEntityTypeConfiguration<TrainingRequest>
    {
        public void Configure(EntityTypeBuilder<TrainingRequest> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.StudentUserId).IsRequired().HasMaxLength(450);
            builder.Property(r => r.TrainerUserId).HasMaxLength(450);
            builder.Property(r => r.Schedule).HasMaxLength(256);
            builder.Property(r => r.Message).HasMaxLength(2000);
            builder.Property(r => r.DeclineReason).HasMaxLength(1000);
            builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(32);
            builder.Property(r => r.Level).HasConversion<string>().HasMaxLength(32);

            // Входящие заявки тренера и лента открытых заявок.
            builder.HasIndex(r => new { r.TrainerUserId, r.Status });
            builder.HasIndex(r => new { r.StudentUserId, r.Status });
        }
    }
}

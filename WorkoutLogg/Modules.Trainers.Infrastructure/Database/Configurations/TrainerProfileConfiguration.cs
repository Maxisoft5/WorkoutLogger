using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Database.Configurations
{
    public class TrainerProfileConfiguration : IEntityTypeConfiguration<TrainerProfile>
    {
        public void Configure(EntityTypeBuilder<TrainerProfile> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.UserId).IsRequired().HasMaxLength(450);
            builder.Property(p => p.About).HasMaxLength(2000);
            builder.Property(p => p.Experience).HasConversion<string>().HasMaxLength(32);
            builder.HasIndex(p => p.UserId).IsUnique();
            builder.HasIndex(p => p.IsActive);
        }
    }
}

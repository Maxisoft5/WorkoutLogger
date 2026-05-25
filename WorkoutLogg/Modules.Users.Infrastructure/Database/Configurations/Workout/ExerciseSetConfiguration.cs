using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Users.Domain.Exercies;

namespace Modules.Users.Infrastructure.Database.Configurations.Workout
{
    public class ExerciseSetConfiguration : IEntityTypeConfiguration<ExerciseSet>
    {
        public void Configure(EntityTypeBuilder<ExerciseSet> builder)
        {
            builder.ToTable("exercise_sets");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SetNumber).HasColumnName("set_number");
            builder.Property(x => x.Reps).HasColumnName("reps");
            builder.Property(x => x.WeightKg).HasColumnName("weight_kg").HasDefaultValue(0.0);
            builder.Property(x => x.RestSeconds).HasColumnName("rest_seconds").HasDefaultValue(60);
            builder.Property(x => x.IsWarmup).HasColumnName("is_warmup").HasDefaultValue(false);
            builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at")
                .HasDefaultValueSql("now() at time zone 'utc'");
            builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at")
                .HasDefaultValueSql("now() at time zone 'utc'");
            builder.HasOne(x => x.Exercise).WithMany(x => x.Sets)
                .HasForeignKey(x => x.ExerciseId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}

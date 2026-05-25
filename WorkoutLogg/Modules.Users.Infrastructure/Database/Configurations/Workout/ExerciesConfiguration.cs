using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moduels.Workouts.DTO.Enums;
using Modules.Users.Domain.Exercies;

namespace Modules.Users.Infrastructure.Database.Configurations.Workout
{
    public class ExerciesConfiguration : IEntityTypeConfiguration<Domain.Exercies.Exercise>
    {
        public void Configure(EntityTypeBuilder<Exercise> builder)
        {
            builder.ToTable("users_exercises");
            builder.HasKey(x => x.Id);
            builder.Property(e => e.ExerciesComplexity)
               .HasColumnName("exercies_complexity")
               .HasDefaultValue(ExerciesComplexity.Low);
            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.Description).HasColumnName("description");
            builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at")
                .HasDefaultValueSql("now() at time zone 'utc'");
            builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at")
                .HasDefaultValueSql("now() at time zone 'utc'");
            builder.HasOne(x => x.Workout).WithMany(x => x.Exercises)
                .HasForeignKey(x => x.WorkoutId);
            builder.HasMany(x => x.Sets).WithOne(x => x.Exercise)
                .HasForeignKey(x => x.ExerciseId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}

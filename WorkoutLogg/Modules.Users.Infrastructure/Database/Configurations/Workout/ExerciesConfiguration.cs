using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moduels.Workouts.DTO.Enums;
using Modules.Users.Domain.Exercies;
using System;
using System.Collections.Generic;
using System.Text;

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
            builder.Property(x => x.NumberSets).HasColumnName("sets_number");
            builder.Property(x => x.NumberRepetition).HasColumnName("repetition_number");
            builder.Property(x => x.RestSeconds).HasColumnName("rest_seconds");
            builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at").HasDefaultValueSql("now() at time zone 'utc'");
            builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at").HasDefaultValueSql("now() at time zone 'utc'");
            builder.HasOne(x => x.Workout).WithMany(x => x.Exercises)
                .HasForeignKey(x => x.WorkoutId);

        }
    }
}

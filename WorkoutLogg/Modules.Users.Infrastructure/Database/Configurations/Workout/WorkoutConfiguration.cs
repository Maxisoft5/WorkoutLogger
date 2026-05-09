using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moduels.Workouts.DTO.Enums;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Workout;
using Modules.Users.DTO.Auth;

namespace Modules.Users.Infrastructure.Database.Configurations.Workout
{
    public class WorkoutConfiguration : IEntityTypeConfiguration<Domain.Workout.WorkoutModel>
    {
        public void Configure(EntityTypeBuilder<Domain.Workout.WorkoutModel> builder)
        {
            builder.ToTable("users_workouts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.StartDate).HasColumnName("start_date");
            builder.Property(x => x.EndDate).HasColumnName("end_date");
            builder.Property(e => e.WorkoutType)
                .HasColumnName("workout_type")
                .HasDefaultValue(WorkoutType.All);
            builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at").HasDefaultValueSql("now() at time zone 'utc'");
            builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at").HasDefaultValueSql("now() at time zone 'utc'");

            builder.HasOne(x => x.User)
                .WithMany(x => x.Workouts).HasForeignKey(x => x.UserId);

            builder.HasMany(x => x.Exercises).WithOne(x => x.Workout)
                .HasForeignKey(x => x.WorkoutId);

        }
    }
}

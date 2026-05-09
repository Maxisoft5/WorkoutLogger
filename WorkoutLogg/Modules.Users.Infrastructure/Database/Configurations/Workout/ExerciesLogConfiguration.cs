using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Users.Domain.Logs;

namespace Modules.Users.Infrastructure.Database.Configurations.Workout
{
    public class ExerciesLogConfiguration : IEntityTypeConfiguration<Domain.Logs.WorkoutLog>
    {
        public void Configure(EntityTypeBuilder<WorkoutLog> builder)
        {
            builder.ToTable("workout_log");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DateLog).HasColumnName("log_date");
            builder.Property(x => x.SetNumber).HasColumnName("set_number");
            builder.Property(x => x.Kg).HasColumnName("kg");
            builder.Property(x => x.RestSeconds).HasColumnName("rest_seconds");
            builder.Property(x => x.RepNumber).HasColumnName("rep_number");
            builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at").HasDefaultValueSql("now() at time zone 'utc'");
            builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at").HasDefaultValueSql("now() at time zone 'utc'");
            builder.Property(x => x.IsHistoryRecord).HasColumnName("is_history_record");
            builder.HasOne(x => x.Exercise).WithMany(x => x.Logs)
                .HasForeignKey(x => x.ExerciseId);
        }
    }
}

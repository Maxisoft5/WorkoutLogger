using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Users.Domain.Users;
using Modules.Users.DTO.Auth;
using Modules.Users.DTO.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Users.Infrastructure.Database.Configurations
{
    public class UserGoalConfiguration : IEntityTypeConfiguration<UserGoal>
    {
        public void Configure(EntityTypeBuilder<UserGoal> builder)
        {
            builder.ToTable("user_goals");
            builder.HasKey(x => x.Id);
            builder.Property(e => e.Goal)
                 .HasColumnName("goal")
                 .HasDefaultValue(UserGoalVariant.LoseFat);
            builder.HasOne(x => x.User).WithMany(x => x.UserGoals).HasForeignKey(x => x.UserId);
            builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at").HasDefaultValueSql("now() at time zone 'utc'");
            builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at").HasDefaultValueSql("now() at time zone 'utc'");

        }
    }
}

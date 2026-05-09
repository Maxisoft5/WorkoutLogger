using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Users.Domain.Users;
using Modules.Users.DTO.Auth;
using Modules.Users.DTO.Users;


namespace Modules.Users.Infrastructure.Database.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.HasIndex(x => x.NormalizedUserName).IsUnique(false);
            builder.HasIndex(x => x.UserName).IsUnique(false);
            builder.Property(x=> x.DateOfBirth).HasColumnName("birth_date");
       
            builder.Property(e => e.Identity)
                .HasColumnName("identity")
                .HasDefaultValue(UserSex.Male);

            builder.Property(e => e.WorkOutCount)
               .HasColumnName("workout_count")
               .HasDefaultValue(WorkOutCountVariant.One);

            builder.ComplexProperty(e => e.BodyStats, bs =>
            {
                bs.Property(b => b.Kg).HasColumnName("kg");
                bs.Property(b => b.Cm).HasColumnName("cm");
                bs.Property(b => b.Fat).HasColumnName("fat");
            });

            builder.HasMany(e => e.UserGoals)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId);

            // Each User can have many UserClaims
            builder.HasMany(e => e.Claims)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            // Each User can have many UserLogins
            builder.HasMany(e => e.UserLogins)
                .WithOne(e => e.User)
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            builder.HasMany(e => e.UserTokens)
                .WithOne(e => e.User)
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            builder.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

       
        }
    }
}

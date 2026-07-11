using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Database.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.StudentUserId).IsRequired().HasMaxLength(450);
            builder.Property(r => r.TrainerUserId).IsRequired().HasMaxLength(450);
            builder.Property(r => r.Text).HasMaxLength(2000);
            builder.Property(r => r.TrainerReply).HasMaxLength(2000);
            // Один отзыв на платёж.
            builder.HasIndex(r => r.PaymentId).IsUnique();
            // Агрегация рейтинга и вывод отзывов тренера (основные запросы).
            builder.HasIndex(r => new { r.TrainerUserId, r.CreatedAtUtc });
        }
    }
}

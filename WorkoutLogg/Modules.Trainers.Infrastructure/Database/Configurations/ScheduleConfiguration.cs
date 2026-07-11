using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Database.Configurations
{
    public class AvailabilitySlotConfiguration : IEntityTypeConfiguration<AvailabilitySlot>
    {
        public void Configure(EntityTypeBuilder<AvailabilitySlot> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.TrainerUserId).IsRequired().HasMaxLength(450);
            builder.Property(s => s.Note).HasMaxLength(500);
            // Поиск доступных слотов тренера в диапазоне дат (основной сценарий).
            builder.HasIndex(s => new { s.TrainerUserId, s.StartUtc, s.IsBooked });
        }
    }

    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.StudentUserId).IsRequired().HasMaxLength(450);
            builder.Property(b => b.TrainerUserId).IsRequired().HasMaxLength(450);
            builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(32);
            builder.Property(b => b.CancelledBy).HasConversion<string?>().HasMaxLength(32);
            builder.Property(b => b.StudentNote).HasMaxLength(1000);
            builder.Property(b => b.CancellationReason).HasMaxLength(1000);
            builder.HasIndex(b => new { b.StudentUserId, b.Status });
            builder.HasIndex(b => new { b.TrainerUserId, b.Status });
            builder.HasIndex(b => b.SlotId).IsUnique(); // один слот — одно активное бронирование
        }
    }
}

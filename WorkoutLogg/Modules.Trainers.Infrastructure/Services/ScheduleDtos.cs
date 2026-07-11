using System.ComponentModel.DataAnnotations;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    // ─── Requests ───────────────────────────────────────────────────────────────

    public class CreateSlotRequest
    {
        /// <summary>Начало слота (UTC).</summary>
        [Required]
        public DateTime StartUtc { get; set; }

        /// <summary>Конец слота (UTC). Должен быть позже StartUtc.</summary>
        [Required]
        public DateTime EndUtc { get; set; }

        /// <summary>Необязательное примечание («онлайн», «зал на Арбате»).</summary>
        [MaxLength(500)]
        public string? Note { get; set; }
    }

    public class CreateBookingRequest
    {
        [Required]
        public Guid SlotId { get; set; }

        [MaxLength(1000)]
        public string? Note { get; set; }
    }

    public class CancelBookingRequest
    {
        [MaxLength(1000)]
        public string? Reason { get; set; }
    }

    // ─── DTOs ────────────────────────────────────────────────────────────────────

    public class SlotDto
    {
        public Guid Id { get; set; }
        public string TrainerUserId { get; set; } = null!;
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public int DurationMinutes { get; set; }
        public string? Note { get; set; }
        public bool IsBooked { get; set; }
    }

    public class BookingDto
    {
        public Guid Id { get; set; }
        public string StudentUserId { get; set; } = null!;
        public string TrainerUserId { get; set; } = null!;
        public Guid SlotId { get; set; }
        public Guid? PaymentId { get; set; }
        public string Status { get; set; } = null!;
        public string? StudentNote { get; set; }
        public string? CancellationReason { get; set; }
        public string? CancelledBy { get; set; }
        public bool IsLateCancel { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ConfirmedAtUtc { get; set; }
        public DateTime? CancelledAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }

        /// <summary>Слот, на который сделано бронирование (денормализовано для удобства).</summary>
        public SlotDto? Slot { get; set; }
    }

    internal static class ScheduleMapper
    {
        public static SlotDto ToDto(AvailabilitySlot s) => new()
        {
            Id = s.Id,
            TrainerUserId = s.TrainerUserId,
            StartUtc = s.StartUtc,
            EndUtc = s.EndUtc,
            DurationMinutes = s.DurationMinutes,
            Note = s.Note,
            IsBooked = s.IsBooked,
        };

        public static BookingDto ToDto(Booking b, AvailabilitySlot? slot = null) => new()
        {
            Id = b.Id,
            StudentUserId = b.StudentUserId,
            TrainerUserId = b.TrainerUserId,
            SlotId = b.SlotId,
            PaymentId = b.PaymentId,
            Status = b.Status.ToString(),
            StudentNote = b.StudentNote,
            CancellationReason = b.CancellationReason,
            CancelledBy = b.CancelledBy?.ToString(),
            IsLateCancel = b.IsLateCancel,
            CreatedAtUtc = b.CreatedAtUtc,
            ConfirmedAtUtc = b.ConfirmedAtUtc,
            CancelledAtUtc = b.CancelledAtUtc,
            CompletedAtUtc = b.CompletedAtUtc,
            Slot = slot is null ? null : ToDto(slot),
        };
    }
}

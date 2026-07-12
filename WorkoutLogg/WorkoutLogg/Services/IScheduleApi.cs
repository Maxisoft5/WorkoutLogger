using Refit;

namespace WorkoutLogg.Services
{
    // Client-side mirror of Modules.Trainers schedule DTOs (см. ScheduleDtos).

    public class SlotDto
    {
        public Guid Id { get; set; }
        public string TrainerUserId { get; set; } = "";
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public int DurationMinutes { get; set; }
        public string? Note { get; set; }
        public bool IsBooked { get; set; }
    }

    public class BookingDto
    {
        public Guid Id { get; set; }
        public string StudentUserId { get; set; } = "";
        public string TrainerUserId { get; set; } = "";
        public Guid SlotId { get; set; }
        public Guid? PaymentId { get; set; }
        public string Status { get; set; } = "";
        public string? StudentNote { get; set; }
        public string? CancellationReason { get; set; }
        public string? CancelledBy { get; set; }
        public bool IsLateCancel { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ConfirmedAtUtc { get; set; }
        public DateTime? CancelledAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
        public SlotDto? Slot { get; set; }
    }

    public record CreateBookingRequestDto(Guid SlotId, string? Note);

    public record CancelBookingRequestDto(string? Reason);

    /// <summary>
    /// REST-контракт расписания и бронирований (M7): свободные слоты тренера,
    /// бронирование учеником, мои бронирования и отмена.
    /// </summary>
    public interface IScheduleApi
    {
        /// <summary>Свободные слоты тренера в диапазоне дат (UTC).</summary>
        [Get("/api/schedule/slots")]
        Task<IApiResponse<List<SlotDto>>> GetAvailableSlotsAsync(
            [Header("Authorization")] string token,
            [AliasAs("trainerId")] string trainerId,
            [AliasAs("from")] DateTime fromUtc,
            [AliasAs("to")] DateTime toUtc,
            CancellationToken ct = default);

        /// <summary>Ученик бронирует свободный слот.</summary>
        [Post("/api/schedule/bookings")]
        Task<IApiResponse<BookingDto>> BookAsync(
            [Header("Authorization")] string token,
            [Body] CreateBookingRequestDto body,
            CancellationToken ct = default);

        /// <summary>Бронирования текущего ученика.</summary>
        [Get("/api/schedule/bookings/my")]
        Task<IApiResponse<List<BookingDto>>> GetMyBookingsAsync(
            [Header("Authorization")] string token,
            CancellationToken ct = default);

        /// <summary>Отмена бронирования (учеником или тренером).</summary>
        [Post("/api/schedule/bookings/{bookingId}/cancel")]
        Task<IApiResponse<BookingDto>> CancelBookingAsync(
            [Header("Authorization")] string token,
            Guid bookingId,
            [Body] CancelBookingRequestDto body,
            CancellationToken ct = default);
    }
}

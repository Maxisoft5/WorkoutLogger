using Modules.Common.Domain.Results;

namespace Modules.Trainers.Infrastructure.Services
{
    public interface IScheduleService
    {
        // ─── Слоты (тренер управляет) ─────────────────────────────────────────

        /// <summary>Тренер добавляет один слот в расписание.</summary>
        Task<Result<SlotDto>> AddSlotAsync(string trainerUserId, CreateSlotRequest request, CancellationToken ct = default);

        /// <summary>Слоты тренера для просмотра учеником в диапазоне дат (только свободные).</summary>
        Task<List<SlotDto>> GetAvailableSlotsAsync(string trainerUserId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);

        /// <summary>Все слоты тренера в диапазоне дат (включая забронированные) — для личного кабинета.</summary>
        Task<List<SlotDto>> GetMyScheduleAsync(string trainerUserId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);

        /// <summary>Тренер удаляет незабронированный слот.</summary>
        Task<Result> DeleteSlotAsync(string trainerUserId, Guid slotId, CancellationToken ct = default);

        // ─── Бронирования ─────────────────────────────────────────────────────

        /// <summary>Ученик бронирует свободный слот тренера.</summary>
        Task<Result<BookingDto>> BookAsync(string studentUserId, CreateBookingRequest request, CancellationToken ct = default);

        /// <summary>Список бронирований текущего ученика.</summary>
        Task<List<BookingDto>> GetStudentBookingsAsync(string studentUserId, CancellationToken ct = default);

        /// <summary>Список бронирований тренера.</summary>
        Task<List<BookingDto>> GetTrainerBookingsAsync(string trainerUserId, CancellationToken ct = default);

        /// <summary>Тренер подтверждает бронирование.</summary>
        Task<Result<BookingDto>> ConfirmAsync(string trainerUserId, Guid bookingId, CancellationToken ct = default);

        /// <summary>Отмена бронирования (учеником или тренером).</summary>
        Task<Result<BookingDto>> CancelAsync(string userId, Guid bookingId, CancelBookingRequest request, CancellationToken ct = default);

        /// <summary>Тренер отмечает тренировку как состоявшуюся.</summary>
        Task<Result<BookingDto>> CompleteAsync(string trainerUserId, Guid bookingId, CancellationToken ct = default);

        /// <summary>Тренер отмечает no-show (ученик не явился).</summary>
        Task<Result<BookingDto>> MarkNoShowAsync(string trainerUserId, Guid bookingId, CancellationToken ct = default);
    }
}

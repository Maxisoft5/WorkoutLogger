namespace Modules.Trainers.Infrastructure.Domain
{
    /// <summary>
    /// Бронирование ученика на слот тренера.
    /// Жизненный цикл: Pending → Confirmed / Cancelled → Completed / NoShow.
    /// </summary>
    public class Booking
    {
        public Guid Id { get; set; }

        public string StudentUserId { get; set; } = null!;
        public string TrainerUserId { get; set; } = null!;

        public Guid SlotId { get; set; }

        /// <summary>
        /// Ссылка на оплату M5. Null — бронирование без оплаты (MVP: оплата и бронь независимы,
        /// можно связать позже через POST /api/trainers/bookings/{id}/link-payment).
        /// </summary>
        public Guid? PaymentId { get; set; }

        public BookingStatus Status { get; set; }

        public string? StudentNote { get; set; }
        public string? CancellationReason { get; set; }
        public BookingCancelledBy? CancelledBy { get; set; }

        /// <summary>Признак «позднего» отмены (< 24 ч до начала слота). Не блокирует отмену, но фиксируется.</summary>
        public bool IsLateCancel { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ConfirmedAtUtc { get; set; }
        public DateTime? CancelledAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
    }

    public enum BookingStatus
    {
        /// <summary>Ученик забронировал, тренер ещё не подтвердил.</summary>
        Pending = 0,

        /// <summary>Тренер подтвердил бронирование.</summary>
        Confirmed = 1,

        /// <summary>Отменено (учеником или тренером).</summary>
        Cancelled = 2,

        /// <summary>Тренировка состоялась, тренер отметил «завершено».</summary>
        Completed = 3,

        /// <summary>Ученик не явился (тренер отметил).</summary>
        NoShow = 4,
    }

    public enum BookingCancelledBy
    {
        Student = 0,
        Trainer = 1,
    }
}

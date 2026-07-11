namespace Modules.Trainers.Infrastructure.Domain
{
    /// <summary>
    /// Слот доступности тренера — единица расписания.
    /// Тренер открывает слоты, ученик их бронирует.
    /// </summary>
    public class AvailabilitySlot
    {
        public Guid Id { get; set; }

        public string TrainerUserId { get; set; } = null!;

        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }

        /// <summary>Продолжительность в минутах (вычисляется, храним для индекса).</summary>
        public int DurationMinutes { get; set; }

        /// <summary>Примечание тренера к слоту (например «онлайн», «зал»).</summary>
        public string? Note { get; set; }

        public bool IsBooked { get; set; }

        /// <summary>Null пока слот не занят; ссылается на Booking.SlotId после бронирования.</summary>
        public Guid? BookingId { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}

namespace Modules.Trainers.Infrastructure.Domain
{
    /// <summary>
    /// Оплата тренировки FitCoins с эскроу-логикой (M5, риск №2 из отчёта-анализа):
    /// при оплате FC списываются с кошелька ученика и удерживаются платформой (Held),
    /// после подтверждения тренировки учеником — выплата тренеру за вычетом комиссии,
    /// при отказе тренера — возврат ученику.
    /// </summary>
    public class TrainingPayment
    {
        public Guid Id { get; set; }

        public string StudentUserId { get; set; } = null!;
        public string TrainerUserId { get; set; } = null!;

        /// <summary>Цена тренировки в FitCoins на момент оплаты (снимок с карточки тренера).</summary>
        public int PriceFc { get; set; }

        /// <summary>Комиссия платформы (10% от цены, экран 01 дизайна).</summary>
        public int CommissionFc { get; set; }

        /// <summary>Выплата тренеру: цена минус комиссия.</summary>
        public int PayoutFc { get; set; }

        public TrainingPaymentStatus Status { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ResolvedAtUtc { get; set; }
    }

    public enum TrainingPaymentStatus
    {
        /// <summary>FC списаны у ученика и удерживаются платформой.</summary>
        Held = 0,

        /// <summary>Ученик подтвердил тренировку — тренеру выплачено.</summary>
        Completed = 1,

        /// <summary>Тренер вернул оплату — FC возвращены ученику.</summary>
        Refunded = 2
    }

    /// <summary>Комиссия платформы (экран 01: «комиссия платформы 10%»).</summary>
    public static class PlatformFees
    {
        public const int CommissionPercent = 10;

        public static int CommissionFor(int priceFc) =>
            (int)Math.Round(priceFc * (CommissionPercent / 100.0), MidpointRounding.AwayFromZero);
    }
}

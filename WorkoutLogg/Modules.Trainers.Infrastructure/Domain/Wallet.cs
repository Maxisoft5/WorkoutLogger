namespace Modules.Trainers.Infrastructure.Domain
{
    /// <summary>
    /// Кошелёк FitCoins (экран 04 «Профиль»). Валюта маркетплейса: 1 FC ≈ 1 ₽.
    /// В MVP FitCoins только зарабатываются (серии, челленджи, рефералы) —
    /// пополнение за деньги отложено из-за требований IAP Apple/Google (см. отчёт-анализ).
    /// </summary>
    public class Wallet
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public int Balance { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }

    /// <summary>Операция по кошельку (блок «История» на экране 04).</summary>
    public class WalletTransaction
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;

        /// <summary>Положительная — начисление, отрицательная — списание.</summary>
        public int Amount { get; set; }

        public WalletTransactionType Type { get; set; }
        public string? Description { get; set; }

        /// <summary>Ключ идемпотентности: защита от двойного начисления одного бонуса.</summary>
        public string? IdempotencyKey { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }

    public enum WalletTransactionType
    {
        /// <summary>+50 — серия тренировок 7 дней.</summary>
        StreakBonus = 0,

        /// <summary>+200 — выполненный челлендж.</summary>
        ChallengeReward = 1,

        /// <summary>+300 — приглашённый друг.</summary>
        ReferralBonus = 2,

        /// <summary>Оплата тренировки (списание у ученика).</summary>
        TrainingPayment = 3,

        /// <summary>Выплата тренеру за тренировку (за вычетом комиссии платформы).</summary>
        TrainingPayout = 4,

        Refund = 5,
        Adjustment = 6
    }

    /// <summary>Суммы бонусов из блока «Заработать FitCoins» на экране 04.</summary>
    public static class RewardAmounts
    {
        public const int Streak7Days = 50;
        public const int Challenge = 200;
        public const int Referral = 300;

        /// <summary>Длина серии тренировок для бонуса.</summary>
        public const int StreakLengthDays = 7;
    }
}

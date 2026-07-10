namespace Modules.Trainers.Infrastructure.Domain
{
    /// <summary>
    /// Публичная карточка тренера в маркетплейсе (экран 01 «Карточка тренера»).
    /// Создаётся после выбора роли «Тренер» при регистрации, редактируется из профиля.
    /// </summary>
    public class TrainerProfile
    {
        public Guid Id { get; set; }

        /// <summary>Id пользователя из модуля Users (IdentityUser.Id). У пользователя ровно один профиль тренера.</summary>
        public string UserId { get; set; } = null!;

        public TrainerSpecializations Specializations { get; set; }

        public ExperienceRange Experience { get; set; }

        public TrainingFormats Formats { get; set; }

        /// <summary>Цена одной тренировки в FitCoins (1 FC ≈ 1 ₽).</summary>
        public int PricePerSession { get; set; }

        /// <summary>Короткое описание «О себе» для карточки в поиске.</summary>
        public string? About { get; set; }

        /// <summary>Показывается ли карточка в поиске учеников. Выключается при смене роли на «Ученик».</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Выставляется при одобрении заявки на верификацию (M9).</summary>
        public bool HasVerifiedBadge { get; set; }

        /// <summary>Конкретный бейдж (Verified / Master). Null если не верифицирован.</summary>
        public VerificationBadge? VerificationBadge { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }

    [Flags]
    public enum TrainerSpecializations
    {
        None = 0,
        Strength = 1,
        WeightLoss = 2,
        Crossfit = 4,
        Yoga = 8,
        Rehabilitation = 16,
        Running = 32
    }

    public enum ExperienceRange
    {
        LessThanOneYear = 0,
        OneToThreeYears = 1,
        ThreeToSevenYears = 2,
        SevenPlusYears = 3
    }

    [Flags]
    public enum TrainingFormats
    {
        None = 0,
        Online = 1,
        Gym = 2,
        OnSite = 4
    }
}

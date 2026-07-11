using Refit;

namespace WorkoutLogg.Services
{
    // Client-side mirror of the Trainers module enums. The server serialises enums as
    // strings (JsonStringEnumConverter); the Refit client for this API is registered with
    // the same converter, so values round-trip by name. Numeric values match the server
    // so [Flags] search filters can also be passed as ints in the query string.

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

    public enum StudentLevel
    {
        Beginner = 0,
        Intermediate = 1,
        Advanced = 2
    }

    public enum TrainerSortBy
    {
        Match = 0,
        PriceAsc = 1,
        PriceDesc = 2,
        Newest = 3
    }

    public enum TrainingRequestStatus
    {
        Pending = 0,
        Accepted = 1,
        Declined = 2,
        Cancelled = 3
    }

    public record TrainerProfileDto(
        Guid Id,
        string UserId,
        TrainerSpecializations Specializations,
        ExperienceRange Experience,
        TrainingFormats Formats,
        int PricePerSession,
        string? About,
        bool IsActive,
        bool HasVerifiedBadge,
        string? VerificationBadge,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc);

    public record TrainerSearchItemDto(
        TrainerProfileDto Profile,
        int MatchScore,
        double? AverageRating,
        int ReviewCount);

    public record TrainerSearchPageDto(
        List<TrainerSearchItemDto> Items, int Page, int PageSize, int TotalCount);

    public record CreateTrainingRequestDto(
        string? TrainerUserId,
        TrainerSpecializations Goal,
        StudentLevel Level,
        TrainingFormats Formats,
        string? Schedule,
        int? Budget,
        string? Message);

    public record TrainingRequestDto(
        Guid Id,
        string StudentUserId,
        string? TrainerUserId,
        TrainerSpecializations Goal,
        StudentLevel Level,
        TrainingFormats Formats,
        string? Schedule,
        int? Budget,
        string? Message,
        TrainingRequestStatus Status,
        string? DeclineReason,
        DateTime CreatedAtUtc,
        DateTime? RespondedAtUtc);

    /// <summary>
    /// REST-контракт маркетплейса тренеров (экран 02 «Ученик: вкладка Тренеры»).
    /// Токен передаётся явно, как в остальных клиентах (IWorkoutsApi/ISubscriptionsApi).
    /// </summary>
    public interface ITrainersApi
    {
        /// <summary>Поиск тренеров с фильтрами и match-скором. Флаги передаются числом.</summary>
        [Get("/api/trainers/search")]
        Task<IApiResponse<TrainerSearchPageDto>> SearchAsync(
            [Header("Authorization")] string token,
            [AliasAs("specializations")] int specializations,
            [AliasAs("formats")] int formats,
            [AliasAs("priceMin")] int? priceMin,
            [AliasAs("priceMax")] int? priceMax,
            [AliasAs("minRating")] double? minRating,
            [AliasAs("sortBy")] int sortBy,
            [AliasAs("page")] int page,
            [AliasAs("pageSize")] int pageSize,
            CancellationToken ct = default);

        /// <summary>Блок «Подобрано для вас»: топ тренеров по match-скору из целей ученика.</summary>
        [Get("/api/trainers/recommended")]
        Task<IApiResponse<List<TrainerSearchItemDto>>> GetRecommendedAsync(
            [Header("Authorization")] string token,
            [AliasAs("top")] int top = 3,
            CancellationToken ct = default);

        /// <summary>Ученик отправляет заявку конкретному тренеру.</summary>
        [Post("/api/trainers/requests")]
        Task<IApiResponse<TrainingRequestDto>> CreateRequestAsync(
            [Header("Authorization")] string token,
            [Body] CreateTrainingRequestDto body,
            CancellationToken ct = default);
    }
}

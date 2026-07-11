using Refit;

namespace WorkoutLogg.Services
{
    // Server serialises WalletTransactionType as a string (JsonStringEnumConverter);
    // the Refit client for this API is registered with the same converter.
    public enum WalletTransactionType
    {
        StreakBonus = 0,
        ChallengeReward = 1,
        ReferralBonus = 2,
        TrainingPayment = 3,
        TrainingPayout = 4,
        Refund = 5,
        Adjustment = 6
    }

    public record WalletDto(string UserId, int Balance);

    public record WalletTransactionDto(
        Guid Id,
        int Amount,
        WalletTransactionType Type,
        string? Description,
        DateTime CreatedAtUtc);

    public record WalletHistoryPageDto(
        List<WalletTransactionDto> Items, int Page, int PageSize, int TotalCount);

    /// <summary>
    /// Кошелёк FitCoins (экран 04 «Профиль»). Токен передаётся явно, как в остальных клиентах.
    /// </summary>
    public interface IWalletApi
    {
        /// <summary>Баланс кошелька (создаётся лениво на сервере).</summary>
        [Get("/api/wallet")]
        Task<IApiResponse<WalletDto>> GetWalletAsync(
            [Header("Authorization")] string token,
            CancellationToken ct = default);

        /// <summary>История операций, новые сверху.</summary>
        [Get("/api/wallet/history")]
        Task<IApiResponse<WalletHistoryPageDto>> GetHistoryAsync(
            [Header("Authorization")] string token,
            [AliasAs("page")] int page,
            [AliasAs("pageSize")] int pageSize,
            CancellationToken ct = default);

        /// <summary>Забрать бонус «+50 за серию 7 дней» (серию проверяет сервер по журналу тренировок).</summary>
        [Post("/api/wallet/rewards/streak")]
        Task<IApiResponse<WalletDto>> ClaimStreakBonusAsync(
            [Header("Authorization")] string token,
            CancellationToken ct = default);
    }
}

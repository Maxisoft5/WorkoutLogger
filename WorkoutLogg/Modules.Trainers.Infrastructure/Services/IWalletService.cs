using Modules.Common.Domain.Results;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    public interface IWalletService
    {
        /// <summary>Баланс кошелька (кошелёк создаётся лениво при первом обращении).</summary>
        Task<WalletDto> GetWalletAsync(string userId, CancellationToken ct = default);

        /// <summary>История операций, новые сверху (блок «История» экрана 04).</summary>
        Task<WalletHistoryPageDto> GetHistoryAsync(string userId, int page, int pageSize, CancellationToken ct = default);

        /// <summary>Начисление FitCoins. Ключ идемпотентности защищает от двойного начисления бонуса.</summary>
        Task<Result<WalletDto>> CreditAsync(string userId, int amount, WalletTransactionType type,
            string? description, string? idempotencyKey = null, CancellationToken ct = default);

        /// <summary>Списание FitCoins с проверкой достаточности баланса.</summary>
        Task<Result<WalletDto>> DebitAsync(string userId, int amount, WalletTransactionType type,
            string? description, string? idempotencyKey = null, CancellationToken ct = default);

        /// <summary>
        /// Бонус «+50 за серию 7 дней»: требуется тренировка в каждый из последних 7 дней
        /// (по UTC, включая сегодня) и не более одного бонуса за 7 дней.
        /// Даты тренировок передаёт вызывающий модуль (Users).
        /// </summary>
        Task<Result<WalletDto>> ClaimStreakBonusAsync(string userId, IReadOnlyCollection<DateTime> workoutDatesUtc,
            DateTime? nowUtc = null, CancellationToken ct = default);
    }
}

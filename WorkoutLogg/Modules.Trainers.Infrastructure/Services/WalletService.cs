using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Results;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    public class WalletService(TrainersDbContext dbContext) : IWalletService
    {
        public async Task<WalletDto> GetWalletAsync(string userId, CancellationToken ct = default)
        {
            var wallet = await GetOrCreateWalletAsync(userId, ct);
            return new WalletDto { UserId = wallet.UserId, Balance = wallet.Balance };
        }

        public async Task<WalletHistoryPageDto> GetHistoryAsync(
            string userId, int page, int pageSize, CancellationToken ct = default)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = dbContext.WalletTransactions
                .AsNoTracking()
                .Where(t => t.UserId == userId);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(t => t.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new WalletHistoryPageDto
            {
                Items = items.Select(t => t.MapTransaction()).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public Task<Result<WalletDto>> CreditAsync(string userId, int amount, WalletTransactionType type,
            string? description, string? idempotencyKey = null, CancellationToken ct = default)
            => ApplyAsync(userId, amount, type, description, idempotencyKey, isDebit: false, ct);

        public Task<Result<WalletDto>> DebitAsync(string userId, int amount, WalletTransactionType type,
            string? description, string? idempotencyKey = null, CancellationToken ct = default)
            => ApplyAsync(userId, amount, type, description, idempotencyKey, isDebit: true, ct);

        public async Task<Result<WalletDto>> ClaimStreakBonusAsync(
            string userId, IReadOnlyCollection<DateTime> workoutDatesUtc,
            DateTime? nowUtc = null, CancellationToken ct = default)
        {
            var today = (nowUtc ?? DateTime.UtcNow).Date;
            var workoutDays = workoutDatesUtc.Select(d => d.Date).ToHashSet();

            var streakDays = Enumerable.Range(0, RewardAmounts.StreakLengthDays)
                .Select(offset => today.AddDays(-offset));
            if (!streakDays.All(workoutDays.Contains))
                return new Result<WalletDto>(TrainerErrors.StreakNotReached());

            // Не более одного бонуса за серию: последнее начисление должно быть старше 7 дней.
            var windowStart = today.AddDays(-(RewardAmounts.StreakLengthDays - 1));
            var alreadyClaimed = await dbContext.WalletTransactions.AnyAsync(t =>
                t.UserId == userId
                && t.Type == WalletTransactionType.StreakBonus
                && t.CreatedAtUtc >= windowStart, ct);
            if (alreadyClaimed)
                return new Result<WalletDto>(TrainerErrors.StreakBonusAlreadyClaimed());

            return await CreditAsync(userId, RewardAmounts.Streak7Days, WalletTransactionType.StreakBonus,
                $"Серия тренировок {RewardAmounts.StreakLengthDays} дней",
                $"streak7:{userId}:{today:yyyy-MM-dd}", ct);
        }

        private async Task<Result<WalletDto>> ApplyAsync(string userId, int amount, WalletTransactionType type,
            string? description, string? idempotencyKey, bool isDebit, CancellationToken ct)
        {
            if (amount <= 0)
                return new Result<WalletDto>(TrainerErrors.InvalidAmount());

            if (idempotencyKey is not null)
            {
                var duplicate = await dbContext.WalletTransactions
                    .AnyAsync(t => t.IdempotencyKey == idempotencyKey, ct);
                if (duplicate)
                    return new Result<WalletDto>(TrainerErrors.DuplicateOperation());
            }

            var wallet = await GetOrCreateWalletAsync(userId, ct);

            if (isDebit && wallet.Balance < amount)
                return new Result<WalletDto>(TrainerErrors.InsufficientFunds());

            wallet.Balance += isDebit ? -amount : amount;
            wallet.UpdatedAtUtc = DateTime.UtcNow;

            dbContext.WalletTransactions.Add(new WalletTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = isDebit ? -amount : amount,
                Type = type,
                Description = description,
                IdempotencyKey = idempotencyKey,
                CreatedAtUtc = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync(ct);
            return new Result<WalletDto>(new WalletDto { UserId = wallet.UserId, Balance = wallet.Balance });
        }

        private async Task<Wallet> GetOrCreateWalletAsync(string userId, CancellationToken ct)
        {
            var wallet = await dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, ct);
            if (wallet is null)
            {
                wallet = new Wallet
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Balance = 0,
                    CreatedAtUtc = DateTime.UtcNow
                };
                dbContext.Wallets.Add(wallet);
                await dbContext.SaveChangesAsync(ct);
            }

            return wallet;
        }
    }
}

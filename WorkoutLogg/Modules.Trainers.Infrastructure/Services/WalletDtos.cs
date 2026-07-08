using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    public class WalletDto
    {
        public string UserId { get; set; } = null!;
        public int Balance { get; set; }
    }

    public class WalletTransactionDto
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public WalletTransactionType Type { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }

    public class WalletHistoryPageDto
    {
        public List<WalletTransactionDto> Items { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public static class WalletMapper
    {
        public static WalletTransactionDto MapTransaction(this WalletTransaction transaction) => new()
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Description = transaction.Description,
            CreatedAtUtc = transaction.CreatedAtUtc
        };
    }
}

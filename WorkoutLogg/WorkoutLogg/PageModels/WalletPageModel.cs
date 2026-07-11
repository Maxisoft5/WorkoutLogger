using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using WorkoutLogg.Localization;
using WorkoutLogg.Services;

namespace WorkoutLogg.PageModels
{
    /// <summary>
    /// Экран 04 «Профиль: кошелёк FitCoins» (M4): баланс, история операций,
    /// блок «Заработать FitCoins» (бонус за серию 7 дней забирается кнопкой).
    /// </summary>
    public partial class WalletPageModel : ObservableObject
    {
        private readonly IWalletApi _api;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BalanceLabel))]
        private int balance;

        [ObservableProperty]
        private ObservableCollection<WalletTxItem> transactions = [];

        [ObservableProperty]
        private bool isEmpty;

        public string BalanceLabel => $"{Balance} FC";

        public WalletPageModel(IWalletApi api)
        {
            _api = api;
        }

        public async Task LoadAsync()
        {
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token)) return;

            try
            {
                var wallet = await _api.GetWalletAsync($"Bearer {token}");
                if (wallet.IsSuccessStatusCode && wallet.Content is not null)
                    Balance = wallet.Content.Balance;

                var history = await _api.GetHistoryAsync($"Bearer {token}", page: 1, pageSize: 50);
                Transactions = history.IsSuccessStatusCode && history.Content is not null
                    ? new ObservableCollection<WalletTxItem>(history.Content.Items.Select(WalletTxItem.FromDto))
                    : [];
            }
            catch
            {
                Transactions = [];
            }
            finally
            {
                IsEmpty = Transactions.Count == 0;
            }
        }

        /// <summary>Забрать бонус за серию. Возвращает (успех, сообщение для пользователя).</summary>
        public async Task<(bool Ok, string Message)> ClaimStreakBonusAsync()
        {
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token)) return (false, Loc.Get("Common_TryAgain"));

            try
            {
                var resp = await _api.ClaimStreakBonusAsync($"Bearer {token}");
                if (resp.IsSuccessStatusCode && resp.Content is not null)
                {
                    await LoadAsync();
                    return (true, Loc.Get("Wallet_StreakClaimed"));
                }

                return (false, Modules.Users.Infrastructure.Api.ApiProblem.GetDetail(
                    resp, Loc.Get("Wallet_StreakUnavailable")));
            }
            catch
            {
                return (false, Loc.Get("Common_TryAgain"));
            }
        }
    }

    /// <summary>Строка истории кошелька: сумма (+/−), тип, дата.</summary>
    public class WalletTxItem
    {
        public int Amount { get; set; }
        public WalletTransactionType Type { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public string TypeLabel => Loc.Get($"Wallet_TxType_{Type}");
        public string AmountLabel => (Amount >= 0 ? "+" : "") + $"{Amount} FC";
        public Color AmountColor => Amount >= 0 ? Color.FromArgb("#16A34A") : Color.FromArgb("#EF4444");

        public string Emoji => Type switch
        {
            WalletTransactionType.StreakBonus => "🔥",
            WalletTransactionType.ChallengeReward => "🏆",
            WalletTransactionType.ReferralBonus => "👥",
            WalletTransactionType.TrainingPayment => "🏋️",
            WalletTransactionType.TrainingPayout => "💸",
            WalletTransactionType.Refund => "↩️",
            _ => "💰",
        };

        public string DateLabel =>
            CreatedAtUtc.ToLocalTime().ToString("d MMM yyyy", new CultureInfo(Loc.Get("_Culture")));

        public string SubLabel => string.IsNullOrWhiteSpace(Description)
            ? DateLabel
            : $"{Description} · {DateLabel}";

        public static WalletTxItem FromDto(WalletTransactionDto d) => new()
        {
            Amount = d.Amount,
            Type = d.Type,
            Description = d.Description,
            CreatedAtUtc = d.CreatedAtUtc,
        };
    }
}

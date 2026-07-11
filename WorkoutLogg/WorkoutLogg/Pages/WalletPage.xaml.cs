using WorkoutLogg.Localization;
using WorkoutLogg.PageModels;

namespace WorkoutLogg.Pages;

public partial class WalletPage : ContentPage
{
    private readonly WalletPageModel _vm;

    public WalletPage(WalletPageModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
        PageLoading.Preload();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        PageLoading.Show();
        try
        {
            await _vm.LoadAsync();
        }
        finally
        {
            PageLoading.Hide();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        PageLoading.Preload();
    }

    private async void OnBackTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("..");

    private async void OnClaimStreakTapped(object sender, TappedEventArgs e)
    {
        PageLoading.Show();
        (bool ok, string message) result;
        try
        {
            result = await _vm.ClaimStreakBonusAsync();
        }
        finally
        {
            PageLoading.Hide();
        }

        await DisplayAlertAsync(
            result.ok ? Loc.Get("Wallet_StreakClaimedTitle") : Loc.Get("Wallet_StreakUnavailableTitle"),
            result.message,
            Loc.Get("Common_OK"));
    }
}

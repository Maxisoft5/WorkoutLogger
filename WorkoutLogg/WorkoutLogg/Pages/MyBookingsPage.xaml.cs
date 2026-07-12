using WorkoutLogg.Localization;
using WorkoutLogg.PageModels;

namespace WorkoutLogg.Pages;

public partial class MyBookingsPage : ContentPage
{
    private readonly MyBookingsPageModel _vm;

    public MyBookingsPage(MyBookingsPageModel vm)
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

    private async void OnCancelTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not BookingItem item) return;

        var confirmed = await DisplayAlertAsync(
            Loc.Get("Bookings_CancelConfirmTitle"),
            Loc.Get("Bookings_CancelConfirmMsg"),
            Loc.Get("Common_Yes"),
            Loc.Get("Common_No"));
        if (!confirmed) return;

        PageLoading.Show();
        (bool ok, string? error) result;
        try
        {
            result = await _vm.CancelAsync(item.Id);
        }
        finally
        {
            PageLoading.Hide();
        }

        if (!result.ok)
            await DisplayAlertAsync(Loc.Get("Common_Error"), result.error ?? Loc.Get("Common_TryAgain"), Loc.Get("Common_OK"));
    }
}

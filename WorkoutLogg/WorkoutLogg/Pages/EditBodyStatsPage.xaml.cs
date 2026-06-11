using WorkoutLogg.Localization;
using WorkoutLogg.Services;

namespace WorkoutLogg.Pages;

public partial class EditBodyStatsPage : ContentPage
{
    private readonly UserProfileService _profileService;

    public EditBodyStatsPage(UserProfileService profileService)
    {
        InitializeComponent();
        _profileService = profileService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var stats = await _profileService.GetCachedBodyStatsAsync();
        if (stats is not null)
        {
            if (stats.Kg > 0) WeightEntry.Text = stats.Kg.ToString();
            if (stats.Cm > 0) HeightEntry.Text = stats.Cm.ToString();
            if (stats.Fat > 0) BodyFatEntry.Text = stats.Fat.ToString("0.#");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        SaveButton.IsEnabled = false;
        try
        {
            var kg  = double.TryParse(WeightEntry.Text,  out var w) && w > 0 ? w  : (double?)null;
            var cm  = double.TryParse(HeightEntry.Text,  out var h) && h > 0 ? h  : (double?)null;
            var fat = double.TryParse(BodyFatEntry.Text, out var f) && f > 0 ? f  : (double?)null;

            var ok = await _profileService.UpdateBodyStatsAsync(kg, cm, fat);
            if (ok)
            {
                await AppShell.DisplayToastAsync(Loc.Get("EditStats_Saved"));
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlertAsync(Loc.Get("EditStats_Title"), Loc.Get("EditStats_Error"), Loc.Get("Common_OK"));
            }
        }
        finally
        {
            SaveButton.IsEnabled = true;
        }
    }

    private void OnBackTapped(object sender, EventArgs e) =>
        Shell.Current.GoToAsync("..");
}

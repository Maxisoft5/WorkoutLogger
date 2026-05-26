using WorkoutLogg.Localization;
using WorkoutLogg.PageModels;
using WorkoutLogg.Services;

namespace WorkoutLogg.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly ProfilePageModel _vm;
    private readonly LanguageService _lang;

    public ProfilePage(ProfilePageModel vm, LanguageService lang)
    {
        InitializeComponent();
        _vm = vm;
        _lang = lang;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }

    private async void OnNotificationsTapped(object sender, TappedEventArgs e) =>
        await DisplayAlert(Loc.Get("Profile_Notifications"), Loc.Get("Common_ComingSoon"), Loc.Get("Common_OK"));

    private async void OnLanguageTapped(object sender, TappedEventArgs e)
    {
        var current = _lang.CurrentCode;
        var options = new[]
        {
            Loc.Get("Language_English"),
            Loc.Get("Language_Russian"),
            Loc.Get("Language_Auto"),
        };
        var codes = new[] { "en-US", "ru-RU", "auto" };

        var chosen = await DisplayActionSheet(
            Loc.Get("Language_Title"), Loc.Get("Common_Cancel"), null, options);

        if (chosen is null || chosen == Loc.Get("Common_Cancel")) return;

        var idx = Array.IndexOf(options, chosen);
        if (idx < 0) return;

        var newCode = codes[idx];
        if (newCode == current) return;

        await _lang.SetLanguageAsync(newCode);
    }

    private async void OnPrivacyTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("Privacy");
}
using WorkoutLogg.Localization;
using WorkoutLogg.PageModels;
using WorkoutLogg.Services;

namespace WorkoutLogg.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly ProfilePageModel _vm;
    private readonly LanguageService _lang;
    private readonly IAuthFlow _authFlow;
    private readonly UserProfileService _userService;

    public ProfilePage(ProfilePageModel vm, LanguageService lang, IAuthFlow authFlow, UserProfileService userService)
    {
        InitializeComponent();
        _vm = vm;
        _lang = lang;
        _authFlow = authFlow;
        _userService = userService;
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

    private async void OnNotificationsTapped(object sender, TappedEventArgs e) =>
        await DisplayAlertAsync(Loc.Get("Profile_Notifications"), Loc.Get("Common_ComingSoon"), Loc.Get("Common_OK"));

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

        var chosen = await DisplayActionSheetAsync(
            Loc.Get("Language_Title"), Loc.Get("Common_Cancel"), null, options);

        if (chosen is null || chosen == Loc.Get("Common_Cancel")) return;

        var idx = Array.IndexOf(options, chosen);
        if (idx < 0) return;

        var newCode = codes[idx];
        if (newCode == current) return;

        await _lang.SetLanguageAsync(newCode);
    }

    private async void OnChangeAvatarTapped(object sender, TappedEventArgs e)
    {
        var photo = await MediaPicker.PickPhotoAsync();
        if (photo is null) return;

        using var stream = await photo.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var base64 = Convert.ToBase64String(ms.ToArray());
        var dataUrl = $"data:image/jpeg;base64,{base64}";

        var ok = await _vm.UpdateProfilePictureAsync(dataUrl);
        if (!ok)
            await DisplayAlertAsync(Loc.Get("Common_Error"), Loc.Get("Common_TryAgain"), Loc.Get("Common_OK"));
    }

    private async void OnEditStatsTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("EditBodyStats");

    private async void OnPremiumTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("Premium");

    private async void OnStandardsTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("Standards");

    private async void OnLogoutTapped(object sender, TappedEventArgs e)
    {
        var confirmed = await DisplayAlertAsync(
            Loc.Get("Profile_Logout"),
            Loc.Get("Profile_LogoutConfirm"),
            Loc.Get("Common_Yes"),
            Loc.Get("Common_Cancel"));

        if (!confirmed) return;

        _userService.ClearCache();
        CurrentUserStore.Clear();
        _authFlow.SignOutAndRedirectToLogin();
    }

    private async void OnPrivacyTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("Privacy");
}
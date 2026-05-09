namespace WorkoutLogg.Pages;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    private async void OnNotificationsTapped(object sender, TappedEventArgs e)
    {
        // TODO: NavigationPage Notifications settings
        await DisplayAlert("Notifications", "Coming soon", "OK");
    }

    private async void OnUnitsTapped(object sender, TappedEventArgs e)
    {
        // TODO: Units picker (kg / lbs)
        await DisplayAlert("Units", "Coming soon", "OK");
    }

    private async void OnLanguageTapped(object sender, TappedEventArgs e)
    {
        // TODO: Language picker
        await DisplayAlert("Language", "Coming soon", "OK");
    }

    private async void OnPrivacyTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("Privacy");
    }
}
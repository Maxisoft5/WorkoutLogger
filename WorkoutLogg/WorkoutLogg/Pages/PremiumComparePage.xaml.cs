namespace WorkoutLogg.Pages;

public partial class PremiumComparePage : ContentPage
{
    public PremiumComparePage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("..");

    private async void OnUpgradeTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("Payment?plan=annual");
}

namespace WorkoutLogg.Pages;

public partial class ForgotPassword : ContentPage
{
    public ForgotPassword()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        Application.Current!.Windows[0].Page = new LoginPage();
    }

    private async void OnSendResetCodeClicked(object sender, EventArgs e)
    {
        // TODO: await _authService.SendResetCodeAsync(email);

        // Передаём email на следующий экран через query parameter
        //await Shell.Current.GoToAsync($"EnterCode?email={Uri.EscapeDataString(email ?? "")}");
        Application.Current!.Windows[0].Page = new EnterPasswordCode() { Email = "maxisoft4@gmail.com" };
    }
}
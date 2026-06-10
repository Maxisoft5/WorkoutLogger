using Modules.Users.Infrastructure.Api;
using Modules.Users.DTO.Auth;

namespace WorkoutLogg.Pages;

public partial class ForgotPassword : ContentPage
{
    private readonly IAuthApi _authApi;

    public ForgotPassword()
    {
        InitializeComponent();
        _authApi = Application.Current!.Handler.MauiContext!.Services.GetRequiredService<IAuthApi>();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        Application.Current!.Windows[0].Page = new LoginPage();
    }

    private async void OnSendResetCodeClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlertAsync("Error", "Please enter your email", "OK");
            return;
        }

        try
        {
            await _authApi.ForgotPassword(new ForgotPasswordRequest(email));
            Application.Current!.Windows[0].Page = new EnterPasswordCode() { Email = email };
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }
}

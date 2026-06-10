using Modules.Users.DTO.Auth;
using Modules.Users.Infrastructure.Api;

namespace WorkoutLogg.Pages;

[QueryProperty(nameof(Email), "email")]
public partial class NewPasswordPage : ContentPage
{
    public string Email { get; set; } = "";
    public string Code { get; set; } = "";

    private readonly IAuthApi _authApi;

    public NewPasswordPage()
    {
        InitializeComponent();
        _authApi = Application.Current!.Handler.MauiContext!.Services.GetRequiredService<IAuthApi>();
    }
    private void OnToggleNewPasswordVisibility(object sender, EventArgs e)
    {
        NewPasswordEntry.IsPassword = !NewPasswordEntry.IsPassword;
        NewPasswordEyeImage.Source = new FontImageSource
        {
            Glyph = NewPasswordEntry.IsPassword ? FluentUI.eye_20_regular : FluentUI.eye_off_20_regular,
            FontFamily = FluentUI.FontFamily,
            Color = Color.FromArgb("#9CA3AF"),
            Size = 20
        };
    }

    private void OnToggleConfirmPasswordVisibility(object sender, EventArgs e)
    {
        ConfirmPasswordEntry.IsPassword = !ConfirmPasswordEntry.IsPassword;
        ConfirmPasswordEyeImage.Source = new FontImageSource
        {
            Glyph = ConfirmPasswordEntry.IsPassword ? FluentUI.eye_20_regular : FluentUI.eye_off_20_regular,
            FontFamily = FluentUI.FontFamily,
            Color = Color.FromArgb("#9CA3AF"),
            Size = 20
        };
    }

    // ── Password strength ─────────────────────────
    private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
    {
        var pw = e.NewTextValue ?? "";
        var (level, label, color) = GetStrength(pw);

        StrengthLabel.Text = label;
        StrengthLabel.TextColor = Color.FromArgb(color);

        var bars = new[] { Bar0, Bar1, Bar2 };
        var active = Color.FromArgb(color);
        var inactive = Color.FromArgb("#E5E7EB");

        for (int i = 0; i < 3; i++)
            bars[i].Color = i < level ? active : inactive;
    }

    private static (int level, string label, string hex) GetStrength(string pw)
    {
        if (pw.Length == 0) return (0, "", "#E5E7EB");
        if (pw.Length < 6) return (1, "Weak", "#EF4444");

        bool hasUpper = pw.Any(char.IsUpper);
        bool hasDigit = pw.Any(char.IsDigit);
        bool hasSymbol = pw.Any(c => !char.IsLetterOrDigit(c));
        int score = (hasUpper ? 1 : 0) + (hasDigit ? 1 : 0) + (hasSymbol ? 1 : 0);

        return score switch
        {
            0 => (1, "Weak", "#EF4444"),
            1 => (2, "Fair", "#F59E0B"),
            _ => (3, "Strong", "#22C55E"),
        };
    }

    // ── Reset ─────────────────────────────────────
    private async void OnResetPasswordClicked(object sender, EventArgs e)
    {
        var pw = NewPasswordEntry.Text;
        var confirm = ConfirmPasswordEntry.Text;

        if (pw != confirm)
        {
            await DisplayAlertAsync("Error", "Passwords do not match", "OK");
            return;
        }

        try
        {
            var result = await _authApi.ResetPassword(new ResetPasswordRequest(Email, Code, pw));
            if (result.IsSuccessStatusCode)
                Application.Current!.Windows[0].Page = new PasswordSuccess();
            else
                await DisplayAlertAsync("Error", "Failed to reset password", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        Application.Current!.Windows[0].Page = new LoginPage();
    }
}
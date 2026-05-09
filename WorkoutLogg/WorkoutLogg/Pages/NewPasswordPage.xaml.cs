namespace WorkoutLogg.Pages;

[QueryProperty(nameof(Email), "email")]
public partial class NewPasswordPage : ContentPage
{
    public string Email { get; set; } = "";
    public NewPasswordPage()
	{
		InitializeComponent();
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
            await DisplayAlert("Error", "Passwords do not match", "OK");
            return;
        }

        // TODO: await _authService.ResetPasswordAsync(Email, pw);
        Application.Current!.Windows[0].Page = new PasswordSuccess();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        Application.Current!.Windows[0].Page = new LoginPage();
    }
}
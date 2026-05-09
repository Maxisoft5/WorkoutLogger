namespace WorkoutLogg.Pages;

[QueryProperty(nameof(Email), "email")]
public partial class EnterPasswordCode : ContentPage
{
    private string _email = "";
    private int _resendSeconds = 28;
    private IDispatcherTimer? _timer;

    private readonly Border[] _boxes;
    private readonly Label[] _labels;
    private readonly string[] _dots = { "·", "·", "·", "·", "·", "·" };

    public string Email
    {
        get => _email;
        set
        {
            _email = Uri.UnescapeDataString(value ?? "");
            SubtitleLabel.Text = $"We sent a 6-digit code to {_email}";
        }
    }

    public EnterPasswordCode()
    {
        InitializeComponent();

        _boxes = new[] { Box0, Box1, Box2, Box3, Box4, Box5 };
        _labels = new[] { Lbl0, Lbl1, Lbl2, Lbl3, Lbl4, Lbl5 };

        StartResendTimer();

        // Показываем клавиатуру сразу
        Loaded += (_, _) =>
        {
            HiddenEntry.IsVisible = true;
            HiddenEntry.Focus();
        };
    }

    // ── OTP input ─────────────────────────────────
    private void OnOtpAreaTapped(object sender, TappedEventArgs e) => HiddenEntry.Focus();

    private void OnCodeTextChanged(object sender, TextChangedEventArgs e)
    {
        var text = e.NewTextValue ?? "";
        for (int i = 0; i < 6; i++)
        {
            if (i < text.Length)
            {
                _labels[i].Text = text[i].ToString();
                _labels[i].TextColor = Color.FromArgb("#7C3AED");
                _boxes[i].Stroke = Color.FromArgb("#7C3AED");
                _boxes[i].StrokeThickness = 2;
            }
            else
            {
                _labels[i].Text = "·";
                _labels[i].TextColor = Color.FromArgb("#D1D5DB");
                _boxes[i].Stroke = Color.FromArgb("#E5E7EB");
                _boxes[i].StrokeThickness = 1.5;
            }
        }
    }

    // ── Resend countdown ──────────────────────────
    private void StartResendTimer()
    {
        _resendSeconds = 28;
        ResendLabel.Text = $"Resend ({_resendSeconds}s)";
        ResendLabel.TextColor = Color.FromArgb("#9CA3AF");

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (_, _) =>
        {
            _resendSeconds--;
            if (_resendSeconds <= 0)
            {
                _timer.Stop();
                ResendLabel.Text = "Resend";
                ResendLabel.TextColor = Color.FromArgb("#7C3AED");
            }
            else
            {
                ResendLabel.Text = $"Resend ({_resendSeconds}s)";
            }
        };
        _timer.Start();
    }

    private void OnResendTapped(object sender, EventArgs e)
    {
        if (_resendSeconds > 0) return;
        // TODO: await _authService.ResendCodeAsync(_email);
        StartResendTimer();
    }

    // ── Verify ────────────────────────────────────
    private async void OnVerifyCodeClicked(object sender, EventArgs e)
    {
        var code = HiddenEntry.Text?.Trim();
        if (code?.Length != 6) return;

        // TODO: await _authService.VerifyCodeAsync(_email, code);
        Application.Current!.Windows[0].Page = new NewPasswordPage() { Email = Email };
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        _timer?.Stop();
        Application.Current!.Windows[0].Page = new LoginPage();
    }
}
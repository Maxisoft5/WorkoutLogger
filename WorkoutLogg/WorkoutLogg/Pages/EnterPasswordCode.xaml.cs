using Modules.Users.Infrastructure.Api;
using Modules.Users.DTO.Auth;
using WorkoutLogg.Localization;

namespace WorkoutLogg.Pages;

[QueryProperty(nameof(Email), "email")]
public partial class EnterPasswordCode : ContentPage
{
    private string _email = "";
    private int _resendSeconds = 28;
    private IDispatcherTimer? _timer;

    private readonly Border[] _boxes;
    private readonly Label[] _labels;

    public string Email
    {
        get => _email;
        set
        {
            _email = Uri.UnescapeDataString(value ?? "");
            SubtitleLabel.Text = $"We sent a 6-digit code to {_email}";
        }
    }

    private readonly IAuthApi _authApi;

    public EnterPasswordCode()
    {
        InitializeComponent();
        _authApi = Application.Current!.Handler.MauiContext!.Services.GetRequiredService<IAuthApi>();

        _boxes = new[] { Box0, Box1, Box2, Box3, Box4, Box5 };
        _labels = new[] { Lbl0, Lbl1, Lbl2, Lbl3, Lbl4, Lbl5 };

        StartResendTimer();

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
        UpdateResendLabel();

        _timer?.Stop();
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (_, _) =>
        {
            _resendSeconds--;
            UpdateResendLabel();
            if (_resendSeconds <= 0)
                _timer!.Stop();
        };
        _timer.Start();
    }

    private void UpdateResendLabel()
    {
        var resendText = Loc.Get("EnterCode_Resend");
        if (_resendSeconds > 0)
        {
            ResendLabel.Text = $"{resendText} ({_resendSeconds}s)";
            ResendLabel.TextColor = Color.FromArgb("#9CA3AF");
        }
        else
        {
            ResendLabel.Text = resendText;
            ResendLabel.TextColor = Color.FromArgb("#7C3AED");
        }
    }

    private async void OnResendTapped(object sender, EventArgs e)
    {
        if (_resendSeconds > 0) return;

        try
        {
            await _authApi.ForgotPassword(new ForgotPasswordRequest(_email));
            StartResendTimer();
            await DisplayAlertAsync("", Loc.Get("EnterCode_ResendSuccess"), "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    // ── Verify ────────────────────────────────────
    private async void OnVerifyCodeClicked(object sender, EventArgs e)
    {
        var code = HiddenEntry.Text?.Trim();
        if (code?.Length != 6) return;

        try
        {
            var response = await _authApi.VerifyResetCode(new VerifyResetCodeRequest(_email, code));
            if (response.IsSuccessStatusCode)
                Application.Current!.Windows[0].Page = new NewPasswordPage { Email = _email, Code = code };
            else
            {
                var error = ApiProblem.GetDetail(response, "Invalid or expired code");
                await DisplayAlertAsync("Error", error, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private void OnBackTapped(object sender, EventArgs e)
    {
        _timer?.Stop();
        Application.Current!.Windows[0].Page = new LoginPage();
    }
}

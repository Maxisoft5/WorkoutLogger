using WorkoutLogg.Localization;
using WorkoutLogg.Services;

namespace WorkoutLogg.Pages;

public partial class PremiumPage : ContentPage
{
    private readonly ISubscriptionsApi _api;
    private bool _isAnnual = true;

    private static readonly Color Purple = Color.FromArgb("#7C3AED");
    private static readonly Color Gray   = Color.FromArgb("#E5E7EB");

    public PremiumPage(ISubscriptionsApi api)
    {
        InitializeComponent();
        _api = api;
        UpdateFootnote();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadStatusAsync();
    }

    private async Task LoadStatusAsync()
    {
        try
        {
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token)) return;
            var resp = await _api.GetStatusAsync($"Bearer {token}");
            if (resp.IsSuccessStatusCode && resp.Content is not null)
                ApplySubscriptionUI(resp.Content);
        }
        catch { }
    }

    private void ApplySubscriptionUI(SubscriptionStatusResponse status)
    {
        var isActive = status.IsActive;

        SubscriptionActiveSection.IsVisible = isActive;
        PurchaseSection.IsVisible           = !isActive;
        CtaLabel.Text                       = isActive ? Loc.Get("Premium_CTA_Renew") : Loc.Get("Premium_CTA_Trial");

        if (!isActive)
        {
            UpdateFootnote();
            return;
        }

        FootnoteLabel.Text = string.Empty;

        // Badge: trial vs active
        var isTrial = status.TrialEndsAt.HasValue && status.TrialEndsAt.Value > DateTime.UtcNow;
        SubStatusBadge.Text = isTrial
            ? $"⭐ {Loc.Get("Premium_Active_Trial").ToUpperInvariant()}"
            : $"⭐ {Loc.Get("Premium_Status_Active").ToUpperInvariant()}";

        // Plan label
        SubPlanLabel.Text = string.Equals(status.Plan, "Annual", StringComparison.OrdinalIgnoreCase)
            ? Loc.Get("Premium_Plan_Annual")
            : Loc.Get("Premium_Plan_Monthly");

        // Expiry + days remaining
        if (status.ExpiresAt.HasValue)
        {
            SubExpiryLabel.Text = status.ExpiresAt.Value.ToLocalTime().ToString("dd.MM.yyyy");
            var daysLeft = (int)(status.ExpiresAt.Value - DateTime.UtcNow).TotalDays;
            SubDaysLeftLabel.Text = daysLeft > 0
                ? string.Format(Loc.Get("Premium_Active_DaysLeft"), daysLeft)
                : Loc.Get("Premium_Expired");
            SubDaysLeftLabel.TextColor = daysLeft > 7
                ? Color.FromArgb("#16A34A")
                : Color.FromArgb("#DC2626");
        }
        else
        {
            SubExpiryLabel.Text   = "—";
            SubDaysLeftLabel.Text = string.Empty;
        }
    }

    private async void OnCloseTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("..");

    private void OnRestoreTapped(object sender, TappedEventArgs e)
    {
        // TODO: restore purchase
    }

    private void OnAnnualTapped(object sender, TappedEventArgs e)
    {
        if (_isAnnual) return;
        _isAnnual = true;
        UpdatePlanUI();
    }

    private void OnMonthlyTapped(object sender, TappedEventArgs e)
    {
        if (!_isAnnual) return;
        _isAnnual = false;
        UpdatePlanUI();
    }

    private void UpdatePlanUI()
    {
        AnnualBorder.BackgroundColor  = _isAnnual ? Color.FromArgb("#EDE9FE") : Colors.White;
        AnnualBorder.Stroke           = new SolidColorBrush(_isAnnual ? Purple : Gray);
        AnnualRadio.BackgroundColor   = _isAnnual ? Purple : Colors.Transparent;
        AnnualRadio.Stroke            = new SolidColorBrush(_isAnnual ? Purple : Gray);
        AnnualRadio.StrokeThickness   = _isAnnual ? 0 : 2;
        AnnualCheck.IsVisible         = _isAnnual;

        MonthlyBorder.BackgroundColor = !_isAnnual ? Color.FromArgb("#EDE9FE") : Colors.White;
        MonthlyBorder.Stroke          = new SolidColorBrush(!_isAnnual ? Purple : Gray);
        MonthlyRadio.BackgroundColor  = !_isAnnual ? Purple : Colors.Transparent;
        MonthlyRadio.Stroke           = new SolidColorBrush(!_isAnnual ? Purple : Gray);
        MonthlyRadio.StrokeThickness  = !_isAnnual ? 0 : 2;
        MonthlyCheck.IsVisible        = !_isAnnual;

        UpdateFootnote();
    }

    private void UpdateFootnote()
    {
        FootnoteLabel.Text = _isAnnual
            ? Loc.Get("Premium_Footnote_Annual")
            : Loc.Get("Premium_Footnote_Monthly");
    }

    private async void OnCtaTapped(object sender, TappedEventArgs e)
    {
        var plan = _isAnnual ? "annual" : "monthly";
        await Shell.Current.GoToAsync($"Payment?plan={plan}");
    }

    private async void OnCompareTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("PremiumCompare");
}

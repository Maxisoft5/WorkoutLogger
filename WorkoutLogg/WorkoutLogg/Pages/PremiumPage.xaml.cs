using WorkoutLogg.Localization;

namespace WorkoutLogg.Pages;

public partial class PremiumPage : ContentPage
{
    private bool _isAnnual = true;

    private static readonly Color Purple = Color.FromArgb("#7C3AED");
    private static readonly Color Gray   = Color.FromArgb("#E5E7EB");

    public PremiumPage()
    {
        InitializeComponent();
        UpdateFootnote();
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

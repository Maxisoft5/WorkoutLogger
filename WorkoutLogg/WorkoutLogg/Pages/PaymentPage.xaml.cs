using System.Globalization;
using WorkoutLogg.Localization;
using WorkoutLogg.Services;

namespace WorkoutLogg.Pages;

[QueryProperty(nameof(Plan), "plan")]
public partial class PaymentPage : ContentPage
{
    private readonly ISubscriptionsApi _api;
    private readonly LanguageService _lang;
    private readonly Services.AppConfiguration _appConfig;

    private string _plan = "annual";
    private bool _isRu;

    // RU method selection: sbp | sber | tpay | card
    private string _ruMethod = "sbp";
    // EN method selection: apple | google | card
    private string _enMethod = "apple";

    public string Plan
    {
        get => _plan;
        set
        {
            _plan = value;
            UpdateOrderSummary();
        }
    }

    public PaymentPage(ISubscriptionsApi api, LanguageService lang, Services.AppConfiguration appConfig)
    {
        InitializeComponent();
        _api = api;
        _lang = lang;
        _appConfig = appConfig;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _isRu = IsRussianLocale();
        RuMethods.IsVisible = _isRu;
        EnMethods.IsVisible = !_isRu;
        RegionLabel.Text = _isRu ? Loc.Get("Payment_Region_RU") : Loc.Get("Payment_Region_EN");
        TestModeBanner.IsVisible = _appConfig.TestMode;
        UpdateOrderSummary();
    }

    private bool IsRussianLocale()
    {
        var code = _lang.CurrentCode;
        if (code == "ru-RU" || code == "ru") return true;
        if (code == "en-US" || code == "en") return false;
        return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ru";
    }

    private void UpdateOrderSummary()
    {
        var isAnnual = _plan == "annual";
        PlanNameLabel.Text = isAnnual ? Loc.Get("Payment_Plan_Annual") : Loc.Get("Payment_Plan_Monthly");

        if (_isRu)
        {
            PriceLabel.Text = isAnnual ? "249 ₽/мес" : "399 ₽/мес";
            DueTodayLabel.Text = Loc.Get("Payment_DueToday");
            BilledAfterLabel.Text = isAnnual
                ? Loc.Get("Payment_BilledAnnually")
                : Loc.Get("Payment_BilledMonthly");
        }
        else
        {
            PriceLabel.Text = isAnnual ? "$3.33/mo" : "$5.99/mo";
            DueTodayLabel.Text = Loc.Get("Payment_DueToday_EN");
            BilledAfterLabel.Text = isAnnual
                ? Loc.Get("Payment_BilledAnnually_EN")
                : Loc.Get("Payment_BilledMonthly_EN");
        }
    }

    // RU method selection
    private void OnSbpTapped(object sender, TappedEventArgs e) => SelectRuMethod("sbp");
    private void OnSberTapped(object sender, TappedEventArgs e) => SelectRuMethod("sber");
    private void OnTpayTapped(object sender, TappedEventArgs e) => SelectRuMethod("tpay");
    private void OnCardRuTapped(object sender, TappedEventArgs e) => SelectRuMethod("card");

    private void SelectRuMethod(string method)
    {
        _ruMethod = method;
        var purple = Color.FromArgb("#7C3AED");
        var gray = Color.FromArgb("#E5E7EB");

        SetMethodStyle(SbpBorder, SbpCheck, method == "sbp", purple, gray, isFirstRu: true);
        SetMethodStyle(SberBorder, SberCheck, method == "sber", purple, gray);
        SetMethodStyle(TpayBorder, TpayCheck, method == "tpay", purple, gray);
        SetMethodStyle(CardRuBorder, CardRuCheck, method == "card", purple, gray);
    }

    // EN method selection
    private void OnAppleTapped(object sender, TappedEventArgs e) => SelectEnMethod("apple");
    private void OnGoogleTapped(object sender, TappedEventArgs e) => SelectEnMethod("google");
    private void OnCardEnTapped(object sender, TappedEventArgs e) => SelectEnMethod("card");

    private void SelectEnMethod(string method)
    {
        _enMethod = method;
        var purple = Color.FromArgb("#7C3AED");
        var gray = Color.FromArgb("#E5E7EB");

        SetMethodStyle(AppleBorder, AppleCheck, method == "apple", purple, gray);
        SetMethodStyle(GoogleBorder, GoogleCheck, method == "google", purple, gray);
        SetMethodStyle(CardEnBorder, CardEnCheck, method == "card", purple, gray);
    }

    private static void SetMethodStyle(
        Border border, Label check, bool selected,
        Color purple, Color gray, bool isFirstRu = false)
    {
        border.BackgroundColor = selected ? Color.FromArgb("#EDE9FE") : Colors.White;
        border.Stroke = new SolidColorBrush(selected ? purple : gray);
        border.StrokeThickness = selected ? 2 : 1;
        // For СБП, the check is always shown (it has the RECOMMENDED badge); for others toggle
        if (!isFirstRu) check.IsVisible = selected;
    }

    private async void OnBackTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("..");

    private void OnRestoreTapped(object sender, TappedEventArgs e)
    {
        // TODO: restore purchase flow
    }

    private async void OnPayTapped(object sender, TappedEventArgs e)
    {
        var token = await LoginService.GetActiveToken();
        if (string.IsNullOrEmpty(token))
        {
            await DisplayAlert(Loc.Get("Common_Error"), Loc.Get("Common_TryAgain"), Loc.Get("Common_OK"));
            return;
        }

        var locale = _isRu ? "ru-RU" : "en-US";

        try
        {
            var resp = await _api.CheckoutAsync($"Bearer {token}",
                new SubscriptionCheckoutRequest(_plan, locale));

            if (!resp.IsSuccessStatusCode || resp.Content is null)
            {
                await DisplayAlert(Loc.Get("Common_Error"), Loc.Get("Payment_Error"), Loc.Get("Common_OK"));
                return;
            }

            if (resp.Content.Activated)
            {
                await DisplayAlert(Loc.Get("Payment_Success_Title"), Loc.Get("Payment_Success_Body"), Loc.Get("Common_OK"));
                await Shell.Current.GoToAsync("//main");
                return;
            }

            if (resp.Content.CheckoutUrl is null)
            {
                await DisplayAlert(Loc.Get("Common_Error"), Loc.Get("Payment_Error"), Loc.Get("Common_OK"));
                return;
            }

            await Launcher.OpenAsync(new Uri(resp.Content.CheckoutUrl));
        }
        catch
        {
            await DisplayAlert(Loc.Get("Common_Error"), Loc.Get("Payment_Error"), Loc.Get("Common_OK"));
        }
    }
}

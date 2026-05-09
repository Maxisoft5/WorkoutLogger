using Modules.Users.DTO.Auth;
using Modules.Users.Infrastructure.Api;

namespace WorkoutLogg.Pages;

public partial class OnboardingBodyStatsPage : ContentPage
{
    private double _weight = 82;
    private double _height = 183;
    private bool _isMetric = true;
    public IAuthApi AuthApi { get; set; }

    public OnboardingBodyStatsPage()
    {
        InitializeComponent();
        AuthApi = Application.Current!.Handler.MauiContext!.Services
          .GetRequiredService<IAuthApi>();
    }

    // ── Weight ─────────────────────────────────────
    private void OnWeightUpTapped(object sender, EventArgs e)
    {
        _weight += _isMetric ? 1 : 1;
        WeightLabel.Text = _weight.ToString("0");
    }

    private void OnWeightDownTapped(object sender, EventArgs e)
    {
        if (_weight > 1) _weight -= 1;
        WeightLabel.Text = _weight.ToString("0");
    }

    // ── Height ─────────────────────────────────────
    private void OnHeightUpTapped(object sender, EventArgs e)
    {
        _height += 1;
        HeightLabel.Text = _height.ToString("0");
    }

    private void OnHeightDownTapped(object sender, EventArgs e)
    {
        if (_height > 1) _height -= 1;
        HeightLabel.Text = _height.ToString("0");
    }

    // ── Unit toggle ────────────────────────────────
    private void OnMetricSelected(object sender, EventArgs e)
    {
        if (_isMetric) return;
        _isMetric = true;

        // lbs → kg
        _weight = Math.Round(_weight / 2.20462, 1);
        // ft → cm  (stored as inches)
        _height = Math.Round(_height * 2.54, 0);

        WeightLabel.Text = _weight.ToString("0");
        HeightLabel.Text = _height.ToString("0");
        WeightUnitLabel.Text = "kg";
        HeightUnitLabel.Text = "cm";
    }

    private void OnImperialSelected(object sender, EventArgs e)
    {
        if (!_isMetric) return;
        _isMetric = false;

        // kg → lbs
        _weight = Math.Round(_weight * 2.20462, 1);
        // cm → inches
        _height = Math.Round(_height / 2.54, 0);

        WeightLabel.Text = _weight.ToString("0");
        HeightLabel.Text = _height.ToString("0");
        WeightUnitLabel.Text = "lbs";
        HeightUnitLabel.Text = "in";
    }

    // ── Navigation ────────────────────────────────
    private async void OnBackClicked(object sender, EventArgs e)
    {
        Application.Current!.Windows[0].Page = new OnboardingProfilePage();
    }

    private async void OnContinueClicked(object sender, EventArgs e)
    {
        var user = new UserDto()
        {
            UserRegistrationStep = Modules.Users.DTO.Users.UserRegistrationStep.Goals,
            BodyStats = new UserBodyStatsDto()
            {
                Kg = (int)_weight,
                Cm = (int)_height,
                Fat = double.TryParse(BodyFatEntry.Text, out var fat) ? fat : 0,
            }
        };
        var activeToken = await LoginService.GetActiveToken();
        var upd = await AuthApi.UpdateAccount(activeToken, user);
        if (upd.IsSuccessStatusCode)
        {
            Application.Current!.Windows[0].Page = new OnboardingGoalsPage();
        }
    }
}
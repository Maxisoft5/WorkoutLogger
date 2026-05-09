using Modules.Users.DTO.Auth;
using Modules.Users.DTO.Users;
using Modules.Users.Infrastructure.Api;

namespace WorkoutLogg.Pages;

public partial class OnboardingGoalsPage : ContentPage
{
    // Выбранные цели (можно несколько)
    private readonly HashSet<string> _selectedGoals = new() { };
    private int _workoutsPerWeek = 4;

    // Словарь: ключ → (карточка Border, чекбокс Border)
    private Dictionary<string, (Border card, Label label, UserGoalVariant option)> _goalMap = null!;
    public IAuthApi AuthApi { get; set; }

    public OnboardingGoalsPage()
    {
        InitializeComponent();
        AuthApi = Application.Current!.Handler.MauiContext!.Services
           .GetRequiredService<IAuthApi>();
        _goalMap = new()
        {
            ["LoseFat"] = (LoseFatBorder, FatGoal, UserGoalVariant.LoseFat),
            ["BuildMuscle"] = (BuildMuscleBorder, BuildMuscleGoal, UserGoalVariant.BuildMuscle),
            ["Endurance"] = (EnduranceBorder, EnduranceGoal, UserGoalVariant.ImporveEndurance),
            ["Strength"] = (StrengthBorder, StrengthCheckGoal, UserGoalVariant.IncreaseStrength),
            ["Flexibility"] = (FlexibilityBorder, FlexibilityCheckGoal, UserGoalVariant.Flexibility),
            ["StayActive"] = (StayActiveBorder, StayActiveCheckGoal, UserGoalVariant.StayActive),
        };
    }

    // ── Goal toggle ───────────────────────────────
    private void OnGoalTapped(object sender, TappedEventArgs e)
    {
        var key = e.Parameter?.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(key)) return;

        if (_selectedGoals.Contains(key))
            _selectedGoals.Remove(key);
        else
            _selectedGoals.Add(key);

        RefreshGoalUI();
    }

    private void RefreshGoalUI()
    {
        var purple = Color.FromArgb("#7C3AED");
        var purpleBg = Color.FromArgb("#F3F0FF");
        var white = Colors.White;
        var grayBg = Color.FromArgb("#E5E7EB");
        var grayStroke = Color.FromArgb("#E5E7EB");

        foreach (var (key, (card, label, option)) in _goalMap)
        {
            bool active = _selectedGoals.Contains(key);
            label.Text = active ? "✓" : "";
            card.BackgroundColor = active ? purpleBg : white;
            card.Stroke = active ? purple : grayStroke;
            card.StrokeThickness = active ? 2 : 1.5;
        }
    }

    // ── Workout frequency ─────────────────────────
    private void OnWorkoutFreqTapped(object sender, EventArgs e)
    {
        if (sender is Button btn && int.TryParse(btn.CommandParameter?.ToString(), out int n))
        {
            _workoutsPerWeek = n;
            RefreshFrequencyUI();
        }
    }

    private void RefreshFrequencyUI()
    {
        var purple = Color.FromArgb("#7C3AED");
        var white = Colors.White;
        var gray = Color.FromArgb("#374151");
        var grayStroke = Color.FromArgb("#E5E7EB");

        var map = new Dictionary<int, (Border border, Button button)>
        {
            [2] = (W2Border, W2Button),
            [3] = (W3Border, W3Button),
            [4] = (W4Border, W4Button),
            [5] = (W5Border, W5Button),
            [6] = (W6Border, W6Button),
        };

        foreach (var (n, (border, button)) in map)
        {
            bool active = n == _workoutsPerWeek;
            border.BackgroundColor = active ? purple : white;
            border.Stroke = active ? purple : grayStroke;
            border.StrokeThickness = active ? 0 : 1.5;
            button.TextColor = active ? white : gray;
        }
    }

 

    // ── Navigation ────────────────────────────────
    private async void OnFinishClicked(object sender, EventArgs e)
    {
        // TODO: сохранить _selectedGoals и _workoutsPerWeek в SharedState / ViewModel
        // TODO: вызвать UserService.SaveOnboardingAsync(...)
        // Переход на главный экран приложения
        var userDto = new UserDto()
        {
            UserRegistrationStep = UserRegistrationStep.Finished,
            Goals = _selectedGoals.Select(s => new UserGoalDto()
            {
                Goal = _goalMap[s].option, Id = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            }).ToList(),
            WorkOutCount = GetCount()
        };
        var token = await LoginService.GetActiveToken();
        var upd = await AuthApi.UpdateAccount(token, userDto);
        if (upd.IsSuccessStatusCode)
        {
            Application.Current!.Windows[0].Page = new AppShell();
            await Shell.Current.GoToAsync("//Dashboard");
        }
    }

    private WorkOutCountVariant GetCount()
    {
        switch(_workoutsPerWeek)
        {
            case 1:
            {
                return WorkOutCountVariant.One;
            }
            case 2:
            {
                return WorkOutCountVariant.Two;
            }
            case 3:
            {
                return WorkOutCountVariant.Three;
            }
            case 4:
            {
                return WorkOutCountVariant.Four;
            }
            case 5:
            {
                return WorkOutCountVariant.Five;
            }
        }
        return WorkOutCountVariant.One;
    }
}
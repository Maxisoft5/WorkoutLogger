using WorkoutLogg.Utilities;
using Modules.Workouts.DTO.Enums;
using WorkoutLogg.Database;
using WorkoutLogg.Database.Entities;
using WorkoutLogg.Localization;
using WorkoutLogg.Services;

namespace WorkoutLogg.Pages;

[QueryProperty(nameof(DateParam), "date")]
[QueryProperty(nameof(SessionIdParam), "sessionId")]
public partial class AddLogPage : ContentPage, IQueryAttributable
{
    private readonly WorkoutDatabase _db;
    private readonly UserProfileService _profileService;

    private DateTime _date = DateTime.Today;
    private Guid _editingSessionId = Guid.Empty;
    private WorkoutEntity? _selectedWorkout;
    private readonly List<ExerciseLogFormRow> _exerciseRows = [];
    private List<WorkoutEntity> _availableWorkouts = [];

    public string? DateParam { get; set; }
    public string? SessionIdParam { get; set; }

    public AddLogPage(WorkoutDatabase db, UserProfileService profileService)
    {
        InitializeComponent();
        _db = db;
        _profileService = profileService;
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("sessionId", out var sid) && Guid.TryParse(sid?.ToString(), out var sessionId))
            _editingSessionId = sessionId;

        if (query.TryGetValue("date", out var d) && DateTime.TryParse(d?.ToString(), out var date))
            _date = date;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _availableWorkouts = await _db.GetWorkoutsAsync();

        if (_editingSessionId != Guid.Empty)
        {
            PageTitle.Text = Loc.Get("AddLog_EditTitle");
            var session = await _db.GetLogSessionWithExercisesAsync(_editingSessionId);
            if (session is null) { await Shell.Current.GoToAsync(".."); return; }

            _date = session.Date;

            if (session.WorkoutId != Guid.Empty)
            {
                _selectedWorkout = _availableWorkouts.FirstOrDefault(w => w.Id == session.WorkoutId);
                WorkoutSelectorLabel.Text = session.WorkoutLabel;
            }

            ExercisesList.Children.Clear();
            _exerciseRows.Clear();

            foreach (var ex in session.Exercises)
                AddExerciseRow(ex);
        }
    }

    private async void OnWorkoutSelectorTapped(object sender, TappedEventArgs e)
    {
        var cancel = Loc.Get("Common_Cancel");
        var customPlan = Loc.Get("AddLog_CustomPlan");
        var options = new List<string> { customPlan };
        foreach (var w in _availableWorkouts)
            options.Add($"{w.MuscleGroup} · {w.StartDate:d MMM yyyy}");

        var result = await DisplayActionSheetAsync(Loc.Get("AddLog_LinkToPlan"), cancel, null, [.. options]);

        if (result is null || result == cancel) return;

        if (result == customPlan)
        {
            _selectedWorkout = null;
            WorkoutSelectorLabel.Text = customPlan;
            ClearAndRebuildExercises(null);
        }
        else
        {
            var idx = options.IndexOf(result) - 1; // -1 for the "custom" option
            if (idx < 0 || idx >= _availableWorkouts.Count) return;

            _selectedWorkout = _availableWorkouts[idx];
            WorkoutSelectorLabel.Text = result;
            ClearAndRebuildExercises(_selectedWorkout);
        }
    }

    private void ClearAndRebuildExercises(WorkoutEntity? workout)
    {
        ExercisesList.Children.Clear();
        _exerciseRows.Clear();

        if (workout is not null)
            foreach (var ex in workout.Exercises)
                AddExerciseRow(null, ex);
    }

    private void AddExerciseRow(LogExerciseEntity? existing = null, WorkoutSetEntity? fromPlan = null)
    {
        var row = new ExerciseLogFormRow(existing, fromPlan);
        row.DeleteRequested += r =>
        {
            _exerciseRows.Remove(r);
            ExercisesList.Children.Remove(r.View);
        };
        _exerciseRows.Add(row);
        ExercisesList.Children.Add(row.View);
    }

    private async void OnAddExerciseTapped(object sender, TappedEventArgs e)
    {
        if (_selectedWorkout is not null && _selectedWorkout.Exercises.Count > 0)
        {
            var cancel = Loc.Get("Common_Cancel");
            var customExercise = Loc.Get("AddLog_CustomExercise");
            var planOptions = _selectedWorkout.Exercises
                .Select(ex => ex.ExerciseName)
                .ToList();
            planOptions.Add(customExercise);

            var pick = await DisplayActionSheetAsync(Loc.Get("AddLog_AddExercise"), cancel, null, [.. planOptions]);
            if (pick is null || pick == cancel) return;

            if (pick == customExercise)
                AddExerciseRow();
            else
            {
                var planEx = _selectedWorkout.Exercises.FirstOrDefault(ex => ex.ExerciseName == pick);
                AddExerciseRow(null, planEx);
            }
        }
        else
        {
            AddExerciseRow();
        }
    }

    private async void OnSaveClicked(object sender, TappedEventArgs e)
    {
        var exercises = _exerciseRows
            .Select(r => r.ToEntity())
            .Where(ex => !string.IsNullOrWhiteSpace(ex.ExerciseName))
            .ToList();

        if (_editingSessionId != Guid.Empty)
        {
            var session = await _db.GetLogSessionWithExercisesAsync(_editingSessionId);
            if (session is null) { await Shell.Current.GoToAsync(".."); return; }

            session.WorkoutId = _selectedWorkout?.Id ?? Guid.Empty;
            session.WorkoutLabel = WorkoutSelectorLabel.Text ?? "";
            session.IsCustom = _selectedWorkout is null;
            session.IsSynced = false;

            await _db.SaveLogSessionAsync(session);
            await _db.ReplaceExerciseLogsAsync(_editingSessionId, exercises);
        }
        else
        {
            var session = new WorkoutLogSessionEntity
            {
                Id = Guid.NewGuid(),
                Date = _date.Date,
                WorkoutId = _selectedWorkout?.Id ?? Guid.Empty,
                WorkoutLabel = WorkoutSelectorLabel.Text ?? "",
                IsCustom = _selectedWorkout is null,
                IsSynced = false,
            };
            await _db.SaveLogSessionAsync(session);
            await _db.ReplaceExerciseLogsAsync(session.Id, exercises);
        }

        // Weight update
        if (double.TryParse(WeightEntry.Text, out var weightKg) && weightKg > 0
            && UpdateWeightCheckBox.IsChecked)
        {
            bool confirmed = await DisplayAlertAsync(
                Loc.Get("AddLog_UpdateWeightTitle"),
                $"{Loc.Get("AddLog_UpdateWeightMsg")} {weightKg:0.#} {Loc.Get("Common_Kg")}?",
                Loc.Get("Common_Yes"),
                Loc.Get("Common_No"));

            if (confirmed)
                await _profileService.UpdateBodyStatsAsync(kg: weightKg);
        }

        await Shell.Current.GoToAsync("..");
    }

    private void OnUpdateWeightLabelTapped(object sender, TappedEventArgs e) =>
        UpdateWeightCheckBox.IsChecked = !UpdateWeightCheckBox.IsChecked;

    private void OnBackTapped(object sender, TappedEventArgs e) =>
        Shell.Current.GoToAsync("..");
}

// ─── ExerciseLogFormRow ───────────────────────────────────────────────────────

internal class ExerciseLogFormRow
{
    private readonly Entry _nameEntry;
    private readonly Picker _complexityPicker;
    private readonly List<SetLogFormRow> _setRows = [];
    private readonly VerticalStackLayout _setsContainer;
    private readonly Guid _workoutSetId;

    public View View { get; }
    public bool IsCustom { get; }
    public event Action<ExerciseLogFormRow>? DeleteRequested;

    public ExerciseLogFormRow(LogExerciseEntity? existing, WorkoutSetEntity? fromPlan)
    {
        IsCustom = fromPlan is null && existing?.IsCustom != false;
        _workoutSetId = fromPlan?.Id ?? existing?.WorkoutSetId ?? Guid.Empty;

        _nameEntry = new Entry
        {
            Placeholder = Loc.Get("AddLog_ExercisePlaceholder"),
            FontSize = 15,
            FontAttributes = FontAttributes.Bold,
            Text = existing?.ExerciseName ?? fromPlan?.ExerciseName ?? ""
        }.WithThemeColor("PlaceholderColor", Color.FromArgb("#9CA3AF")).WithThemeColor("TextColor", Color.FromArgb("#111827"));

        _complexityPicker = new Picker
        {
            Title = Loc.Get("AddWorkout_Complexity")
        }.WithThemeColor("TextColor", Color.FromArgb("#111827")).WithThemeColor("TitleColor", Color.FromArgb("#9CA3AF"));
        _complexityPicker.Items.Add(Loc.Get("AddWorkout_Complexity_Low"));
        _complexityPicker.Items.Add(Loc.Get("AddWorkout_Complexity_Middle"));
        _complexityPicker.Items.Add(Loc.Get("AddWorkout_Complexity_High"));
        _complexityPicker.SelectedIndex = (int)(existing?.Complexity ?? fromPlan?.ExerciseComplexity ?? ExerciseComplexity.Low);

        _setsContainer = new VerticalStackLayout { Spacing = 8 };

        if (existing?.Sets.Count > 0)
        {
            foreach (var s in existing.Sets.OrderBy(x => x.SetNumber))
            {
                var row = new SetLogFormRow(s, existing.Sets.IndexOf(s) + 1);
                row.DeleteRequested += RemoveSetRow;
                _setRows.Add(row);
                _setsContainer.Children.Add(row.View);
            }
        }
        else if (fromPlan?.Sets.Count > 0)
        {
            // Pre-fill sets from plan as a suggestion
            foreach (var s in fromPlan.Sets.OrderBy(x => x.SetNumber))
            {
                var logSet = new LogSetEntity
                {
                    SetNumber = s.SetNumber,
                    Reps = s.Reps,
                    WeightKg = s.WeightKg,
                    RestSeconds = s.RestSeconds,
                    IsWarmup = s.IsWarmup,
                };
                var row = new SetLogFormRow(logSet, s.SetNumber);
                row.DeleteRequested += RemoveSetRow;
                _setRows.Add(row);
                _setsContainer.Children.Add(row.View);
            }
        }

        var badge = BuildBadge();

        var addSetBtn = new Border
        {
            MinimumHeightRequest = 48,
            Padding = new Thickness(12, 8),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            StrokeThickness = 0,
            HorizontalOptions = LayoutOptions.Start,
            Content = new Label
            {
                Text = Loc.Get("AddWorkout_AddSet"),
                FontSize = 13,
                FontAttributes = FontAttributes.Bold
            }.WithThemeColor("TextColor", Color.FromArgb("#7C3AED"))
        }.WithThemeColor("BackgroundColor", Color.FromArgb("#EDE9FE"));
        addSetBtn.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(AddSet),
        });

        var deleteBtn = FitnessIcons.Action(FitnessIcons.Close, Loc.Get("Common_Delete"), Color.FromArgb("#EF4444"));
        deleteBtn.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => DeleteRequested?.Invoke(this)),
        });

        var header = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection(
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)),
        };
        header.Children.Add(_nameEntry);
        Grid.SetColumn(deleteBtn, 1);
        header.Children.Add(deleteBtn);

        View = new Border
        {
            Padding = new Thickness(16),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
            StrokeThickness = 1,
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children = { header, badge, _complexityPicker, _setsContainer, addSetBtn },
            }
        }.WithThemeColor("BackgroundColor", Colors.White).WithThemeColor("Stroke", Color.FromArgb("#F3F4F6"));
    }

    private Border BuildBadge()
    {
        var (bg, fg, text) = IsCustom
            ? ("#FEF3C7", "#D97706", Loc.Get("AddLog_Custom"))
            : ("#EDE9FE", "#7C3AED", Loc.Get("AddLog_FromPlan"));

        return new Border
        {
            Padding = new Thickness(8, 4),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
            StrokeThickness = 0,
            HorizontalOptions = LayoutOptions.Start,
            Content = new Label
            {
                Text = text,
                FontSize = 11,
                FontAttributes = FontAttributes.Bold
            }.WithThemeColor("TextColor", Color.FromArgb(fg))
        }.WithThemeColor("BackgroundColor", Color.FromArgb(bg));
    }

    private void AddSet()
    {
        var num = _setRows.Count + 1;
        var row = new SetLogFormRow(null, num);
        row.DeleteRequested += RemoveSetRow;
        _setRows.Add(row);
        _setsContainer.Children.Add(row.View);
        RenumberSets();
    }

    private void RemoveSetRow(SetLogFormRow row)
    {
        _setRows.Remove(row);
        _setsContainer.Children.Remove(row.View);
        RenumberSets();
    }

    private void RenumberSets()
    {
        for (int i = 0; i < _setRows.Count; i++)
            _setRows[i].UpdateNumber(i + 1);
    }

    public LogExerciseEntity ToEntity() => new()
    {
        Id = Guid.NewGuid(),
        WorkoutSetId = _workoutSetId,
        ExerciseName = _nameEntry.Text ?? "",
        IsCustom = IsCustom,
        Complexity = _complexityPicker.SelectedIndex switch
        {
            1 => ExerciseComplexity.Middle,
            2 => ExerciseComplexity.High,
            _ => ExerciseComplexity.Low,
        },
        Sets = _setRows.Select((r, i) => r.ToEntity(i + 1)).ToList(),
    };
}

// ─── SetLogFormRow ────────────────────────────────────────────────────────────

internal class SetLogFormRow
{
    private readonly Label _numberLabel;
    private readonly Label _warmupBadge;
    private bool _isWarmup;

    private readonly Entry _weightEntry;
    private readonly Entry _repsEntry;
    private readonly Entry _restEntry;
    private bool _useMinutes;
    private readonly Label _unitToggle;

    public View View { get; }
    public event Action<SetLogFormRow>? DeleteRequested;

    public SetLogFormRow(LogSetEntity? existing, int number)
    {
        _isWarmup = existing?.IsWarmup ?? false;

        _numberLabel = new Label
        {
            Text = $"{Loc.Get("Common_Set")} {number}",
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center,
            MinimumWidthRequest = 42
        }.WithThemeColor("TextColor", Color.FromArgb("#9CA3AF"));

        _warmupBadge = BuildWarmupBadge();

        _weightEntry = NumEntry(Loc.Get("Common_Kg"));
        _repsEntry = NumEntry(Loc.Get("Common_Reps"));
        _restEntry = NumEntry(Loc.Get("Common_Rest"));

        _useMinutes = false;
        _unitToggle = new Label
        {
            Text = Loc.Get("Common_Sec"),
            FontSize = 11,
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(4, 2),
            MinimumWidthRequest = 48,MinimumHeightRequest = 48,
            HorizontalTextAlignment = TextAlignment.Center,VerticalTextAlignment = TextAlignment.Center
        }.WithThemeColor("TextColor", Color.FromArgb("#7C3AED")).WithThemeColor("BackgroundColor", Colors.Transparent);
        _unitToggle.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(ToggleUnit),
        });

        var deleteBtn = FitnessIcons.Action(FitnessIcons.Close, Loc.Get("Common_Delete"), Color.FromArgb("#EF4444"));
        deleteBtn.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => DeleteRequested?.Invoke(this)),
        });

        if (existing is not null) Fill(existing);

        var actions = new Grid { ColumnDefinitions = new ColumnDefinitionCollection(new(GridLength.Star), new(GridLength.Auto), new(GridLength.Auto)) };
        actions.Add(_numberLabel);
        actions.Add(_warmupBadge, 1);
        actions.Add(deleteBtn, 2);
        var row = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection(
                new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                new ColumnDefinition(GridLength.Auto)),
            ColumnSpacing = 4,
            Padding = new Thickness(0, 4),
        };

        var kgX = new Label { Text = Loc.Get("Common_KgX"),FontSize = 12,VerticalOptions = LayoutOptions.Center }.WithThemeColor("TextColor", Color.FromArgb("#9CA3AF"));
        var clock = new Label { Text = FitnessIcons.Timer,FontFamily = FitnessIcons.FontFamily,FontSize = 18,VerticalOptions = LayoutOptions.Center }.WithThemeColor("TextColor", Color.FromArgb("#7C3AED"));

        Grid.SetColumn(_weightEntry, 0);
        Grid.SetColumn(kgX, 1);
        Grid.SetColumn(_repsEntry, 2);
        Grid.SetColumn(clock, 3);
        Grid.SetColumn(_restEntry, 4);
        Grid.SetColumn(_unitToggle, 5);

        row.Children.Add(_weightEntry);
        row.Children.Add(kgX);
        row.Children.Add(_repsEntry);
        row.Children.Add(clock);
        row.Children.Add(_restEntry);
        row.Children.Add(_unitToggle);

        View = new VerticalStackLayout
        {
            Spacing = 0,
            Children =
            {
                new BoxView { HeightRequest = 1}.WithThemeColor("Color", Color.FromArgb("#F3F4F6")),
                actions,
                row,
            },
        };
    }

    private Label BuildWarmupBadge()
    {
        var badge = FitnessIcons.Action(_isWarmup ? FitnessIcons.Fire : FitnessIcons.Circle, Loc.Get("Common_ToggleWarmup"));
        badge.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => ToggleWarmup(badge)),
        });
        return badge;
    }

    private void ToggleWarmup(Label badge)
    {
        _isWarmup = !_isWarmup;
        badge.Text = _isWarmup ? FitnessIcons.Fire : FitnessIcons.Circle;
    }

    private void ToggleUnit()
    {
        if (double.TryParse(_restEntry.Text, out var val))
        {
            _restEntry.Text = !_useMinutes
                ? (val / 60.0).ToString("0.##")
                : ((int)(val * 60)).ToString();
        }
        _useMinutes = !_useMinutes;
        _unitToggle.Text = _useMinutes ? Loc.Get("Common_Min") : Loc.Get("Common_Sec");
        _restEntry.Placeholder = _useMinutes ? Loc.Get("Common_Min") : Loc.Get("Common_Sec");
    }

    private void Fill(LogSetEntity s)
    {
        _weightEntry.Text = s.WeightKg > 0 ? s.WeightKg.ToString("0.##") : "";
        _repsEntry.Text = s.Reps > 0 ? s.Reps.ToString() : "";
        _restEntry.Text = s.RestSeconds > 0 ? s.RestSeconds.ToString() : "";
        _isWarmup = s.IsWarmup;
        _warmupBadge.Text = _isWarmup ? FitnessIcons.Fire : FitnessIcons.Circle;
    }

    public void UpdateNumber(int n) => _numberLabel.Text = $"{Loc.Get("Common_Set")} {n}";

    private static Entry NumEntry(string placeholder) => new Entry()
    {
        Placeholder = placeholder,
        Keyboard = Keyboard.Numeric,
        FontSize = 13,
        HorizontalTextAlignment = TextAlignment.Center    }.WithThemeColor("PlaceholderColor", Color.FromArgb("#C0C0C0")).WithThemeColor("TextColor", Color.FromArgb("#111827"));

    private int GetRestSeconds()
    {
        if (!double.TryParse(_restEntry.Text, out var val)) return 0;
        return _useMinutes ? (int)(val * 60) : (int)val;
    }

    public LogSetEntity ToEntity(int setNumber) => new()
    {
        Id = Guid.NewGuid(),
        SetNumber = setNumber,
        Reps = int.TryParse(_repsEntry.Text, out var r) ? r : 0,
        WeightKg = double.TryParse(_weightEntry.Text, out var w) ? w : 0,
        RestSeconds = GetRestSeconds(),
        IsWarmup = _isWarmup,
    };
}

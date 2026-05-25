using Moduels.Workouts.DTO.Enums;
using WorkoutLogg.Database;
using WorkoutLogg.Database.Entities;

namespace WorkoutLogg.Pages;

[QueryProperty(nameof(DateParam), "date")]
[QueryProperty(nameof(SessionIdParam), "sessionId")]
public partial class AddLogPage : ContentPage, IQueryAttributable
{
    private readonly WorkoutDatabase _db;

    private DateTime _date = DateTime.Today;
    private Guid _editingSessionId = Guid.Empty;
    private WorkoutEntity? _selectedWorkout;
    private readonly List<ExerciseLogFormRow> _exerciseRows = [];
    private List<WorkoutEntity> _availableWorkouts = [];

    public string? DateParam { get; set; }
    public string? SessionIdParam { get; set; }

    public AddLogPage(WorkoutDatabase db)
    {
        InitializeComponent();
        _db = db;
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
            PageTitle.Text = "Edit Log";
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
        var options = new List<string> { "— Custom (no plan)" };
        foreach (var w in _availableWorkouts)
            options.Add($"{w.MuscleGroup} · {w.StartDate:d MMM yyyy}");

        var result = await DisplayActionSheet("Link to workout plan?", "Cancel", null, [.. options]);

        if (result is null || result == "Cancel") return;

        if (result == "— Custom (no plan)")
        {
            _selectedWorkout = null;
            WorkoutSelectorLabel.Text = "— Custom (no plan)";
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
            var planOptions = _selectedWorkout.Exercises
                .Select(ex => ex.ExerciseName)
                .ToList();
            planOptions.Add("⚡ Custom exercise");

            var pick = await DisplayActionSheet("Add exercise", "Cancel", null, [.. planOptions]);
            if (pick is null || pick == "Cancel") return;

            if (pick == "⚡ Custom exercise")
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

        await Shell.Current.GoToAsync("..");
    }

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
            Placeholder = "Exercise name",
            PlaceholderColor = Color.FromArgb("#9CA3AF"),
            TextColor = Color.FromArgb("#111827"),
            FontSize = 15,
            FontAttributes = FontAttributes.Bold,
            Text = existing?.ExerciseName ?? fromPlan?.ExerciseName ?? "",
        };

        _complexityPicker = new Picker
        {
            Title = "Complexity",
            TextColor = Color.FromArgb("#111827"),
            TitleColor = Color.FromArgb("#9CA3AF"),
        };
        _complexityPicker.Items.Add("🟢  Low");
        _complexityPicker.Items.Add("🟡  Middle");
        _complexityPicker.Items.Add("🔴  High");
        _complexityPicker.SelectedIndex = (int)(existing?.Complexity ?? fromPlan?.ExerciesComplexity ?? ExerciesComplexity.Low);

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
            BackgroundColor = Color.FromArgb("#EDE9FE"),
            Padding = new Thickness(12, 8),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            StrokeThickness = 0,
            HorizontalOptions = LayoutOptions.Start,
            Content = new Label
            {
                Text = "+ Add Set",
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#7C3AED"),
            },
        };
        addSetBtn.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(AddSet),
        });

        var deleteBtn = new Label
        {
            Text = "✕",
            FontSize = 16,
            TextColor = Color.FromArgb("#EF4444"),
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
        };
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
            BackgroundColor = Colors.White,
            Padding = new Thickness(16),
            Stroke = Color.FromArgb("#F3F4F6"),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
            StrokeThickness = 1,
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children = { header, badge, _complexityPicker, _setsContainer, addSetBtn },
            },
        };
    }

    private Border BuildBadge()
    {
        var (bg, fg, text) = IsCustom
            ? ("#FEF3C7", "#D97706", "⚡ Custom")
            : ("#EDE9FE", "#7C3AED", "📋 From plan");

        return new Border
        {
            BackgroundColor = Color.FromArgb(bg),
            Padding = new Thickness(8, 4),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
            StrokeThickness = 0,
            HorizontalOptions = LayoutOptions.Start,
            Content = new Label
            {
                Text = text,
                FontSize = 11,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb(fg),
            },
        };
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
            1 => ExerciesComplexity.Middle,
            2 => ExerciesComplexity.High,
            _ => ExerciesComplexity.Low,
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
            Text = $"Set {number}",
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#9CA3AF"),
            VerticalOptions = LayoutOptions.Center,
            MinimumWidthRequest = 42,
        };

        _warmupBadge = BuildWarmupBadge();

        _weightEntry = NumEntry("kg");
        _repsEntry = NumEntry("reps");
        _restEntry = NumEntry("rest");

        _useMinutes = false;
        _unitToggle = new Label
        {
            Text = "sec",
            FontSize = 11,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#7C3AED"),
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(4, 2),
        };
        _unitToggle.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(ToggleUnit),
        });

        var deleteBtn = new Label
        {
            Text = "✕",
            FontSize = 13,
            TextColor = Color.FromArgb("#EF4444"),
            VerticalOptions = LayoutOptions.Center,
        };
        deleteBtn.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => DeleteRequested?.Invoke(this)),
        });

        if (existing is not null) Fill(existing);

        var row = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection(
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto)),
            ColumnSpacing = 4,
            Padding = new Thickness(0, 4),
        };

        var kgX = new Label { Text = "kg ×", FontSize = 12, TextColor = Color.FromArgb("#9CA3AF"), VerticalOptions = LayoutOptions.Center };
        var clock = new Label { Text = "⏱", FontSize = 13, VerticalOptions = LayoutOptions.Center };

        Grid.SetColumn(_numberLabel, 0);
        Grid.SetColumn(_warmupBadge, 1);
        Grid.SetColumn(_weightEntry, 2);
        Grid.SetColumn(kgX, 3);
        Grid.SetColumn(_repsEntry, 4);
        Grid.SetColumn(clock, 5);
        Grid.SetColumn(_restEntry, 6);
        Grid.SetColumn(_unitToggle, 7);
        Grid.SetColumn(deleteBtn, 8);

        row.Children.Add(_numberLabel);
        row.Children.Add(_warmupBadge);
        row.Children.Add(_weightEntry);
        row.Children.Add(kgX);
        row.Children.Add(_repsEntry);
        row.Children.Add(clock);
        row.Children.Add(_restEntry);
        row.Children.Add(_unitToggle);
        row.Children.Add(deleteBtn);

        View = new VerticalStackLayout
        {
            Spacing = 0,
            Children =
            {
                new BoxView { HeightRequest = 1, Color = Color.FromArgb("#F3F4F6") },
                row,
            },
        };
    }

    private Label BuildWarmupBadge()
    {
        var badge = new Label
        {
            Text = _isWarmup ? "🔥" : "○",
            FontSize = 16,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(4, 0),
        };
        badge.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => ToggleWarmup(badge)),
        });
        return badge;
    }

    private void ToggleWarmup(Label badge)
    {
        _isWarmup = !_isWarmup;
        badge.Text = _isWarmup ? "🔥" : "○";
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
        _unitToggle.Text = _useMinutes ? "min" : "sec";
        _restEntry.Placeholder = _useMinutes ? "min" : "sec";
    }

    private void Fill(LogSetEntity s)
    {
        _weightEntry.Text = s.WeightKg > 0 ? s.WeightKg.ToString("0.##") : "";
        _repsEntry.Text = s.Reps > 0 ? s.Reps.ToString() : "";
        _restEntry.Text = s.RestSeconds > 0 ? s.RestSeconds.ToString() : "";
        _isWarmup = s.IsWarmup;
        _warmupBadge.Text = _isWarmup ? "🔥" : "○";
    }

    public void UpdateNumber(int n) => _numberLabel.Text = $"Set {n}";

    private static Entry NumEntry(string placeholder) => new()
    {
        Placeholder = placeholder,
        PlaceholderColor = Color.FromArgb("#C0C0C0"),
        TextColor = Color.FromArgb("#111827"),
        Keyboard = Keyboard.Numeric,
        FontSize = 13,
        HorizontalTextAlignment = TextAlignment.Center,
    };

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

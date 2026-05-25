using Moduels.Workouts.DTO.Enums;
using WorkoutLogg.Database;
using WorkoutLogg.Database.Entities;

namespace WorkoutLogg.Pages;

[QueryProperty(nameof(WorkoutIdParam), "workoutId")]
public partial class AddWorkoutPage : ContentPage, IQueryAttributable
{
    private readonly WorkoutDatabase _db;
    private WorkoutType _selectedType = WorkoutType.Strength;
    private Guid _editingId = Guid.Empty;
    private readonly List<ExerciseFormRow> _exerciseRows = [];

    private static readonly (WorkoutType Type, string Label, string Emoji)[] WorkoutTypes =
    [
        (WorkoutType.Strength,    "Strength",    "🏋️"),
        (WorkoutType.Cardio,      "Cardio",      "🏃"),
        (WorkoutType.BodyBuilding,"Body Building","💪"),
        (WorkoutType.Running,     "Running",     "🏃"),
        (WorkoutType.Yoga,        "Yoga",        "🧘"),
        (WorkoutType.Stretch,     "Stretch",     "🧘"),
    ];

    public string? WorkoutIdParam { get; set; }

    public AddWorkoutPage(WorkoutDatabase db)
    {
        InitializeComponent();
        _db = db;
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("workoutId", out var val) && Guid.TryParse(val?.ToString(), out var id))
            _editingId = id;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_editingId != Guid.Empty)
        {
            var workout = await _db.GetWorkoutWithExercisesAsync(_editingId);
            if (workout is null) { await Shell.Current.GoToAsync(".."); return; }

            _selectedType = workout.MuscleGroup;
            WorkoutDatePicker.Date = workout.StartDate.Date;
            StartTimePicker.Time = workout.StartDate.TimeOfDay;
            EndTimePicker.Time = workout.EndDate.TimeOfDay;

            ExercisesList.Children.Clear();
            _exerciseRows.Clear();

            foreach (var ex in workout.Exercises)
                AddExerciseRow(ex);
        }
        else
        {
            WorkoutDatePicker.Date = DateTime.Today;
            StartTimePicker.Time = TimeSpan.FromHours(DateTime.Now.Hour);
            EndTimePicker.Time = TimeSpan.FromHours(DateTime.Now.Hour + 1);
        }

        ExercisesEmpty.IsVisible = _exerciseRows.Count == 0;
        BuildTypeChips();
    }

    private void AddExerciseRow(WorkoutSetEntity? existing = null)
    {
        var row = new ExerciseFormRow(existing);
        row.DeleteRequested += r => { _exerciseRows.Remove(r); ExercisesList.Children.Remove(r.View); ExercisesEmpty.IsVisible = _exerciseRows.Count == 0; };
        _exerciseRows.Add(row);
        ExercisesList.Children.Add(row.View);
        ExercisesEmpty.IsVisible = false;
    }

    private void OnAddExerciseTapped(object sender, TappedEventArgs e) => AddExerciseRow();

    private void BuildTypeChips()
    {
        TypeChips.Children.Clear();
        foreach (var (type, label, emoji) in WorkoutTypes)
            TypeChips.Children.Add(BuildChip(type, $"{emoji} {label}", type == _selectedType));
    }

    private Border BuildChip(WorkoutType type, string text, bool active)
    {
        var label = new Label
        {
            Text = text, FontSize = 13, FontAttributes = FontAttributes.Bold,
            TextColor = active ? Colors.White : Color.FromArgb("#6B7280"),
        };
        label.GestureRecognizers.Add(new TapGestureRecognizer
        {
            CommandParameter = type,
            Command = new Command<WorkoutType>(t => { _selectedType = t; BuildTypeChips(); }),
        });
        return new Border
        {
            BackgroundColor = active ? Color.FromArgb("#7C3AED") : Colors.White,
            Stroke = active ? Color.FromArgb("#7C3AED") : Color.FromArgb("#E5E7EB"),
            StrokeThickness = active ? 0 : 1.5,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 },
            Padding = new Thickness(14, 8),
            Content = label,
        };
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var date = (WorkoutDatePicker.Date ?? DateTime.Today).Date;
        var startTime = StartTimePicker.Time ?? TimeSpan.FromHours(DateTime.Now.Hour);
        var endTime   = EndTimePicker.Time   ?? startTime.Add(TimeSpan.FromHours(1));
        var start = date + startTime;
        var end   = date + endTime;
        if (end <= start) end = start.AddHours(1);

        var exercises = _exerciseRows
            .Select(r => r.ToEntity(Guid.Empty))
            .Where(ex => !string.IsNullOrWhiteSpace(ex.ExerciseName))
            .ToList();

        if (_editingId != Guid.Empty)
        {
            var existing = await _db.GetWorkoutWithExercisesAsync(_editingId);
            if (existing is null) { await Shell.Current.GoToAsync(".."); return; }
            existing.MuscleGroup = _selectedType;
            existing.StartDate = start;
            existing.EndDate = end;
            existing.IsSynced = false;
            await _db.SaveWorkoutAsync(existing);
            await _db.ReplaceExercisesAsync(_editingId, exercises);
        }
        else
        {
            var workout = new WorkoutEntity
            {
                Id = Guid.NewGuid(), MuscleGroup = _selectedType,
                StartDate = start, EndDate = end,
                IsSynced = false, RemoteId = Guid.Empty,
            };
            var res = await _db.SaveWorkoutAsync(workout);
            await _db.ReplaceExercisesAsync(workout.Id, exercises);
        }

        await Shell.Current.GoToAsync("..");
    }

    private void OnBackClicked(object sender, EventArgs e) => Shell.Current.GoToAsync("..");
}

// ─── ExerciseFormRow ──────────────────────────────────────────────────────────

internal class ExerciseFormRow
{
    private readonly Entry _nameEntry;
    private readonly Entry _descEntry;
    private readonly Picker _complexityPicker;
    private readonly List<SetFormRow> _setRows = [];
    private readonly VerticalStackLayout _setsContainer;

    public View View { get; }
    public event Action<ExerciseFormRow>? DeleteRequested;

    public ExerciseFormRow(WorkoutSetEntity? existing)
    {
        _nameEntry = TextField("Exercise name", 15, FontAttributes.Bold);
        _descEntry = TextField("Description (optional)", 13, FontAttributes.None);

        _complexityPicker = new Picker
        {
            Title = "Complexity", TextColor = Color.FromArgb("#111827"),
            TitleColor = Color.FromArgb("#9CA3AF"),
        };
        _complexityPicker.Items.Add("🟢  Low");
        _complexityPicker.Items.Add("🟡  Middle");
        _complexityPicker.Items.Add("🔴  High");
        _complexityPicker.SelectedIndex = 0;

        if (existing is not null)
        {
            _nameEntry.Text = existing.ExerciseName;
            _descEntry.Text = existing.Description;
            _complexityPicker.SelectedIndex = (int)existing.ExerciesComplexity;
        }

        _setsContainer = new VerticalStackLayout { Spacing = 8 };

        if (existing?.Sets.Count > 0)
        {
            foreach (var s in existing.Sets.OrderBy(x => x.SetNumber))
            {
                var row = new SetFormRow(s, existing.Sets.IndexOf(s) + 1);
                row.DeleteRequested += RemoveSetRow;
                _setRows.Add(row);
                _setsContainer.Children.Add(row.View);
            }
        }

        var addSetBtn = new Border
        {
            BackgroundColor = Color.FromArgb("#EDE9FE"),
            Padding = new Thickness(12, 8),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            StrokeThickness = 0,
            HorizontalOptions = LayoutOptions.Start,
            Content = new Label { Text = "+ Add Set", FontSize = 13, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#7C3AED") },
        };
        addSetBtn.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(AddSet),
        });

        var deleteExerciseBtn = new Label
        {
            Text = "✕", FontSize = 16, TextColor = Color.FromArgb("#EF4444"),
            HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center,
        };
        deleteExerciseBtn.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => DeleteRequested?.Invoke(this)),
        });

        var header = new Grid { ColumnDefinitions = new ColumnDefinitionCollection(new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto)) };
        header.Children.Add(_nameEntry);
        Grid.SetColumn(deleteExerciseBtn, 1);
        header.Children.Add(deleteExerciseBtn);

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
                Children = { header, _descEntry, _complexityPicker, _setsContainer, addSetBtn }
            }
        };
    }

    private void AddSet()
    {
        var num = _setRows.Count + 1;
        var row = new SetFormRow(null, num);
        row.DeleteRequested += RemoveSetRow;
        _setRows.Add(row);
        _setsContainer.Children.Add(row.View);
        RenumberSets();
    }

    private void RemoveSetRow(SetFormRow row)
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

    private static Entry TextField(string placeholder, double fontSize, FontAttributes attrs) => new()
    {
        Placeholder = placeholder, PlaceholderColor = Color.FromArgb("#9CA3AF"),
        TextColor = Color.FromArgb("#111827"), FontSize = fontSize, FontAttributes = attrs,
    };

    public WorkoutSetEntity ToEntity(Guid workoutId) => new()
    {
        Id = Guid.NewGuid(),
        WorkoutId = workoutId,
        ExerciseName = _nameEntry.Text ?? "",
        Description = _descEntry.Text ?? "",
        ExerciesComplexity = _complexityPicker.SelectedIndex switch
        {
            1 => ExerciesComplexity.Middle,
            2 => ExerciesComplexity.High,
            _ => ExerciesComplexity.Low,
        },
        Sets = _setRows.Select((r, i) => r.ToDetail(Guid.Empty, i + 1)).ToList(),
    };
}

// ─── SetFormRow ───────────────────────────────────────────────────────────────

internal class SetFormRow
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
    public event Action<SetFormRow>? DeleteRequested;

    public SetFormRow(WorkoutSetDetailEntity? existing, int number)
    {
        _isWarmup = existing?.IsWarmup ?? false;

        _numberLabel = new Label
        {
            Text = $"Set {number}", FontSize = 12, FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#9CA3AF"), VerticalOptions = LayoutOptions.Center, MinimumWidthRequest = 42,
        };

        _warmupBadge = BuildWarmupBadge();

        _weightEntry = NumEntry("kg");
        _repsEntry   = NumEntry("reps");
        _restEntry   = NumEntry("rest");

        _useMinutes = false;
        _unitToggle = new Label
        {
            Text = "sec", FontSize = 11, FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#7C3AED"), VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(4, 2),
        };
        _unitToggle.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(ToggleUnit) });

        var deleteBtn = new Label
        {
            Text = "✕", FontSize = 13, TextColor = Color.FromArgb("#EF4444"),
            VerticalOptions = LayoutOptions.Center,
        };
        deleteBtn.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => DeleteRequested?.Invoke(this)) });

        if (existing is not null) Fill(existing);

        // Layout: [Set N] [🔥] │ [wt] kg × [reps] │ ⏱ [rest] [sec] │ [✕]
        var row = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection(
                new ColumnDefinition(GridLength.Auto),   // Set N
                new ColumnDefinition(GridLength.Auto),   // warmup
                new ColumnDefinition(new GridLength(1, GridUnitType.Star)),  // weight
                new ColumnDefinition(GridLength.Auto),   // "kg ×"
                new ColumnDefinition(new GridLength(1, GridUnitType.Star)),  // reps
                new ColumnDefinition(GridLength.Auto),   // "⏱"
                new ColumnDefinition(new GridLength(1, GridUnitType.Star)),  // rest
                new ColumnDefinition(GridLength.Auto),   // unit toggle
                new ColumnDefinition(GridLength.Auto)),  // ✕
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

        // Wrap in a subtle separator
        View = new VerticalStackLayout
        {
            Spacing = 0,
            Children =
            {
                new BoxView { HeightRequest = 1, Color = Color.FromArgb("#F3F4F6") },
                row,
            }
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
        badge.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => ToggleWarmup(badge)) });
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
            if (!_useMinutes)
            {
                // sec → min: store display in minutes
                _restEntry.Text = (val / 60.0).ToString("0.##");
            }
            else
            {
                // min → sec
                _restEntry.Text = ((int)(val * 60)).ToString();
            }
        }
        _useMinutes = !_useMinutes;
        _unitToggle.Text = _useMinutes ? "min" : "sec";
        _restEntry.Placeholder = _useMinutes ? "min" : "sec";
    }

    private void Fill(WorkoutSetDetailEntity s)
    {
        _weightEntry.Text = s.WeightKg > 0 ? s.WeightKg.ToString("0.##") : "";
        _repsEntry.Text   = s.Reps > 0     ? s.Reps.ToString()           : "";
        // default display in seconds
        _restEntry.Text   = s.RestSeconds > 0 ? s.RestSeconds.ToString() : "";
        _isWarmup = s.IsWarmup;
        _warmupBadge.Text = _isWarmup ? "🔥" : "○";
    }

    public void UpdateNumber(int n) => _numberLabel.Text = $"Set {n}";

    private static Entry NumEntry(string placeholder) => new()
    {
        Placeholder = placeholder, PlaceholderColor = Color.FromArgb("#C0C0C0"),
        TextColor = Color.FromArgb("#111827"), Keyboard = Keyboard.Numeric,
        FontSize = 13, HorizontalTextAlignment = TextAlignment.Center,
    };

    private int GetRestSeconds()
    {
        if (!double.TryParse(_restEntry.Text, out var val)) return 60;
        return _useMinutes ? (int)(val * 60) : (int)val;
    }

    public WorkoutSetDetailEntity ToDetail(Guid workoutSetId, int setNumber) => new()
    {
        Id           = Guid.NewGuid(),
        WorkoutSetId = workoutSetId,
        SetNumber    = setNumber,
        Reps         = int.TryParse(_repsEntry.Text,    out var r) ? r : 0,
        WeightKg     = double.TryParse(_weightEntry.Text, out var w) ? w : 0,
        RestSeconds  = GetRestSeconds(),
        IsWarmup     = _isWarmup,
    };
}

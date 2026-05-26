using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Moduels.Workouts.DTO.Enums;
using System.Collections.ObjectModel;
using WorkoutLogg.Database;
using WorkoutLogg.Database.Entities;
using WorkoutLogg.Services;

namespace WorkoutLogg.PageModels
{
    public partial class WorkoutsPageModel : ObservableObject
    {
        private readonly WorkoutDatabase _db;
        private readonly WorkoutSyncService _sync;

        private List<WorkoutEntity> _allWorkouts = [];

        [ObservableProperty]
        private ObservableCollection<WorkoutDisplayItem> workouts = [];

        [ObservableProperty]
        private DateTime? selectedDate = DateTime.Today;

        [ObservableProperty]
        private WorkoutType activeFilter = WorkoutType.All;

        [ObservableProperty]
        private string sessionCount = "0 sessions logged";

        [ObservableProperty]
        private ObservableCollection<DateTime> markedDates = [];

        [ObservableProperty]
        private bool isEmpty = true;

        public WorkoutsPageModel(WorkoutDatabase db, WorkoutSyncService sync)
        {
            _db = db;
            _sync = sync;
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            _allWorkouts = await _db.GetWorkoutsAsync();

            var dates = _allWorkouts.Select(w => w.StartDate.Date).Distinct();
            MarkedDates = new ObservableCollection<DateTime>(dates);
            SessionCount = $"{_allWorkouts.Count} sessions logged";

            ApplyFilter();
            _ = _sync.TrySyncAsync();
        }

        [RelayCommand]
        public void SetFilter(WorkoutType filter)
        {
            ActiveFilter = filter;
            ApplyFilter();
        }

        [RelayCommand]
        public void SelectDate(DateTime date)
        {
            SelectedDate = SelectedDate?.Date == date.Date ? null : date;
            ApplyFilter();
        }

        [RelayCommand]
        public async Task DeleteWorkoutAsync(Guid workoutId)
        {
            var entity = _allWorkouts.FirstOrDefault(w => w.Id == workoutId);
            await _db.DeleteWorkoutAsync(workoutId);

            if (entity is not null)
                _ = _sync.TryDeleteRemoteAsync(entity.RemoteId);

            await LoadAsync();
        }

        private void ApplyFilter()
        {
            IEnumerable<WorkoutEntity> filtered = _allWorkouts;

            if (ActiveFilter != WorkoutType.All)
                filtered = filtered.Where(w => w.MuscleGroup == ActiveFilter);

            if (SelectedDate.HasValue)
                filtered = filtered.Where(w => w.StartDate.Date == SelectedDate.Value.Date);

            var items = filtered
                .OrderByDescending(w => w.StartDate)
                .Select(WorkoutDisplayItem.FromEntity)
                .ToList();

            Workouts = new ObservableCollection<WorkoutDisplayItem>(items);
            IsEmpty = items.Count == 0;
        }
    }

    public class WorkoutDisplayItem
    {
        public Guid Id { get; set; }
        public WorkoutType WorkoutType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ExerciseCount { get; set; }
        public double TotalWeightKg { get; set; }
        public bool IsSynced { get; set; }

        public string Emoji => WorkoutType switch
        {
            WorkoutType.Strength => "🏋️",
            WorkoutType.Cardio => "🏃",
            WorkoutType.Stretch => "🧘",
            WorkoutType.BodyBuilding => "💪",
            WorkoutType.Yoga => "🧘",
            WorkoutType.Running => "🏃",
            _ => "🏋️",
        };

        public string TypeLabel => WorkoutType.ToString();

        public string DateLabel
        {
            get
            {
                var today = DateTime.Today;
                var date = StartDate.Date;
                if (date == today) return "Today";
                if (date == today.AddDays(-1)) return "Yesterday";
                return StartDate.ToString("ddd, d MMM");
            }
        }

        public int DurationMinutes => (int)(EndDate - StartDate).TotalMinutes;

        public string SubLabel => $"{DateLabel} · {DurationMinutes} min · {ExerciseCount} exercises";

        public string WeightLabel => WorkoutType is WorkoutType.Cardio or WorkoutType.Running
            ? "—"
            : TotalWeightKg > 0 ? $"{TotalWeightKg:0} kg" : "—";

        public Color IconBackground => WorkoutType switch
        {
            WorkoutType.Cardio or WorkoutType.Running => Color.FromArgb("#EDE9FE"),
            WorkoutType.Stretch or WorkoutType.Yoga => Color.FromArgb("#FDE8FF"),
            _ => Color.FromArgb("#7C3AED"),
        };

        public Color IconTextColor => WorkoutType is WorkoutType.Cardio or WorkoutType.Running
            or WorkoutType.Stretch or WorkoutType.Yoga
            ? Color.FromArgb("#7C3AED")
            : Colors.White;

        public Color WeightColor => WeightLabel == "—"
            ? Color.FromArgb("#9CA3AF")
            : Color.FromArgb("#7C3AED");

        public static WorkoutDisplayItem FromEntity(WorkoutEntity e) => new()
        {
            Id = e.Id,
            WorkoutType = e.MuscleGroup,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            ExerciseCount = e.Exercises.Count,
            TotalWeightKg = e.Exercises.Sum(ex => ex.Sets.Where(s => !s.IsWarmup).Sum(s => s.WeightKg * s.Reps)),
            IsSynced = e.IsSynced,
        };
    }
}

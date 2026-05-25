using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WorkoutLogg.Database;
using WorkoutLogg.Database.Entities;

namespace WorkoutLogg.PageModels
{
    public partial class LoggerPageModel : ObservableObject
    {
        private readonly WorkoutDatabase _db;

        [ObservableProperty] private DateTime selectedDate = DateTime.Today;
        [ObservableProperty] private string dateLabel = "Today";
        [ObservableProperty] private ObservableCollection<LogSessionDisplayItem> sessions = [];
        [ObservableProperty] private bool isEmpty = true;
        [ObservableProperty] private ObservableCollection<DateTime> markedDates = [];

        public LoggerPageModel(WorkoutDatabase db) => _db = db;

        [RelayCommand]
        public async Task LoadAsync()
        {
            DateLabel = FormatDate(SelectedDate);

            var data = await _db.GetLogSessionsForDateAsync(SelectedDate);
            Sessions = new ObservableCollection<LogSessionDisplayItem>(
                data.Select(LogSessionDisplayItem.FromEntity));
            IsEmpty = Sessions.Count == 0;

            var logged = await _db.GetLoggedDatesAsync();
            MarkedDates = new ObservableCollection<DateTime>(logged);
        }

        [RelayCommand]
        public async Task DeleteSessionAsync(Guid sessionId)
        {
            await _db.DeleteLogSessionAsync(sessionId);
            await LoadAsync();
        }

        private static string FormatDate(DateTime d)
        {
            if (d.Date == DateTime.Today) return "Today";
            if (d.Date == DateTime.Today.AddDays(-1)) return "Yesterday";
            return d.ToString("dddd, d MMMM");
        }
    }

    public class LogSessionDisplayItem
    {
        public Guid Id { get; set; }
        public bool IsCustom { get; set; }
        public string WorkoutLabel { get; set; } = "";
        public int ExerciseCount { get; set; }
        public int TotalSets { get; set; }
        public double TotalWeightKg { get; set; }

        public string Emoji => IsCustom ? "⚡" : "📋";
        public string Title => IsCustom ? "Custom workout" : WorkoutLabel;
        public string SubLabel => $"{ExerciseCount} exercises · {TotalSets} sets";

        public string WeightLabel => TotalWeightKg > 0 ? $"{TotalWeightKg:0} kg" : "—";

        public string BadgeText => IsCustom ? "⚡ Custom" : "📋 From plan";

        public Color BadgeColor => IsCustom
            ? Color.FromArgb("#FEF3C7")
            : Color.FromArgb("#EDE9FE");

        public Color BadgeTextColor => IsCustom
            ? Color.FromArgb("#D97706")
            : Color.FromArgb("#7C3AED");

        public static LogSessionDisplayItem FromEntity(WorkoutLogSessionEntity e) => new()
        {
            Id = e.Id,
            IsCustom = e.IsCustom,
            WorkoutLabel = e.WorkoutLabel,
            ExerciseCount = e.Exercises.Count,
            TotalSets = e.Exercises.Sum(ex => ex.Sets.Count),
            TotalWeightKg = e.Exercises
                .Sum(ex => ex.Sets.Where(s => !s.IsWarmup).Sum(s => s.WeightKg * s.Reps)),
        };
    }
}

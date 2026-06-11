using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkoutLogg.Database;
using WorkoutLogg.Database.Entities;
using WorkoutLogg.Localization;

namespace WorkoutLogg.PageModels
{
    public partial class DashboardPageModel : ObservableObject
    {
        private readonly WorkoutDatabase _db;

        // ── Stats cards ───────────────────────────────────────────────────────
        [ObservableProperty] private string totalWorkoutsLabel = "0";
        [ObservableProperty] private string weeklyChangeLabel = "0 this week";
        [ObservableProperty] private string streakDaysLabel = "0";
        [ObservableProperty] private string weekVolumeLabel = "0 kg";

        // ── Goals (weekly) ────────────────────────────────────────────────────
        [ObservableProperty] private string workoutsGoalPct = "0%";
        [ObservableProperty] private string volumeGoalPct = "0%";
        [ObservableProperty] private string consistencyGoalPct = "0%";
        [ObservableProperty] private string workoutsGoalSub = "0 / 4 sessions";
        [ObservableProperty] private string volumeGoalSub = "0 / 5 000 kg";
        [ObservableProperty] private string consistencyGoalSub = "0 / 5 days";

        // ── Last logged session ───────────────────────────────────────────────
        [ObservableProperty] private string lastTitle = "No workouts logged yet";
        [ObservableProperty] private string lastSubLabel = "Tap + in Logger to start";
        [ObservableProperty] private string lastVolumeLabel = "—";
        [ObservableProperty] private string lastEmoji = "📓";
        [ObservableProperty] private bool hasLastWorkout;

        // ── Week bars (built in code-behind via LoadAsync) ────────────────────
        public List<DayBarData> WeekBars { get; private set; } = [];

        public DashboardPageModel(WorkoutDatabase db) => _db = db;

        [RelayCommand]
        public async Task LoadAsync()
        {
            var today = DateTime.Today;
            var weekStart = today.AddDays(-(((int)today.DayOfWeek + 6) % 7)); // Monday

            // Total logged sessions
            var total = await _db.GetTotalLogSessionCountAsync();
            TotalWorkoutsLabel = total.ToString();

            // This-week sessions (full hierarchy for volume calculation)
            var weekSessions = await _db.GetLogSessionsForWeekAsync(weekStart);
            WeeklyChangeLabel = $"+{weekSessions.Count} {Loc.Get("Dashboard_ThisWeek")}";

            // Streak (from distinct logged dates)
            var allDates = await _db.GetLoggedDatesAsync();
            StreakDaysLabel = CalculateStreak(allDates).ToString();

            // Volume this week
            var weekVol = CalcVolume(weekSessions);
            WeekVolumeLabel = FormatVolume(weekVol);

            // Goals (targets: 4 sessions, 5 000 kg, 5 active days)
            const int sessionsTarget = 4;
            const double volumeTarget = 5_000;
            const int daysTarget = 5;

            var activeDays = weekSessions.Select(s => s.Date.Date).Distinct().Count();
            var wCount = weekSessions.Count;

            WorkoutsGoalPct = $"{Math.Min(100, wCount * 100 / sessionsTarget)}%";
            VolumeGoalPct = $"{Math.Min(100, (int)(weekVol * 100 / volumeTarget))}%";
            ConsistencyGoalPct = $"{Math.Min(100, activeDays * 100 / daysTarget)}%";

            WorkoutsGoalSub = $"{wCount} / {sessionsTarget} {Loc.Get("Common_Sessions")}";
            VolumeGoalSub = $"{weekVol:0} / {volumeTarget:0} {Loc.Get("Common_Kg")}";
            ConsistencyGoalSub = $"{activeDays} / {daysTarget} {Loc.Get("Dashboard_Days")}";

            // Weekly bar data
            WeekBars = BuildWeekBars(weekSessions, weekStart, today);

            // Last logged session
            var last = await _db.GetLastLogSessionAsync();
            if (last is not null)
            {
                HasLastWorkout = true;
                LastTitle = last.IsCustom ? Loc.Get("Dashboard_CustomWorkout") : last.WorkoutLabel;
                LastEmoji = last.IsCustom ? "⚡" : "🏋️";

                var culture = new CultureInfo(Loc.Get("_Culture"));
                var dateStr = last.Date.Date == today ? Loc.Get("Dashboard_Today")
                    : last.Date.Date == today.AddDays(-1) ? Loc.Get("Dashboard_Yesterday")
                    : last.Date.ToString("d MMM", culture);

                var sets = last.Exercises.Sum(ex => ex.Sets.Count);
                LastSubLabel = $"{dateStr} · {last.Exercises.Count} exercises · {sets} sets";

                var vol = CalcVolume([last]);
                LastVolumeLabel = vol > 0 ? $"{vol:0} {Loc.Get("Common_Kg")}" : "—";
            }
            else
            {
                HasLastWorkout = false;
                LastTitle = Loc.Get("Dashboard_NoWorkouts");
                LastSubLabel = Loc.Get("Dashboard_TapToStart");
                LastVolumeLabel = "";
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static double CalcVolume(IEnumerable<WorkoutLogSessionEntity> sessions) =>
            sessions.Sum(s => s.Exercises
                .Sum(ex => ex.Sets.Where(set => !set.IsWarmup)
                    .Sum(set => set.WeightKg * set.Reps)));

        private static string FormatVolume(double kg) =>
            kg >= 1000 ? $"{kg / 1000:0.#}t" : $"{kg:0} {Loc.Get("Common_Kg")}";

        private static int CalculateStreak(List<DateTime> distinctDates)
        {
            if (distinctDates.Count == 0) return 0;

            var sorted = distinctDates.Select(d => d.Date).Distinct()
                .OrderByDescending(d => d).ToList();
            var today = DateTime.Today;

            // Streak is dead if last log was more than 1 day ago
            if (sorted[0] < today.AddDays(-1)) return 0;

            var streak = 0;
            var expected = sorted[0];

            foreach (var date in sorted)
            {
                if (date == expected) { streak++; expected = expected.AddDays(-1); }
                else break;
            }

            return streak;
        }

        private static List<DayBarData> BuildWeekBars(
            List<WorkoutLogSessionEntity> sessions, DateTime weekStart, DateTime today)
        {
            var counts = new int[7];
            foreach (var s in sessions)
                counts[((int)s.Date.DayOfWeek + 6) % 7]++;

            double maxCount = counts.Max() is > 0 ? counts.Max() : 1;
            string[] labels =
            [
                Loc.Get("Calendar_Mo"), Loc.Get("Calendar_Tu"), Loc.Get("Calendar_We"),
                Loc.Get("Calendar_Th"), Loc.Get("Calendar_Fr"), Loc.Get("Calendar_Sa"),
                Loc.Get("Calendar_Su"),
            ];

            var bars = new List<DayBarData>();
            for (int i = 0; i < 7; i++)
            {
                bars.Add(new DayBarData
                {
                    DayLabel = labels[i],
                    BarHeight = counts[i] > 0 ? Math.Max(12, counts[i] / maxCount * 68) : 8,
                    HasActivity = counts[i] > 0,
                    IsToday = weekStart.AddDays(i).Date == today,
                });
            }
            return bars;
        }
    }

    public class DayBarData
    {
        public string DayLabel { get; set; } = "";
        public double BarHeight { get; set; }
        public bool HasActivity { get; set; }
        public bool IsToday { get; set; }
    }
}

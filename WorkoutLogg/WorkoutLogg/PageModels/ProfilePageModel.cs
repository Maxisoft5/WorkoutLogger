using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Modules.Users.DTO.Auth;
using Modules.Users.DTO.Users;
using System.Collections.ObjectModel;
using WorkoutLogg.Database;
using WorkoutLogg.Services;

namespace WorkoutLogg.PageModels
{
    public partial class ProfilePageModel : ObservableObject
    {
        private readonly WorkoutDatabase _db;
        private readonly UserProfileService _userService;

        // ── User card ──────────────────────────────────────────────────────────
        [ObservableProperty] string userInitials = "?";
        [ObservableProperty] string userName = "";
        [ObservableProperty] string userEmail = "";
        [ObservableProperty] string weightLabel = "—";
        [ObservableProperty] string heightLabel = "—";
        [ObservableProperty] string bodyFatLabel = "—";
        [ObservableProperty] string memberSinceLabel = "";
        [ObservableProperty] bool isPremium = false;
        [ObservableProperty] string streakLabel = "0";
        [ObservableProperty] string totalSessionsLabel = "0";

        // ── Personal Records (always 4 slots) ─────────────────────────────────
        [ObservableProperty] ObservableCollection<PersonalRecordVM> personalRecords = [];

        // ── Achievements ───────────────────────────────────────────────────────
        [ObservableProperty] ObservableCollection<AchievementVM> achievements = [];
        [ObservableProperty] string unlockedLabel = "All ›";

        public ProfilePageModel(WorkoutDatabase db, UserProfileService userService)
        {
            _db = db;
            _userService = userService;
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            var profile = await _userService.RefreshProfileAsync();
            if (profile is not null)
                ApplyProfile(profile, await _userService.GetJoinedDateAsync());

            var stats = await _db.GetProfileStatsAsync();
            ApplyStats(stats, profile);
        }

        private void ApplyProfile(UserDto user, DateTime? joined)
        {
            var name = !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName : user.Email ?? "User";
            UserName = name;
            UserEmail = user.Email ?? "";
            UserInitials = BuildInitials(name);
            IsPremium = user.IsPremium == true;

            var stats = user.BodyStats;
            WeightLabel = stats?.Kg > 0 ? $"{stats.Kg} kg" : "—";
            HeightLabel = stats?.Cm > 0 ? $"{stats.Cm} cm" : "—";
            BodyFatLabel = stats?.Fat > 0 ? $"{stats.Fat:0}%" : "—";

            MemberSinceLabel = joined.HasValue
                ? $"Member since {joined.Value:MMM yyyy}"
                : "Member";
        }

        private void ApplyStats(ProfileStats stats, UserDto? user)
        {
            StreakLabel = stats.CurrentStreak.ToString();
            TotalSessionsLabel = stats.TotalSessions.ToString();

            // Personal Records — always 4 slots
            var prs = stats.TopPRs;
            PersonalRecords = new ObservableCollection<PersonalRecordVM>(
                Enumerable.Range(0, 4).Select(i => i < prs.Count
                    ? new PersonalRecordVM(prs[i].ExerciseName, $"{prs[i].MaxWeightKg:0.#} kg")
                    : new PersonalRecordVM("No record yet", "—")));

            // Achievements
            var unlocked = BuildAchievements(stats, user);
            Achievements = new ObservableCollection<AchievementVM>(unlocked);
            var count = unlocked.Count(a => a.IsUnlocked);
            UnlockedLabel = $"{count}/{unlocked.Count} ›";
        }

        private static List<AchievementVM> BuildAchievements(ProfileStats stats, UserDto? user)
        {
            var all = new[]
            {
                new AchievementVM("🎯", "First Step",      "Log your first workout",      stats.TotalSessions >= 1,            "#EDE9FE", "#7C3AED"),
                new AchievementVM("🦾", "Dedicated",       "10 workouts logged",          stats.TotalSessions >= 10,           "#EDE9FE", "#7C3AED"),
                new AchievementVM("💪", "Iron Will",       "30 workouts logged",          stats.TotalSessions >= 30,           "#EDE9FE", "#7C3AED"),
                new AchievementVM("🏆", "Legend",          "100 workouts logged",         stats.TotalSessions >= 100,          "#FEF3C7", "#D97706"),
                new AchievementVM("🔥", "On Fire",         "5+ workouts in one week",     stats.MaxWeekSessions >= 5,          "#FEE2E2", "#DC2626"),
                new AchievementVM("📅", "Week Streak",     "7 consecutive days",          stats.CurrentStreak >= 7,            "#DCFCE7", "#16A34A"),
                new AchievementVM("📅", "Month Streak",    "30 consecutive days",         stats.CurrentStreak >= 30,           "#DCFCE7", "#15803D"),
                new AchievementVM("⚡", "Power",           "Lift 100+ kg in one set",     stats.HasHeavySet,                   "#FEF3C7", "#D97706"),
                new AchievementVM("🌈", "Variety",         "Log 10 different exercises",  stats.UniqueExerciseCount >= 10,     "#EDE9FE", "#7C3AED"),
                new AchievementVM("📋", "Plan Follower",   "10 plan-based sessions",      stats.PlanBasedSessions >= 10,       "#DBEAFE", "#2563EB"),
                new AchievementVM("🚀", "Century Sets",    "100 total sets logged",       stats.TotalSets >= 100,              "#FEE2E2", "#DC2626"),
                new AchievementVM("🌅", "Early Riser",     "Log a workout before 8 AM",   stats.HasEarlySession,               "#FEF3C7", "#D97706"),
                new AchievementVM("⭐", "Premium",         "Premium member",              user?.IsPremium == true,             "#FEF3C7", "#D97706"),
            };
            return [.. all];
        }

        private static string BuildInitials(string name)
        {
            if (name.Contains('@'))
                return name[0].ToString().ToUpper();

            var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 2
                ? $"{parts[0][0]}{parts[^1][0]}".ToUpper()
                : parts[0][0].ToString().ToUpper();
        }
    }

    // ── Supporting view models ────────────────────────────────────────────────

    public class PersonalRecordVM(string exerciseName, string weightLabel)
    {
        public string ExerciseName { get; } = exerciseName;
        public string WeightLabel { get; } = weightLabel;
        public bool HasRecord => WeightLabel != "—";
        public Color ValueColor => HasRecord ? Color.FromArgb("#111827") : Color.FromArgb("#9CA3AF");
    }

    public class AchievementVM(string emoji, string title, string description,
        bool isUnlocked, string unlockedBg, string unlockedFg)
    {
        public string Emoji { get; } = emoji;
        public string Title { get; } = title;
        public string Description { get; } = description;
        public bool IsUnlocked { get; } = isUnlocked;

        public Color DisplayColor => IsUnlocked
            ? Color.FromArgb(unlockedBg)
            : Color.FromArgb("#F3F4F6");

        public Color TitleColor => IsUnlocked
            ? Color.FromArgb(unlockedFg)
            : Color.FromArgb("#D1D5DB");

        public double Opacity => IsUnlocked ? 1.0 : 0.5;
    }
}

using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Modules.Users.DTO.Auth;
using Modules.Users.DTO.Users;
using System.Collections.ObjectModel;
using WorkoutLogg.Database;
using WorkoutLogg.Localization;
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
        [ObservableProperty] bool hasProfilePicture = false;
        [ObservableProperty] bool hasNoProfilePicture = true;
        [ObservableProperty] ImageSource? avatarImageSource;
        [ObservableProperty] int achievementsGridHeight = 400;

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
            WeightLabel = stats?.Kg > 0 ? $"{stats.Kg} {Loc.Get("Common_Kg")}" : "—";
            HeightLabel = stats?.Cm > 0 ? $"{stats.Cm} {Loc.Get("Common_Cm")}" : "—";
            BodyFatLabel = stats?.Fat > 0 ? $"{stats.Fat:0}%" : "—";

            var culture = new CultureInfo(Loc.Get("_Culture"));
            MemberSinceLabel = joined.HasValue
                ? $"{Loc.Get("Profile_MemberSince")} {joined.Value.ToString("MMM yyyy", culture)}"
                : Loc.Get("Profile_Member");

            SetProfilePicture(user.ProfilePicture);
        }

        public void SetProfilePicture(string? dataUrl)
        {
            HasProfilePicture = !string.IsNullOrEmpty(dataUrl);
            HasNoProfilePicture = !HasProfilePicture;
            if (HasProfilePicture && dataUrl!.Contains(","))
            {
                var base64 = dataUrl.Split(',')[1];
                var bytes = Convert.FromBase64String(base64);
                AvatarImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            else
            {
                AvatarImageSource = null;
            }
        }

        public async Task<bool> UpdateProfilePictureAsync(string dataUrl)
        {
            var ok = await _userService.UpdateProfilePictureAsync(dataUrl);
            if (ok) SetProfilePicture(dataUrl);
            return ok;
        }

        private void ApplyStats(ProfileStats stats, UserDto? user)
        {
            StreakLabel = stats.CurrentStreak.ToString();
            TotalSessionsLabel = stats.TotalSessions.ToString();

            var prs = stats.TopPRs;
            PersonalRecords = new ObservableCollection<PersonalRecordVM>(
                Enumerable.Range(0, 4).Select(i => i < prs.Count
                    ? new PersonalRecordVM(prs[i].ExerciseName, $"{prs[i].MaxWeightKg:0.#} {Loc.Get("Common_Kg")}")
                    : new PersonalRecordVM(Loc.Get("Common_NoRecordYet"), "—")));

            var list = BuildAchievements(stats, user);
            Achievements = new ObservableCollection<AchievementVM>(list);
            var count = list.Count(a => a.IsUnlocked);
            UnlockedLabel = $"{count}/{list.Count} ›";
            AchievementsGridHeight = (int)Math.Ceiling(list.Count / 2.0) * 118;
        }

        private static double ExercisePR(List<PersonalRecordEntry> prs, params string[] keywords)
            => prs.Where(p => keywords.Any(k =>
                    p.ExerciseName.Contains(k, StringComparison.OrdinalIgnoreCase)))
               .Select(p => p.MaxWeightKg)
               .DefaultIfEmpty(0).Max();

        private static List<AchievementVM> BuildAchievements(ProfileStats stats, UserDto? user)
        {
            var all = stats.AllPRs;
            var s = stats;

            double bench    = ExercisePR(all, "bench", "жим лёж", "жим леж");
            double squat    = ExercisePR(all, "squat", "присед");
            double deadlift = ExercisePR(all, "deadlift", "становая", "dead lift");
            double total    = bench + squat + deadlift;

            string Kg(double v) => $"{v:0.#} {Loc.Get("Common_Kg")}";
            string Prog(double cur, double target) =>
                cur > 0 ? $"{Kg(cur)} / {Kg(target)}" : $"0 / {Kg(target)}";
            string Cnt(int cur, int target) => $"{cur} / {target}";

            return
            [
                // ── Общие ─────────────────────────────────────────────────────────
                new("🎯", Loc.Get("Ach_FirstStep"),    Loc.Get("Ach_FirstStep_Desc"),    "",                              s.TotalSessions >= 1,   "#EDE9FE", "#7C3AED"),
                new("💪", Loc.Get("Ach_10Sessions"),   Loc.Get("Ach_10Sessions_Desc"),   Cnt(s.TotalSessions,10),         s.TotalSessions >= 10,  "#EDE9FE", "#7C3AED"),
                new("🏋️", Loc.Get("Ach_30Sessions"),   Loc.Get("Ach_30Sessions_Desc"),   Cnt(s.TotalSessions,30),         s.TotalSessions >= 30,  "#EDE9FE", "#7C3AED"),
                new("🏆", Loc.Get("Ach_100Sessions"),  Loc.Get("Ach_100Sessions_Desc"),  Cnt(s.TotalSessions,100),        s.TotalSessions >= 100, "#FEF3C7", "#D97706"),

                // ── Постоянство ───────────────────────────────────────────────────
                new("🔥", Loc.Get("Ach_OnFire"),       Loc.Get("Ach_OnFire_Desc"),       Cnt(s.MaxWeekSessions,5),        s.MaxWeekSessions >= 5, "#FEE2E2", "#DC2626"),
                new("📅", Loc.Get("Ach_Week7"),        Loc.Get("Ach_Week7_Desc"),        Cnt(s.CurrentStreak,7),          s.CurrentStreak >= 7,   "#DCFCE7", "#16A34A"),
                new("🗓️", Loc.Get("Ach_Month30"),      Loc.Get("Ach_Month30_Desc"),      Cnt(s.CurrentStreak,30),         s.CurrentStreak >= 30,  "#DCFCE7", "#15803D"),
                new("🌅", Loc.Get("Ach_EarlyRiser"),   Loc.Get("Ach_EarlyRiser_Desc"),   "",                              s.HasEarlySession,      "#FEF3C7", "#D97706"),

                // ── Жим лёжа ──────────────────────────────────────────────────────
                new("🫷", Loc.Get("Ach_Bench60"),      Loc.Get("Ach_Bench60_Desc"),      Prog(bench,60),                  bench >= 60,            "#EDE9FE", "#7C3AED"),
                new("💪", Loc.Get("Ach_Bench100"),     Loc.Get("Ach_Bench100_Desc"),     Prog(bench,100),                 bench >= 100,           "#EDE9FE", "#7C3AED"),
                new("🏅", Loc.Get("Ach_Bench120"),     Loc.Get("Ach_Bench120_Desc"),     Prog(bench,120),                 bench >= 120,           "#FEF3C7", "#D97706"),
                new("👑", Loc.Get("Ach_Bench140"),     Loc.Get("Ach_Bench140_Desc"),     Prog(bench,140),                 bench >= 140,           "#FEF3C7", "#D97706"),

                // ── Присед ────────────────────────────────────────────────────────
                new("🦵", Loc.Get("Ach_Squat60"),      Loc.Get("Ach_Squat60_Desc"),      Prog(squat,60),                  squat >= 60,            "#DBEAFE", "#2563EB"),
                new("🦾", Loc.Get("Ach_Squat100"),     Loc.Get("Ach_Squat100_Desc"),     Prog(squat,100),                 squat >= 100,           "#DBEAFE", "#2563EB"),
                new("💫", Loc.Get("Ach_Squat140"),     Loc.Get("Ach_Squat140_Desc"),     Prog(squat,140),                 squat >= 140,           "#EDE9FE", "#7C3AED"),
                new("🔱", Loc.Get("Ach_Squat180"),     Loc.Get("Ach_Squat180_Desc"),     Prog(squat,180),                 squat >= 180,           "#FEF3C7", "#D97706"),

                // ── Становая ──────────────────────────────────────────────────────
                new("⛏️", Loc.Get("Ach_Dead80"),       Loc.Get("Ach_Dead80_Desc"),       Prog(deadlift,80),               deadlift >= 80,         "#DBEAFE", "#2563EB"),
                new("🪝", Loc.Get("Ach_Dead100"),      Loc.Get("Ach_Dead100_Desc"),      Prog(deadlift,100),              deadlift >= 100,        "#DBEAFE", "#2563EB"),
                new("⚓", Loc.Get("Ach_Dead140"),      Loc.Get("Ach_Dead140_Desc"),      Prog(deadlift,140),              deadlift >= 140,        "#EDE9FE", "#7C3AED"),
                new("🚀", Loc.Get("Ach_Dead200"),      Loc.Get("Ach_Dead200_Desc"),      Prog(deadlift,200),              deadlift >= 200,        "#FEF3C7", "#D97706"),

                // ── Пауэрлифтинг тотал ────────────────────────────────────────────
                new("⚡", Loc.Get("Ach_Total300"),     Loc.Get("Ach_Total300_Desc"),     Prog(total,300),                 total >= 300,           "#FEE2E2", "#DC2626"),
                new("🔥", Loc.Get("Ach_Total500"),     Loc.Get("Ach_Total500_Desc"),     Prog(total,500),                 total >= 500,           "#FEE2E2", "#DC2626"),

                // ── Разнообразие / прочее ─────────────────────────────────────────
                new("🌈", Loc.Get("Ach_Variety"),      Loc.Get("Ach_Variety_Desc"),      Cnt(s.UniqueExerciseCount,10),   s.UniqueExerciseCount >= 10, "#EDE9FE", "#7C3AED"),
                new("📋", Loc.Get("Ach_PlanPro"),      Loc.Get("Ach_PlanPro_Desc"),      Cnt(s.PlanBasedSessions,10),     s.PlanBasedSessions >= 10,   "#DBEAFE", "#2563EB"),
                new("🚩", Loc.Get("Ach_100Sets"),      Loc.Get("Ach_100Sets_Desc"),      Cnt(s.TotalSets,100),            s.TotalSets >= 100,          "#FEE2E2", "#DC2626"),
                new("⭐", Loc.Get("Ach_Premium"),      Loc.Get("Ach_Premium_Desc"),      "",                              user?.IsPremium == true,     "#FEF3C7", "#D97706"),
            ];
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
        string progressText, bool isUnlocked, string unlockedBg, string unlockedFg)
    {
        public string Emoji { get; } = emoji;
        public string Title { get; } = title;
        public string Description { get; } = description;
        public string ProgressText { get; } = progressText;
        public bool HasProgress { get; } = !string.IsNullOrEmpty(progressText);
        public bool IsUnlocked { get; } = isUnlocked;

        public Color EmojiCircleColor => IsUnlocked
            ? Color.FromArgb(unlockedBg)
            : Color.FromArgb("#F3F4F6");

        public Color CardBg => IsUnlocked
            ? Color.FromArgb(unlockedBg).WithAlpha(0.35f)
            : Color.FromArgb("#F9FAFB");

        public Color TitleColor => IsUnlocked
            ? Color.FromArgb(unlockedFg)
            : Color.FromArgb("#C9D1DB");

        public Color ProgressColor => IsUnlocked
            ? Color.FromArgb(unlockedFg)
            : Color.FromArgb("#9CA3AF");

        public double Opacity => IsUnlocked ? 1.0 : 0.65;
    }
}

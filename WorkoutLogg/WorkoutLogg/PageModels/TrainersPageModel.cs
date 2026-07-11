using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using WorkoutLogg.Localization;
using WorkoutLogg.Services;

namespace WorkoutLogg.PageModels
{
    /// <summary>
    /// Экран 02 «Ученик: вкладка Тренеры»: поиск тренеров с фильтрами по специализациям,
    /// сортировкой и блоком «Подобрано для вас». Match-скор считает бэкенд (M2).
    /// </summary>
    public partial class TrainersPageModel : ObservableObject
    {
        private readonly ITrainersApi _api;

        [ObservableProperty]
        private ObservableCollection<TrainerCardItem> results = [];

        [ObservableProperty]
        private ObservableCollection<TrainerCardItem> recommended = [];

        [ObservableProperty]
        private bool isEmpty;

        [ObservableProperty]
        private bool hasRecommended;

        [ObservableProperty]
        private TrainerSpecializations selectedSpecializations = TrainerSpecializations.None;

        [ObservableProperty]
        private TrainerSortBy sortBy = TrainerSortBy.Match;

        /// <summary>Минимальный рейтинг фильтра (null — без фильтра; 4.5 / 4.8 из дизайна).</summary>
        [ObservableProperty]
        private double? minRating;

        /// <summary>Выбранная карточка передаётся на страницу деталей (без сериализации через query).</summary>
        public TrainerCardItem? SelectedTrainer { get; set; }

        public TrainersPageModel(ITrainersApi api)
        {
            _api = api;
        }

        public async Task LoadAsync()
        {
            await LoadRecommendedAsync();
            await SearchAsync();
        }

        private async Task LoadRecommendedAsync()
        {
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token)) return;

            try
            {
                var resp = await _api.GetRecommendedAsync($"Bearer {token}", 3);
                if (resp.IsSuccessStatusCode && resp.Content is not null)
                {
                    Recommended = new ObservableCollection<TrainerCardItem>(
                        resp.Content.Select(TrainerCardItem.FromDto));
                    HasRecommended = Recommended.Count > 0;
                }
            }
            catch
            {
                // блок «Подобрано для вас» необязательный — молча пропускаем при ошибке
            }
        }

        public async Task SearchAsync()
        {
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token)) return;

            try
            {
                var resp = await _api.SearchAsync(
                    $"Bearer {token}",
                    (int)SelectedSpecializations,
                    (int)TrainingFormats.None,
                    null,
                    null,
                    MinRating,
                    (int)SortBy,
                    page: 1,
                    pageSize: 20);

                if (resp.IsSuccessStatusCode && resp.Content is not null)
                    Results = new ObservableCollection<TrainerCardItem>(
                        resp.Content.Items.Select(TrainerCardItem.FromDto));
                else
                    Results = [];
            }
            catch
            {
                Results = [];
            }
            finally
            {
                IsEmpty = Results.Count == 0;
            }
        }

        /// <summary>Переключить чип специализации (мультивыбор) и перезапустить поиск.</summary>
        public async Task ToggleSpecializationAsync(TrainerSpecializations flag)
        {
            SelectedSpecializations ^= flag;
            await SearchAsync();
        }

        public async Task SetSortAsync(TrainerSortBy sort)
        {
            if (SortBy == sort) return;
            SortBy = sort;
            await SearchAsync();
        }

        /// <summary>Фильтр по минимальному рейтингу (null / 4.5 / 4.8) и перезапуск поиска.</summary>
        public async Task SetMinRatingAsync(double? min)
        {
            if (MinRating == min) return;
            MinRating = min;
            await SearchAsync();
        }

        /// <summary>Найти карточку по UserId среди результатов и рекомендаций (для перехода к деталям).</summary>
        public TrainerCardItem? FindByUserId(string userId) =>
            Results.FirstOrDefault(t => t.UserId == userId)
            ?? Recommended.FirstOrDefault(t => t.UserId == userId);
    }

    /// <summary>Карточка тренера для списка/деталей: маппинг флагов в локализованные подписи.</summary>
    public class TrainerCardItem
    {
        public string UserId { get; set; } = "";
        public TrainerSpecializations Specializations { get; set; }
        public ExperienceRange Experience { get; set; }
        public TrainingFormats Formats { get; set; }
        public int PricePerSession { get; set; }
        public string? About { get; set; }
        public int MatchScore { get; set; }
        public bool HasVerifiedBadge { get; set; }
        public string? VerificationBadge { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public string Title => TrainerLabels.PrimarySpecialization(Specializations);
        public string Emoji => TrainerLabels.SpecializationEmoji(Specializations);
        public string SpecializationsLabel => TrainerLabels.Specializations(Specializations);
        public string ExperienceLabel => TrainerLabels.Experience(Experience);
        public string FormatsLabel => TrainerLabels.Formats(Formats);
        public string PriceLabel => $"{PricePerSession} FC";
        public string PricePerSessionLabel => $"{PricePerSession} FC / {Loc.Get("Trainers_PerSession")}";
        public string MatchLabel => string.Format(Loc.Get("Trainers_MatchPercent"), MatchScore);
        public string SubLabel => $"{ExperienceLabel} · {FormatsLabel}";
        public bool HasAbout => !string.IsNullOrWhiteSpace(About);

        // Рейтинг (M8) и верификация (M9)
        public bool HasRating => AverageRating.HasValue;
        public string RatingLabel => AverageRating.HasValue ? $"⭐ {AverageRating.Value:0.0}" : "";
        public string ReviewCountLabel => string.Format(Loc.Get("Trainers_Reviews"), ReviewCount);
        public string RatingWithCountLabel => AverageRating.HasValue
            ? $"⭐ {AverageRating.Value:0.0} · {ReviewCountLabel}"
            : Loc.Get("Trainers_NoReviews");
        public bool IsVerified => HasVerifiedBadge;
        public string VerifiedLabel => VerificationBadge == "Master"
            ? Loc.Get("Trainers_Badge_Master")
            : Loc.Get("Trainers_Badge_Verified");

        public Color MatchColor => MatchScore >= 80
            ? Color.FromArgb("#16A34A")
            : MatchScore >= 50 ? Color.FromArgb("#7C3AED") : Color.FromArgb("#9CA3AF");

        public static TrainerCardItem FromDto(TrainerSearchItemDto dto) => new()
        {
            UserId = dto.Profile.UserId,
            Specializations = dto.Profile.Specializations,
            Experience = dto.Profile.Experience,
            Formats = dto.Profile.Formats,
            PricePerSession = dto.Profile.PricePerSession,
            About = dto.Profile.About,
            MatchScore = dto.MatchScore,
            HasVerifiedBadge = dto.Profile.HasVerifiedBadge,
            VerificationBadge = dto.Profile.VerificationBadge,
            AverageRating = dto.AverageRating,
            ReviewCount = dto.ReviewCount,
        };
    }

    /// <summary>Локализованные подписи для доменных enum-ов тренеров.</summary>
    public static class TrainerLabels
    {
        private static readonly (TrainerSpecializations Flag, string Emoji)[] SpecEmojis =
        {
            (TrainerSpecializations.Strength, "🏋️"),
            (TrainerSpecializations.WeightLoss, "🔥"),
            (TrainerSpecializations.Crossfit, "🤸"),
            (TrainerSpecializations.Yoga, "🧘"),
            (TrainerSpecializations.Rehabilitation, "🩺"),
            (TrainerSpecializations.Running, "🏃"),
        };

        public static string Specializations(TrainerSpecializations s)
        {
            if (s == TrainerSpecializations.None) return Loc.Get("Trainers_Spec_Any");

            var parts = new List<string>();
            foreach (TrainerSpecializations flag in Enum.GetValues<TrainerSpecializations>())
            {
                if (flag == TrainerSpecializations.None) continue;
                if (s.HasFlag(flag)) parts.Add(Loc.Get($"Trainers_Spec_{flag}"));
            }
            return string.Join(" · ", parts);
        }

        public static string PrimarySpecialization(TrainerSpecializations s)
        {
            foreach (TrainerSpecializations flag in Enum.GetValues<TrainerSpecializations>())
            {
                if (flag == TrainerSpecializations.None) continue;
                if (s.HasFlag(flag)) return Loc.Get($"Trainers_Spec_{flag}");
            }
            return Loc.Get("Trainers_Title");
        }

        public static string SpecializationEmoji(TrainerSpecializations s)
        {
            foreach (var (flag, emoji) in SpecEmojis)
                if (s.HasFlag(flag)) return emoji;
            return "🏋️";
        }

        public static string Experience(ExperienceRange e) => Loc.Get($"Trainers_Exp_{e}");

        public static string Formats(TrainingFormats f)
        {
            if (f == TrainingFormats.None) return Loc.Get("Trainers_Fmt_Any");

            var parts = new List<string>();
            foreach (TrainingFormats flag in Enum.GetValues<TrainingFormats>())
            {
                if (flag == TrainingFormats.None) continue;
                if (f.HasFlag(flag)) parts.Add(Loc.Get($"Trainers_Fmt_{flag}"));
            }
            return string.Join(" · ", parts);
        }

        public static string Level(StudentLevel l) => Loc.Get($"Trainers_Level_{l}");
    }
}

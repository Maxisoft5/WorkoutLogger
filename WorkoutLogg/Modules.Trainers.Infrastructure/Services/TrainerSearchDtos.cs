using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    /// <summary>
    /// Фильтры поиска тренеров (bottom-sheet «Фильтры» на экране 02):
    /// цель → специализации, формат, цена. Фильтр по рейтингу появится вместе с отзывами (M8).
    /// </summary>
    public class TrainerSearchRequest
    {
        /// <summary>Желаемые специализации (флаги). None — без фильтра.</summary>
        public TrainerSpecializations Specializations { get; set; }

        /// <summary>Желаемые форматы (флаги). None — без фильтра.</summary>
        public TrainingFormats Formats { get; set; }

        public int? PriceMin { get; set; }
        public int? PriceMax { get; set; }

        public TrainerSortBy SortBy { get; set; } = TrainerSortBy.Match;

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public enum TrainerSortBy
    {
        Match = 0,
        PriceAsc = 1,
        PriceDesc = 2,
        Newest = 3
    }

    /// <summary>
    /// Предпочтения ученика для расчёта match-скора. Выводятся из целей ученика
    /// (UserGoals) и заданных фильтров; отсутствующее предпочтение не штрафует тренера.
    /// </summary>
    public class StudentPreferences
    {
        public TrainerSpecializations DesiredSpecializations { get; set; }
        public TrainingFormats DesiredFormats { get; set; }

        /// <summary>Бюджет за тренировку в FitCoins (обычно верхняя граница фильтра цены).</summary>
        public int? Budget { get; set; }
    }

    public class TrainerSearchItemDto
    {
        public TrainerProfileDto Profile { get; set; } = null!;

        /// <summary>Детерминированный match-скор 0–100 (v1: специализации 50 + формат 30 + цена 20).</summary>
        public int MatchScore { get; set; }
    }

    public class TrainerSearchPageDto
    {
        public List<TrainerSearchItemDto> Items { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}

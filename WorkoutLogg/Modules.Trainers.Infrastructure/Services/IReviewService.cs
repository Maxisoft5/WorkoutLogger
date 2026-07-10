using Modules.Common.Domain.Results;

namespace Modules.Trainers.Infrastructure.Services
{
    public interface IReviewService
    {
        /// <summary>Ученик оставляет отзыв после завершённой тренировки.</summary>
        Task<Result<ReviewDto>> PostAsync(string studentUserId, PostReviewRequest request, CancellationToken ct = default);

        /// <summary>Список отзывов тренера (новые сверху, постраничный вывод).</summary>
        Task<ReviewsPageDto> GetTrainerReviewsAsync(string trainerUserId, int page = 1, int pageSize = 20, CancellationToken ct = default);

        /// <summary>Тренер отвечает на отзыв (один раз).</summary>
        Task<Result<ReviewDto>> ReplyAsync(string trainerUserId, Guid reviewId, ReplyToReviewRequest request, CancellationToken ct = default);

        /// <summary>Агрегированный рейтинг тренера за последние 12 месяцев.</summary>
        Task<TrainerRatingDto> GetRatingAsync(string trainerUserId, CancellationToken ct = default);

        /// <summary>Рейтинги для нескольких тренеров (батч для поиска M2).</summary>
        Task<Dictionary<string, TrainerRatingDto>> GetRatingBatchAsync(IEnumerable<string> trainerUserIds, CancellationToken ct = default);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Trainers.Infrastructure.Services;
using Modules.Users.Domain.Authentication;
using WorkoutLogger.WebApi.Extensions;

namespace WorkoutLogger.WebApi.Controllers
{
    /// <summary>
    /// Отзывы и рейтинг тренеров (M8).
    /// Отзыв доступен только после завершённой оплаченной тренировки.
    /// </summary>
    [ApiController]
    [Route("api/reviews")]
    [Authorize]
    public class ReviewsController(IReviewService reviewService, ICurrentUser currentUser) : ControllerBase
    {
        /// <summary>
        /// Ученик оставляет отзыв на тренера.
        /// Требуется TrainingPayment.Status = Completed (M5).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PostReview([FromBody] PostReviewRequest request, CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await reviewService.PostAsync(userId, request, ct);
            return result.ToActionResult();
        }

        /// <summary>Список отзывов тренера (новые сверху).</summary>
        [HttpGet("trainer/{trainerId}")]
        public async Task<IActionResult> GetTrainerReviews(
            string trainerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var reviews = await reviewService.GetTrainerReviewsAsync(trainerId, page, pageSize, ct);
            return Ok(reviews);
        }

        /// <summary>Рейтинг тренера за последние 12 месяцев.</summary>
        [HttpGet("trainer/{trainerId}/rating")]
        public async Task<IActionResult> GetTrainerRating(string trainerId, CancellationToken ct)
        {
            var rating = await reviewService.GetRatingAsync(trainerId, ct);
            return Ok(rating);
        }

        /// <summary>Тренер отвечает на отзыв (один раз).</summary>
        [HttpPost("{reviewId:guid}/reply")]
        public async Task<IActionResult> Reply(
            Guid reviewId, [FromBody] ReplyToReviewRequest request, CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await reviewService.ReplyAsync(userId, reviewId, request, ct);
            return result.ToActionResult();
        }
    }
}

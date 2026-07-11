using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Results;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    public class ReviewService(TrainersDbContext db) : IReviewService
    {
        private const int RatingWindowMonths = 12;

        public async Task<Result<ReviewDto>> PostAsync(
            string studentUserId, PostReviewRequest request, CancellationToken ct = default)
        {
            if (request.Rating is < 1 or > 5)
                return new Result<ReviewDto>(TrainerErrors.InvalidRating());

            // Проверяем, что платёж существует, принадлежит ученику и завершён.
            var payment = await db.TrainingPayments.FindAsync([request.PaymentId], ct);

            if (payment is null || payment.StudentUserId != studentUserId)
                return new Result<ReviewDto>(TrainerErrors.ReviewRequiresCompletedPayment());

            if (payment.Status != TrainingPaymentStatus.Completed)
                return new Result<ReviewDto>(TrainerErrors.ReviewRequiresCompletedPayment());

            // Один отзыв на один платёж.
            var duplicate = await db.Reviews.AnyAsync(r => r.PaymentId == request.PaymentId, ct);
            if (duplicate)
                return new Result<ReviewDto>(TrainerErrors.ReviewAlreadyExists());

            var review = new Review
            {
                Id = Guid.NewGuid(),
                StudentUserId = studentUserId,
                TrainerUserId = payment.TrainerUserId,
                PaymentId = request.PaymentId,
                Rating = request.Rating,
                Text = request.Text?.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
            };

            db.Reviews.Add(review);
            await db.SaveChangesAsync(ct);
            return new Result<ReviewDto>(ReviewMapper.ToDto(review));
        }

        public async Task<ReviewsPageDto> GetTrainerReviewsAsync(
            string trainerUserId, int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = db.Reviews
                .Where(r => r.TrainerUserId == trainerUserId)
                .OrderByDescending(r => r.CreatedAtUtc);

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new ReviewsPageDto
            {
                Items = items.Select(ReviewMapper.ToDto).ToList(),
                TotalCount = total,
                HasMore = total > page * pageSize,
            };
        }

        public async Task<Result<ReviewDto>> ReplyAsync(
            string trainerUserId, Guid reviewId, ReplyToReviewRequest request, CancellationToken ct = default)
        {
            var review = await db.Reviews.FindAsync([reviewId], ct);

            if (review is null)
                return new Result<ReviewDto>(TrainerErrors.ReviewNotFound());

            if (review.TrainerUserId != trainerUserId)
                return new Result<ReviewDto>(TrainerErrors.ReviewReplyForbidden());

            if (review.TrainerReply is not null)
                return new Result<ReviewDto>(TrainerErrors.ReplyAlreadyExists());

            review.TrainerReply = request.Reply.Trim();
            review.TrainerRepliedAtUtc = DateTime.UtcNow;

            await db.SaveChangesAsync(ct);
            return new Result<ReviewDto>(ReviewMapper.ToDto(review));
        }

        public async Task<TrainerRatingDto> GetRatingAsync(
            string trainerUserId, CancellationToken ct = default)
        {
            var cutoff = DateTime.UtcNow.AddMonths(-RatingWindowMonths);

            var reviews = await db.Reviews
                .Where(r => r.TrainerUserId == trainerUserId && r.CreatedAtUtc >= cutoff)
                .Select(r => r.Rating)
                .ToListAsync(ct);

            return new TrainerRatingDto
            {
                TrainerUserId = trainerUserId,
                AverageRating = reviews.Count > 0 ? reviews.Average() : null,
                ReviewCount = reviews.Count,
            };
        }

        public async Task<Dictionary<string, TrainerRatingDto>> GetRatingBatchAsync(
            IEnumerable<string> trainerUserIds, CancellationToken ct = default)
        {
            var ids = trainerUserIds.Distinct().ToList();
            if (ids.Count == 0) return [];

            var cutoff = DateTime.UtcNow.AddMonths(-RatingWindowMonths);

            var raw = await db.Reviews
                .Where(r => ids.Contains(r.TrainerUserId) && r.CreatedAtUtc >= cutoff)
                .GroupBy(r => r.TrainerUserId)
                .Select(g => new
                {
                    TrainerUserId = g.Key,
                    Average = g.Average(r => (double)r.Rating),
                    Count = g.Count(),
                })
                .ToListAsync(ct);

            var dict = raw.ToDictionary(
                x => x.TrainerUserId,
                x => new TrainerRatingDto
                {
                    TrainerUserId = x.TrainerUserId,
                    AverageRating = x.Average,
                    ReviewCount = x.Count,
                });

            // Тренеры без отзывов — явно добавляем с null-рейтингом.
            foreach (var id in ids.Where(id => !dict.ContainsKey(id)))
                dict[id] = new TrainerRatingDto { TrainerUserId = id, AverageRating = null, ReviewCount = 0 };

            return dict;
        }
    }
}

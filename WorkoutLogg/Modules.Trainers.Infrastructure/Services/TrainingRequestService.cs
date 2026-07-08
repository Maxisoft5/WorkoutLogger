using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Results;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    public class TrainingRequestService(TrainersDbContext dbContext) : ITrainingRequestService
    {
        public async Task<Result<TrainingRequestDto>> CreateAsync(
            string studentUserId, CreateTrainingRequestDto request, CancellationToken ct = default)
        {
            if (request.TrainerUserId == studentUserId)
                return new Result<TrainingRequestDto>(TrainerErrors.CannotRequestSelf());

            if (request.TrainerUserId is not null)
            {
                var trainerExists = await dbContext.TrainerProfiles
                    .AnyAsync(p => p.UserId == request.TrainerUserId && p.IsActive, ct);
                if (!trainerExists)
                    return new Result<TrainingRequestDto>(TrainerErrors.TrainerNotFoundOrInactive());

                var duplicate = await dbContext.TrainingRequests.AnyAsync(r =>
                    r.StudentUserId == studentUserId
                    && r.TrainerUserId == request.TrainerUserId
                    && r.Status == TrainingRequestStatus.Pending, ct);
                if (duplicate)
                    return new Result<TrainingRequestDto>(TrainerErrors.RequestAlreadyPending());
            }
            else
            {
                // Одна открытая заявка в ленте на ученика, чтобы не спамить.
                var openPending = await dbContext.TrainingRequests.AnyAsync(r =>
                    r.StudentUserId == studentUserId
                    && r.TrainerUserId == null
                    && r.Status == TrainingRequestStatus.Pending, ct);
                if (openPending)
                    return new Result<TrainingRequestDto>(TrainerErrors.OpenRequestAlreadyPending());
            }

            var entity = new TrainingRequest
            {
                Id = Guid.NewGuid(),
                StudentUserId = studentUserId,
                TrainerUserId = request.TrainerUserId,
                Goal = request.Goal,
                Level = request.Level,
                Formats = request.Formats,
                Schedule = request.Schedule,
                Budget = request.Budget,
                Message = request.Message,
                Status = TrainingRequestStatus.Pending,
                CreatedAtUtc = DateTime.UtcNow
            };

            dbContext.TrainingRequests.Add(entity);
            await dbContext.SaveChangesAsync(ct);
            return new Result<TrainingRequestDto>(entity.MapRequest());
        }

        public async Task<List<TrainingRequestDto>> GetMyRequestsAsync(
            string studentUserId, CancellationToken ct = default)
        {
            var items = await dbContext.TrainingRequests
                .AsNoTracking()
                .Where(r => r.StudentUserId == studentUserId)
                .OrderByDescending(r => r.CreatedAtUtc)
                .ToListAsync(ct);
            return items.Select(r => r.MapRequest()).ToList();
        }

        public async Task<Result<TrainingRequestDto>> CancelAsync(
            string studentUserId, Guid requestId, CancellationToken ct = default)
        {
            var request = await dbContext.TrainingRequests.FirstOrDefaultAsync(r => r.Id == requestId, ct);
            if (request is null)
                return new Result<TrainingRequestDto>(TrainerErrors.RequestNotFound());

            if (request.StudentUserId != studentUserId)
                return new Result<TrainingRequestDto>(TrainerErrors.NotRequestOwner());

            if (request.Status != TrainingRequestStatus.Pending)
                return new Result<TrainingRequestDto>(TrainerErrors.RequestNotPending());

            request.Status = TrainingRequestStatus.Cancelled;
            request.RespondedAtUtc = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(ct);
            return new Result<TrainingRequestDto>(request.MapRequest());
        }

        public async Task<List<TrainingRequestDto>> GetIncomingAsync(
            string trainerUserId, CancellationToken ct = default)
        {
            var items = await dbContext.TrainingRequests
                .AsNoTracking()
                .Where(r => r.TrainerUserId == trainerUserId && r.Status == TrainingRequestStatus.Pending)
                .OrderByDescending(r => r.CreatedAtUtc)
                .ToListAsync(ct);
            return items.Select(r => r.MapRequest()).ToList();
        }

        public async Task<Result<TrainingRequestsPageDto>> GetOpenFeedAsync(
            string trainerUserId, OpenRequestsFeedFilter filter, CancellationToken ct = default)
        {
            var page = Math.Max(filter.Page, 1);
            var pageSize = Math.Clamp(filter.PageSize, 1, 100);

            var query = dbContext.TrainingRequests
                .AsNoTracking()
                .Where(r => r.TrainerUserId == null && r.Status == TrainingRequestStatus.Pending);

            if (filter.ByMyProfile)
            {
                var profile = await dbContext.TrainerProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.UserId == trainerUserId && p.IsActive, ct);
                if (profile is null)
                    return new Result<TrainingRequestsPageDto>(TrainerErrors.ProfileNotFound());

                var specializations = profile.Specializations;
                query = query.Where(r =>
                    r.Goal == TrainerSpecializations.None
                    || (r.Goal & specializations) != TrainerSpecializations.None);
            }

            if (filter.OnlineOnly)
                query = query.Where(r => (r.Formats & TrainingFormats.Online) != TrainingFormats.None);

            if (filter.BeginnersOnly)
                query = query.Where(r => r.Level == StudentLevel.Beginner);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(r => r.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new Result<TrainingRequestsPageDto>(new TrainingRequestsPageDto
            {
                Items = items.Select(r => r.MapRequest()).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            });
        }

        public async Task<Result<TrainingRequestDto>> AcceptAsync(
            string trainerUserId, Guid requestId, CancellationToken ct = default)
        {
            var request = await dbContext.TrainingRequests.FirstOrDefaultAsync(r => r.Id == requestId, ct);
            if (request is null)
                return new Result<TrainingRequestDto>(TrainerErrors.RequestNotFound());

            if (request.TrainerUserId is not null && request.TrainerUserId != trainerUserId)
                return new Result<TrainingRequestDto>(TrainerErrors.NotRequestTrainer());

            if (request.Status != TrainingRequestStatus.Pending)
                return new Result<TrainingRequestDto>(TrainerErrors.RequestNotPending());

            if (request.StudentUserId == trainerUserId)
                return new Result<TrainingRequestDto>(TrainerErrors.CannotRequestSelf());

            // Открытую заявку из ленты принимает первый откликнувшийся тренер.
            request.TrainerUserId ??= trainerUserId;
            request.Status = TrainingRequestStatus.Accepted;
            request.RespondedAtUtc = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(ct);
            return new Result<TrainingRequestDto>(request.MapRequest());
        }

        public async Task<Result<TrainingRequestDto>> DeclineAsync(
            string trainerUserId, Guid requestId, string? reason, CancellationToken ct = default)
        {
            var request = await dbContext.TrainingRequests.FirstOrDefaultAsync(r => r.Id == requestId, ct);
            if (request is null)
                return new Result<TrainingRequestDto>(TrainerErrors.RequestNotFound());

            if (request.TrainerUserId != trainerUserId)
                return new Result<TrainingRequestDto>(TrainerErrors.NotRequestTrainer());

            if (request.Status != TrainingRequestStatus.Pending)
                return new Result<TrainingRequestDto>(TrainerErrors.RequestNotPending());

            request.Status = TrainingRequestStatus.Declined;
            request.DeclineReason = reason;
            request.RespondedAtUtc = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(ct);
            return new Result<TrainingRequestDto>(request.MapRequest());
        }

        public async Task<List<TrainingRequestDto>> GetMyStudentsAsync(
            string trainerUserId, CancellationToken ct = default)
        {
            var items = await dbContext.TrainingRequests
                .AsNoTracking()
                .Where(r => r.TrainerUserId == trainerUserId && r.Status == TrainingRequestStatus.Accepted)
                .OrderByDescending(r => r.RespondedAtUtc)
                .ToListAsync(ct);
            return items.Select(r => r.MapRequest()).ToList();
        }

        public async Task<TrainerStatsDto> GetStatsAsync(string trainerUserId, CancellationToken ct = default)
        {
            var pending = await dbContext.TrainingRequests.CountAsync(
                r => r.TrainerUserId == trainerUserId && r.Status == TrainingRequestStatus.Pending, ct);
            var students = await dbContext.TrainingRequests
                .Where(r => r.TrainerUserId == trainerUserId && r.Status == TrainingRequestStatus.Accepted)
                .Select(r => r.StudentUserId)
                .Distinct()
                .CountAsync(ct);

            return new TrainerStatsDto
            {
                PendingRequestsCount = pending,
                StudentsCount = students
            };
        }
    }
}

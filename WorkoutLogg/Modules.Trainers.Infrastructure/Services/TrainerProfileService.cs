using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Results;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    public class TrainerProfileService(TrainersDbContext dbContext) : ITrainerProfileService
    {
        public async Task<Result<TrainerProfileDto>> UpsertAsync(
            string userId, UpsertTrainerProfileRequest request, CancellationToken ct = default)
        {
            var validationError = Validate(request);
            if (validationError is not null)
                return new Result<TrainerProfileDto>(validationError);

            var profile = await dbContext.TrainerProfiles.FirstOrDefaultAsync(p => p.UserId == userId, ct);
            if (profile is null)
            {
                profile = new TrainerProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAtUtc = DateTime.UtcNow
                };
                dbContext.TrainerProfiles.Add(profile);
            }
            else
            {
                profile.UpdatedAtUtc = DateTime.UtcNow;
            }

            profile.Specializations = request.Specializations;
            profile.Experience = request.Experience;
            profile.Formats = request.Formats;
            profile.PricePerSession = request.PricePerSession;
            profile.About = request.About;
            profile.IsActive = request.IsActive;

            await dbContext.SaveChangesAsync(ct);
            return new Result<TrainerProfileDto>(profile.MapProfile());
        }

        public async Task<Result<TrainerProfileDto>> GetMyAsync(string userId, CancellationToken ct = default)
        {
            var profile = await dbContext.TrainerProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId, ct);

            if (profile is null)
                return new Result<TrainerProfileDto>(TrainerErrors.ProfileNotFound());

            return new Result<TrainerProfileDto>(profile.MapProfile());
        }

        public async Task<TrainerProfilesPageDto> GetActiveAsync(int page, int pageSize, CancellationToken ct = default)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = dbContext.TrainerProfiles
                .AsNoTracking()
                .Where(p => p.IsActive);

            var totalCount = await query.CountAsync(ct);
            var entities = await query
                .OrderBy(p => p.PricePerSession)
                .ThenBy(p => p.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
            var items = entities.Select(p => p.MapProfile()).ToList();

            return new TrainerProfilesPageDto
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        private static Error? Validate(UpsertTrainerProfileRequest request)
        {
            if (request.Specializations == TrainerSpecializations.None)
                return TrainerErrors.NoSpecializations();

            if (request.Formats == TrainingFormats.None)
                return TrainerErrors.NoFormats();

            if (request.PricePerSession is < TrainerErrors.MinPricePerSession or > TrainerErrors.MaxPricePerSession)
                return TrainerErrors.InvalidPrice();

            return null;
        }
    }
}

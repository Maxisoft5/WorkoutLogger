using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Results;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    public class VerificationService(TrainersDbContext db) : IVerificationService
    {
        public async Task<Result<TrainerVerificationDto>> SubmitAsync(
            string trainerUserId, SubmitVerificationRequest request, CancellationToken ct = default)
        {
            var existing = await db.TrainerVerifications
                .FirstOrDefaultAsync(v => v.TrainerUserId == trainerUserId, ct);

            if (existing is not null)
                return new Result<TrainerVerificationDto>(TrainerErrors.VerificationAlreadyExists());

            foreach (var doc in request.Documents)
            {
                if (!IsValidHttpsUrl(doc.FileUrl))
                    return new Result<TrainerVerificationDto>(TrainerErrors.InvalidFileUrl());
            }

            var now = DateTime.UtcNow;
            var verification = new TrainerVerification
            {
                Id = Guid.NewGuid(),
                TrainerUserId = trainerUserId,
                Status = VerificationStatus.Pending,
                SubmittedAtUtc = now,
            };
            db.TrainerVerifications.Add(verification);

            var documents = request.Documents.Select(d => new VerificationDocument
            {
                Id = Guid.NewGuid(),
                VerificationId = verification.Id,
                Type = d.Type,
                FileName = d.FileName.Trim(),
                FileUrl = d.FileUrl.Trim(),
                UploadedAtUtc = now,
            }).ToList();
            db.VerificationDocuments.AddRange(documents);

            await db.SaveChangesAsync(ct);
            return new Result<TrainerVerificationDto>(VerificationMapper.ToDto(verification, documents));
        }

        public async Task<Result<TrainerVerificationDto>> AddDocumentAsync(
            string trainerUserId, AddDocumentRequest request, CancellationToken ct = default)
        {
            if (!IsValidHttpsUrl(request.FileUrl))
                return new Result<TrainerVerificationDto>(TrainerErrors.InvalidFileUrl());

            var (verification, error) = await LoadPendingVerificationAsync(trainerUserId, ct);
            if (error is not null) return new Result<TrainerVerificationDto>(error);

            var doc = new VerificationDocument
            {
                Id = Guid.NewGuid(),
                VerificationId = verification!.Id,
                Type = request.Type,
                FileName = request.FileName.Trim(),
                FileUrl = request.FileUrl.Trim(),
                UploadedAtUtc = DateTime.UtcNow,
            };
            db.VerificationDocuments.Add(doc);
            await db.SaveChangesAsync(ct);

            var docs = await LoadDocumentsAsync(verification.Id, ct);
            return new Result<TrainerVerificationDto>(VerificationMapper.ToDto(verification, docs));
        }

        public async Task<Result<TrainerVerificationDto>> RemoveDocumentAsync(
            string trainerUserId, Guid documentId, CancellationToken ct = default)
        {
            var (verification, error) = await LoadPendingVerificationAsync(trainerUserId, ct);
            if (error is not null) return new Result<TrainerVerificationDto>(error);

            var doc = await db.VerificationDocuments.FindAsync([documentId], ct);
            if (doc is null)
                return new Result<TrainerVerificationDto>(TrainerErrors.DocumentNotFound());

            if (doc.VerificationId != verification!.Id)
                return new Result<TrainerVerificationDto>(TrainerErrors.DocumentBelongsToAnotherVerification());

            db.VerificationDocuments.Remove(doc);
            await db.SaveChangesAsync(ct);

            var docs = await LoadDocumentsAsync(verification.Id, ct);
            return new Result<TrainerVerificationDto>(VerificationMapper.ToDto(verification, docs));
        }

        public async Task<Result<TrainerVerificationDto>> GetMyVerificationAsync(
            string trainerUserId, CancellationToken ct = default)
        {
            var verification = await db.TrainerVerifications
                .FirstOrDefaultAsync(v => v.TrainerUserId == trainerUserId, ct);

            if (verification is null)
                return new Result<TrainerVerificationDto>(TrainerErrors.VerificationNotFound());

            var docs = await LoadDocumentsAsync(verification.Id, ct);
            return new Result<TrainerVerificationDto>(VerificationMapper.ToDto(verification, docs));
        }

        public async Task<Result<TrainerVerificationDto>> ReviewAsync(
            string moderatorUserId, Guid verificationId, ReviewVerificationRequest request, CancellationToken ct = default)
        {
            var verification = await db.TrainerVerifications.FindAsync([verificationId], ct);
            if (verification is null)
                return new Result<TrainerVerificationDto>(TrainerErrors.VerificationNotFound());

            if (verification.Status != VerificationStatus.Pending)
                return new Result<TrainerVerificationDto>(TrainerErrors.VerificationNotPending());

            var now = DateTime.UtcNow;
            verification.Status = request.Approved ? VerificationStatus.Approved : VerificationStatus.Rejected;
            verification.ModeratorComment = request.Comment?.Trim();
            verification.ReviewedAtUtc = now;
            verification.ReviewedByUserId = moderatorUserId;

            if (request.Approved)
            {
                verification.Badge = request.Badge ?? VerificationBadge.Verified;

                // Отражаем верификацию на карточке тренера — выставляем бейдж через HasVerifiedBadge.
                var profile = await db.TrainerProfiles
                    .FirstOrDefaultAsync(p => p.UserId == verification.TrainerUserId, ct);
                if (profile is not null)
                {
                    profile.HasVerifiedBadge = true;
                    profile.VerificationBadge = verification.Badge;
                }
            }

            await db.SaveChangesAsync(ct);

            var docs = await LoadDocumentsAsync(verification.Id, ct);
            return new Result<TrainerVerificationDto>(VerificationMapper.ToDto(verification, docs));
        }

        public async Task<List<TrainerVerificationDto>> GetPendingAsync(CancellationToken ct = default)
        {
            var verifications = await db.TrainerVerifications
                .Where(v => v.Status == VerificationStatus.Pending)
                .OrderBy(v => v.SubmittedAtUtc)
                .ToListAsync(ct);

            var ids = verifications.Select(v => v.Id).ToList();
            var allDocs = await db.VerificationDocuments
                .Where(d => ids.Contains(d.VerificationId))
                .ToListAsync(ct);

            var docsByVerification = allDocs.GroupBy(d => d.VerificationId)
                .ToDictionary(g => g.Key, g => g.ToList());

            return verifications
                .Select(v => VerificationMapper.ToDto(v,
                    docsByVerification.GetValueOrDefault(v.Id, [])))
                .ToList();
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private async Task<(TrainerVerification? verification, Error? error)>
            LoadPendingVerificationAsync(string trainerUserId, CancellationToken ct)
        {
            var verification = await db.TrainerVerifications
                .FirstOrDefaultAsync(v => v.TrainerUserId == trainerUserId, ct);

            if (verification is null)
                return (null, TrainerErrors.VerificationNotFound());

            if (verification.Status != VerificationStatus.Pending)
                return (null, TrainerErrors.VerificationNotPending());

            return (verification, null);
        }

        private Task<List<VerificationDocument>> LoadDocumentsAsync(Guid verificationId, CancellationToken ct) =>
            db.VerificationDocuments
                .Where(d => d.VerificationId == verificationId)
                .OrderBy(d => d.UploadedAtUtc)
                .ToListAsync(ct);

        private static bool IsValidHttpsUrl(string url) =>
            Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme == "https";
    }
}

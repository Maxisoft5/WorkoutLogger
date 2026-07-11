using System.ComponentModel.DataAnnotations;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    // ─── Requests ────────────────────────────────────────────────────────────

    public class SubmitVerificationRequest
    {
        /// <summary>Список документов для первичной подачи.</summary>
        public List<AddDocumentRequest> Documents { get; set; } = [];
    }

    public class AddDocumentRequest
    {
        [Required]
        public DocumentType Type { get; set; }

        [Required]
        [MaxLength(500)]
        public string FileName { get; set; } = null!;

        /// <summary>Прямая HTTPS-ссылка на загруженный файл.</summary>
        [Required]
        [MaxLength(2048)]
        public string FileUrl { get; set; } = null!;
    }

    public class ReviewVerificationRequest
    {
        [Required]
        public bool Approved { get; set; }

        [MaxLength(2000)]
        public string? Comment { get; set; }

        /// <summary>Бейдж при одобрении (Verified по умолчанию).</summary>
        public VerificationBadge? Badge { get; set; }
    }

    // ─── DTOs ────────────────────────────────────────────────────────────────

    public class VerificationDocumentDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public DateTime UploadedAtUtc { get; set; }
    }

    public class TrainerVerificationDto
    {
        public Guid Id { get; set; }
        public string TrainerUserId { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? ModeratorComment { get; set; }
        public string? Badge { get; set; }
        public DateTime SubmittedAtUtc { get; set; }
        public DateTime? ReviewedAtUtc { get; set; }
        public List<VerificationDocumentDto> Documents { get; set; } = [];
    }

    internal static class VerificationMapper
    {
        public static VerificationDocumentDto ToDto(VerificationDocument d) => new()
        {
            Id = d.Id,
            Type = d.Type.ToString(),
            FileName = d.FileName,
            FileUrl = d.FileUrl,
            UploadedAtUtc = d.UploadedAtUtc,
        };

        public static TrainerVerificationDto ToDto(TrainerVerification v, List<VerificationDocument> docs) => new()
        {
            Id = v.Id,
            TrainerUserId = v.TrainerUserId,
            Status = v.Status.ToString(),
            ModeratorComment = v.ModeratorComment,
            Badge = v.Badge?.ToString(),
            SubmittedAtUtc = v.SubmittedAtUtc,
            ReviewedAtUtc = v.ReviewedAtUtc,
            Documents = docs.Select(ToDto).ToList(),
        };
    }
}

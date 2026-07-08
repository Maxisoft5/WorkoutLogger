using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    /// <summary>Запрос создания/обновления собственной карточки тренера.</summary>
    public class UpsertTrainerProfileRequest
    {
        public TrainerSpecializations Specializations { get; set; }
        public ExperienceRange Experience { get; set; }
        public TrainingFormats Formats { get; set; }

        /// <summary>Цена одной тренировки в FitCoins.</summary>
        public int PricePerSession { get; set; }

        public string? About { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class TrainerProfileDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public TrainerSpecializations Specializations { get; set; }
        public ExperienceRange Experience { get; set; }
        public TrainingFormats Formats { get; set; }
        public int PricePerSession { get; set; }
        public string? About { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }

    public class TrainerProfilesPageDto
    {
        public List<TrainerProfileDto> Items { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public static class TrainerProfileMapper
    {
        public static TrainerProfileDto MapProfile(this TrainerProfile profile) => new()
        {
            Id = profile.Id,
            UserId = profile.UserId,
            Specializations = profile.Specializations,
            Experience = profile.Experience,
            Formats = profile.Formats,
            PricePerSession = profile.PricePerSession,
            About = profile.About,
            IsActive = profile.IsActive,
            CreatedAtUtc = profile.CreatedAtUtc,
            UpdatedAtUtc = profile.UpdatedAtUtc
        };
    }
}

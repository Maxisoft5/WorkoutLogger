using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    /// <summary>Создание заявки учеником: адресной (TrainerUserId задан) или открытой в ленту.</summary>
    public class CreateTrainingRequestDto
    {
        public string? TrainerUserId { get; set; }
        public TrainerSpecializations Goal { get; set; }
        public StudentLevel Level { get; set; }
        public TrainingFormats Formats { get; set; }
        public string? Schedule { get; set; }
        public int? Budget { get; set; }
        public string? Message { get; set; }
    }

    public class TrainingRequestDto
    {
        public Guid Id { get; set; }
        public string StudentUserId { get; set; } = null!;
        public string? TrainerUserId { get; set; }
        public TrainerSpecializations Goal { get; set; }
        public StudentLevel Level { get; set; }
        public TrainingFormats Formats { get; set; }
        public string? Schedule { get; set; }
        public int? Budget { get; set; }
        public string? Message { get; set; }
        public TrainingRequestStatus Status { get; set; }
        public string? DeclineReason { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? RespondedAtUtc { get; set; }
    }

    /// <summary>Фильтры ленты «Ищут тренера сейчас» (чипы в дизайне).</summary>
    public class OpenRequestsFeedFilter
    {
        /// <summary>«По моему профилю» — цель заявки пересекается со специализациями тренера.</summary>
        public bool ByMyProfile { get; set; }

        /// <summary>«Онлайн» — ученик готов заниматься онлайн.</summary>
        public bool OnlineOnly { get; set; }

        /// <summary>«Новички» — уровень ученика Beginner.</summary>
        public bool BeginnersOnly { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class TrainingRequestsPageDto
    {
        public List<TrainingRequestDto> Items { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>Блок статистики тренера на экране 03 (рейтинг появится в M8).</summary>
    public class TrainerStatsDto
    {
        public int PendingRequestsCount { get; set; }
        public int StudentsCount { get; set; }
    }

    public static class TrainingRequestMapper
    {
        public static TrainingRequestDto MapRequest(this TrainingRequest request) => new()
        {
            Id = request.Id,
            StudentUserId = request.StudentUserId,
            TrainerUserId = request.TrainerUserId,
            Goal = request.Goal,
            Level = request.Level,
            Formats = request.Formats,
            Schedule = request.Schedule,
            Budget = request.Budget,
            Message = request.Message,
            Status = request.Status,
            DeclineReason = request.DeclineReason,
            CreatedAtUtc = request.CreatedAtUtc,
            RespondedAtUtc = request.RespondedAtUtc
        };
    }
}

using Modules.Common.Domain.Results;

namespace Modules.Trainers.Infrastructure.Services
{
    public interface ITrainingRequestService
    {
        /// <summary>Ученик создаёт заявку: адресную конкретному тренеру или открытую в ленту.</summary>
        Task<Result<TrainingRequestDto>> CreateAsync(string studentUserId, CreateTrainingRequestDto request, CancellationToken ct = default);

        /// <summary>Заявки текущего ученика (все статусы, новые сверху).</summary>
        Task<List<TrainingRequestDto>> GetMyRequestsAsync(string studentUserId, CancellationToken ct = default);

        /// <summary>Ученик отменяет свою ожидающую заявку.</summary>
        Task<Result<TrainingRequestDto>> CancelAsync(string studentUserId, Guid requestId, CancellationToken ct = default);

        /// <summary>Входящие ожидающие заявки тренера (блок «Заявки» на экране 03).</summary>
        Task<List<TrainingRequestDto>> GetIncomingAsync(string trainerUserId, CancellationToken ct = default);

        /// <summary>Лента «Ищут тренера сейчас»: открытые заявки с фильтрами-чипами.</summary>
        Task<Result<TrainingRequestsPageDto>> GetOpenFeedAsync(string trainerUserId, OpenRequestsFeedFilter filter, CancellationToken ct = default);

        /// <summary>Тренер принимает заявку (адресную себе или открытую из ленты).</summary>
        Task<Result<TrainingRequestDto>> AcceptAsync(string trainerUserId, Guid requestId, CancellationToken ct = default);

        /// <summary>Тренер отклоняет адресную заявку (опционально с причиной).</summary>
        Task<Result<TrainingRequestDto>> DeclineAsync(string trainerUserId, Guid requestId, string? reason, CancellationToken ct = default);

        /// <summary>Принятые ученики тренера (вкладка «Мои ученики»).</summary>
        Task<List<TrainingRequestDto>> GetMyStudentsAsync(string trainerUserId, CancellationToken ct = default);

        /// <summary>Статистика тренера: заявки в ожидании и количество учеников.</summary>
        Task<TrainerStatsDto> GetStatsAsync(string trainerUserId, CancellationToken ct = default);
    }
}

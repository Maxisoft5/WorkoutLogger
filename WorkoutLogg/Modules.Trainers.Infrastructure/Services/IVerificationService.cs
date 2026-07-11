using Modules.Common.Domain.Results;

namespace Modules.Trainers.Infrastructure.Services
{
    public interface IVerificationService
    {
        /// <summary>Тренер подаёт заявку на верификацию с документами.</summary>
        Task<Result<TrainerVerificationDto>> SubmitAsync(string trainerUserId, SubmitVerificationRequest request, CancellationToken ct = default);

        /// <summary>Тренер добавляет документ к существующей pending-заявке.</summary>
        Task<Result<TrainerVerificationDto>> AddDocumentAsync(string trainerUserId, AddDocumentRequest request, CancellationToken ct = default);

        /// <summary>Тренер удаляет документ из своей pending-заявки.</summary>
        Task<Result<TrainerVerificationDto>> RemoveDocumentAsync(string trainerUserId, Guid documentId, CancellationToken ct = default);

        /// <summary>Текущий статус верификации тренера.</summary>
        Task<Result<TrainerVerificationDto>> GetMyVerificationAsync(string trainerUserId, CancellationToken ct = default);

        /// <summary>Модератор одобряет или отклоняет заявку.</summary>
        Task<Result<TrainerVerificationDto>> ReviewAsync(string moderatorUserId, Guid verificationId, ReviewVerificationRequest request, CancellationToken ct = default);

        /// <summary>Список заявок в статусе Pending (для модераторской очереди).</summary>
        Task<List<TrainerVerificationDto>> GetPendingAsync(CancellationToken ct = default);
    }
}

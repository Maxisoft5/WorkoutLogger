using Modules.Common.Domain.Results;

namespace Modules.Trainers.Infrastructure.Services
{
    public interface ITrainerProfileService
    {
        /// <summary>Создаёт или обновляет карточку тренера текущего пользователя.</summary>
        Task<Result<TrainerProfileDto>> UpsertAsync(string userId, UpsertTrainerProfileRequest request, CancellationToken ct = default);

        /// <summary>Возвращает карточку тренера текущего пользователя.</summary>
        Task<Result<TrainerProfileDto>> GetMyAsync(string userId, CancellationToken ct = default);

        /// <summary>Постраничный список активных тренеров для вкладки «Тренеры» ученика.</summary>
        Task<TrainerProfilesPageDto> GetActiveAsync(int page, int pageSize, CancellationToken ct = default);
    }
}

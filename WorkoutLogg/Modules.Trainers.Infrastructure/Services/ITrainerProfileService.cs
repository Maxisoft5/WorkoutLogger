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

        /// <summary>
        /// Поиск активных тренеров с фильтрами и match-скором (экран 02).
        /// Предпочтения ученика влияют только на скор и сортировку, фильтры — на выборку.
        /// </summary>
        Task<TrainerSearchPageDto> SearchAsync(TrainerSearchRequest request, StudentPreferences preferences, CancellationToken ct = default);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Trainers.Infrastructure.Services;
using WorkoutLogger.WebApi.Extensions;
using WorkoutLogger.WebApi.Services;

namespace WorkoutLogger.WebApi.Controllers
{
    /// <summary>
    /// Маркетплейс тренеров (M1): карточка тренера и базовый список активных тренеров.
    /// Поиск с фильтрами и match-скором — отдельный этап (M2).
    /// </summary>
    [ApiController]
    [Route("api/trainers")]
    [Authorize]
    public class TrainersController : ControllerBase
    {
        private readonly ITrainerProfileService _trainerProfileService;
        private readonly ICurrentUser _currentUser;

        public TrainersController(ITrainerProfileService trainerProfileService, ICurrentUser currentUser)
        {
            _trainerProfileService = trainerProfileService;
            _currentUser = currentUser;
        }

        /// <summary>Создать или обновить собственную карточку тренера.</summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpsertMyProfile(
            [FromBody] UpsertTrainerProfileRequest request, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await _trainerProfileService.UpsertAsync(userId, request, ct);
            return result.ToActionResult();
        }

        /// <summary>Получить собственную карточку тренера.</summary>
        [HttpGet("profile/me")]
        public async Task<IActionResult> GetMyProfile(CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await _trainerProfileService.GetMyAsync(userId, ct);
            return result.ToActionResult();
        }

        /// <summary>Постраничный список активных тренеров.</summary>
        [HttpGet]
        public async Task<IActionResult> GetTrainers(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var result = await _trainerProfileService.GetActiveAsync(page, pageSize, ct);
            return Ok(result);
        }
    }
}

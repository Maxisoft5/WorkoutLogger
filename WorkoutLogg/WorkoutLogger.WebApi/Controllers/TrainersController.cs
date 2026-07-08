using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Trainers.Infrastructure.Services;
using Modules.Users.Domain.Authentication;
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
        private readonly IUserService _userService;
        private readonly ICurrentUser _currentUser;

        public TrainersController(
            ITrainerProfileService trainerProfileService,
            IUserService userService,
            ICurrentUser currentUser)
        {
            _trainerProfileService = trainerProfileService;
            _userService = userService;
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

        /// <summary>
        /// Поиск тренеров с фильтрами и match-скором (экран 02, bottom-sheet «Фильтры»).
        /// Если специализации в фильтре не заданы, предпочтения для скора берутся из целей ученика.
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] TrainerSearchRequest request, CancellationToken ct)
        {
            var preferences = await BuildPreferencesAsync(request);
            var result = await _trainerProfileService.SearchAsync(request, preferences, ct);
            return Ok(result);
        }

        /// <summary>Блок «Подобрано для вас»: топ тренеров по match-скору из целей ученика.</summary>
        [HttpGet("recommended")]
        public async Task<IActionResult> GetRecommended([FromQuery] int top = 3, CancellationToken ct = default)
        {
            var request = new TrainerSearchRequest
            {
                SortBy = TrainerSortBy.Match,
                Page = 1,
                PageSize = Math.Clamp(top, 1, 10)
            };
            var preferences = await BuildPreferencesAsync(request);
            var result = await _trainerProfileService.SearchAsync(request, preferences, ct);
            return Ok(result.Items);
        }

        private async Task<StudentPreferences> BuildPreferencesAsync(TrainerSearchRequest request)
        {
            var preferences = new StudentPreferences
            {
                DesiredSpecializations = request.Specializations,
                DesiredFormats = request.Formats,
                Budget = request.PriceMax
            };

            // Фильтры не заданы — используем цели ученика из онбординга («Подобрано для вас»).
            if (preferences.DesiredSpecializations == Modules.Trainers.Infrastructure.Domain.TrainerSpecializations.None
                && _currentUser.Email is not null)
            {
                var user = await _userService.GetUserByEmail(_currentUser.Email);
                if (user.IsSuccess && user.Value?.Goals is not null)
                {
                    preferences.DesiredSpecializations = TrainerMatchCalculator.MapGoalsToSpecializations(
                        user.Value.Goals.Select(g => g.Goal));
                }
            }

            return preferences;
        }
    }
}

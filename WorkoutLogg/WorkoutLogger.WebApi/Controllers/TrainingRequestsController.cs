using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Trainers.Infrastructure.Services;
using WorkoutLogger.WebApi.Extensions;
using WorkoutLogger.WebApi.Services;

namespace WorkoutLogger.WebApi.Controllers
{
    /// <summary>
    /// Заявки учеников на тренировки (M3, экран 03 «Тренер: вкладка Ученики»):
    /// входящие заявки с Принять/Отклонить, лента «Ищут тренера сейчас», статистика.
    /// </summary>
    [ApiController]
    [Route("api/trainers/requests")]
    [Authorize]
    public class TrainingRequestsController : ControllerBase
    {
        private readonly ITrainingRequestService _requestService;
        private readonly ICurrentUser _currentUser;

        public TrainingRequestsController(ITrainingRequestService requestService, ICurrentUser currentUser)
        {
            _requestService = requestService;
            _currentUser = currentUser;
        }

        /// <summary>Ученик отправляет заявку тренеру (или открытую в ленту, если trainerUserId не задан).</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTrainingRequestDto request, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await _requestService.CreateAsync(userId, request, ct);
            return result.ToActionResult();
        }

        /// <summary>Заявки текущего ученика.</summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMy(CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            return Ok(await _requestService.GetMyRequestsAsync(userId, ct));
        }

        /// <summary>Ученик отменяет свою ожидающую заявку.</summary>
        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await _requestService.CancelAsync(userId, id, ct);
            return result.ToActionResult();
        }

        /// <summary>Входящие ожидающие заявки тренера.</summary>
        [HttpGet("incoming")]
        public async Task<IActionResult> GetIncoming(CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            return Ok(await _requestService.GetIncomingAsync(userId, ct));
        }

        /// <summary>Лента «Ищут тренера сейчас» с фильтрами: по моему профилю / онлайн / новички.</summary>
        [HttpGet("open")]
        public async Task<IActionResult> GetOpenFeed([FromQuery] OpenRequestsFeedFilter filter, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await _requestService.GetOpenFeedAsync(userId, filter, ct);
            return result.ToActionResult();
        }

        /// <summary>Тренер принимает заявку.</summary>
        [HttpPost("{id:guid}/accept")]
        public async Task<IActionResult> Accept(Guid id, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await _requestService.AcceptAsync(userId, id, ct);
            return result.ToActionResult();
        }

        /// <summary>Тренер отклоняет заявку (опционально с причиной).</summary>
        [HttpPost("{id:guid}/decline")]
        public async Task<IActionResult> Decline(Guid id, [FromBody] DeclineRequestDto? body, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await _requestService.DeclineAsync(userId, id, body?.Reason, ct);
            return result.ToActionResult();
        }

        /// <summary>Принятые ученики тренера.</summary>
        [HttpGet("students")]
        public async Task<IActionResult> GetMyStudents(CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            return Ok(await _requestService.GetMyStudentsAsync(userId, ct));
        }

        /// <summary>Статистика тренера для экрана 03: заявки в ожидании / ученики.</summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats(CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            return Ok(await _requestService.GetStatsAsync(userId, ct));
        }
    }

    public class DeclineRequestDto
    {
        public string? Reason { get; set; }
    }
}

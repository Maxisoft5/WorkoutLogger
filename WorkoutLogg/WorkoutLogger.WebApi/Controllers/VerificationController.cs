using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Trainers.Infrastructure.Services;
using Modules.Users.Domain.Authentication;
using WorkoutLogger.WebApi.Extensions;

namespace WorkoutLogger.WebApi.Controllers
{
    /// <summary>
    /// Верификация тренеров (M9): загрузка документов, ручная модерация, бейджи «Проверен»/«КМС».
    /// </summary>
    [ApiController]
    [Route("api/verification")]
    [Authorize]
    public class VerificationController(IVerificationService verificationService, ICurrentUser currentUser) : ControllerBase
    {
        /// <summary>Тренер подаёт заявку на верификацию с документами.</summary>
        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] SubmitVerificationRequest request, CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await verificationService.SubmitAsync(userId, request, ct);
            return result.ToActionResult();
        }

        /// <summary>Текущий статус верификации тренера.</summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMy(CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await verificationService.GetMyVerificationAsync(userId, ct);
            return result.ToActionResult();
        }

        /// <summary>Тренер добавляет документ к pending-заявке.</summary>
        [HttpPost("documents")]
        public async Task<IActionResult> AddDocument([FromBody] AddDocumentRequest request, CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await verificationService.AddDocumentAsync(userId, request, ct);
            return result.ToActionResult();
        }

        /// <summary>Тренер удаляет документ из своей pending-заявки.</summary>
        [HttpDelete("documents/{documentId:guid}")]
        public async Task<IActionResult> RemoveDocument(Guid documentId, CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await verificationService.RemoveDocumentAsync(userId, documentId, ct);
            return result.ToActionResult();
        }

        // ─── Модераторские эндпоинты ──────────────────────────────────────────

        /// <summary>
        /// Список заявок, ожидающих проверки (для очереди модератора).
        /// MVP: доступен любому авторизованному пользователю; в продакшене — ограничить ролью Admin.
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending(CancellationToken ct)
        {
            var verifications = await verificationService.GetPendingAsync(ct);
            return Ok(verifications);
        }

        /// <summary>Модератор одобряет или отклоняет заявку.</summary>
        [HttpPost("{verificationId:guid}/review")]
        public async Task<IActionResult> ReviewVerification(
            Guid verificationId, [FromBody] ReviewVerificationRequest request, CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await verificationService.ReviewAsync(userId, verificationId, request, ct);
            return result.ToActionResult();
        }
    }
}

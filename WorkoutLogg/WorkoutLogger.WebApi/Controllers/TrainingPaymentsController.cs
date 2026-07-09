using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Trainers.Infrastructure.Services;
using WorkoutLogger.WebApi.Extensions;
using WorkoutLogger.WebApi.Services;

namespace WorkoutLogger.WebApi.Controllers
{
    /// <summary>
    /// Оплата тренировок FitCoins с эскроу (M5): ученик платит — FC удерживаются,
    /// после подтверждения тренировки учеником тренер получает выплату минус комиссия 10%,
    /// тренер может вернуть оплату, если тренировка не состоится.
    /// </summary>
    [ApiController]
    [Route("api/trainers/payments")]
    [Authorize]
    public class TrainingPaymentsController : ControllerBase
    {
        private readonly ITrainingPaymentService _paymentService;
        private readonly ICurrentUser _currentUser;

        public TrainingPaymentsController(ITrainingPaymentService paymentService, ICurrentUser currentUser)
        {
            _paymentService = paymentService;
            _currentUser = currentUser;
        }

        /// <summary>Ученик оплачивает тренировку у принявшего его тренера.</summary>
        [HttpPost]
        public async Task<IActionResult> Pay([FromBody] PayForTrainingRequest request, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await _paymentService.PayAsync(userId, request.TrainerUserId, ct);
            return result.ToActionResult();
        }

        /// <summary>Ученик подтверждает проведённую тренировку — тренеру уходит выплата.</summary>
        [HttpPost("{id:guid}/complete")]
        public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await _paymentService.CompleteAsync(userId, id, ct);
            return result.ToActionResult();
        }

        /// <summary>Тренер возвращает оплату ученику.</summary>
        [HttpPost("{id:guid}/refund")]
        public async Task<IActionResult> Refund(Guid id, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await _paymentService.RefundAsync(userId, id, ct);
            return result.ToActionResult();
        }

        /// <summary>Мои оплаты (ученик).</summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMy(CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            return Ok(await _paymentService.GetMyPaymentsAsync(userId, ct));
        }

        /// <summary>Полученные оплаты (тренер).</summary>
        [HttpGet("received")]
        public async Task<IActionResult> GetReceived(CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            return Ok(await _paymentService.GetReceivedPaymentsAsync(userId, ct));
        }
    }

    public class PayForTrainingRequest
    {
        public string TrainerUserId { get; set; } = null!;
    }
}

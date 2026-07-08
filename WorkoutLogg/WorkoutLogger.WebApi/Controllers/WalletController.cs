using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Trainers.Infrastructure.Domain;
using Modules.Trainers.Infrastructure.Services;
using Modules.Users.Domain.Authentication;
using WorkoutLogger.WebApi.Extensions;
using WorkoutLogger.WebApi.Services;

namespace WorkoutLogger.WebApi.Controllers
{
    /// <summary>
    /// Кошелёк FitCoins (M4, экран 04 «Профиль»): баланс, история операций,
    /// бонус за серию тренировок. В MVP FitCoins только зарабатываются —
    /// пополнение за деньги отложено (IAP-риски, см. отчёт-анализ).
    /// Бонусы за челленджи и рефералов подключатся вместе с этими системами.
    /// </summary>
    [ApiController]
    [Route("api/wallet")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly IUserService _userService;
        private readonly ICurrentUser _currentUser;

        public WalletController(IWalletService walletService, IUserService userService, ICurrentUser currentUser)
        {
            _walletService = walletService;
            _userService = userService;
            _currentUser = currentUser;
        }

        /// <summary>Баланс кошелька текущего пользователя.</summary>
        [HttpGet]
        public async Task<IActionResult> GetWallet(CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            return Ok(await _walletService.GetWalletAsync(userId, ct));
        }

        /// <summary>История операций (новые сверху).</summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            return Ok(await _walletService.GetHistoryAsync(userId, page, pageSize, ct));
        }

        /// <summary>Забрать бонус «+50 за серию 7 дней» — серия проверяется по журналу тренировок.</summary>
        [HttpPost("rewards/streak")]
        public async Task<IActionResult> ClaimStreakBonus(CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            var since = DateTime.UtcNow.Date.AddDays(-(RewardAmounts.StreakLengthDays - 1));
            var workoutDates = await _userService.GetWorkoutDatesAsync(userId, since);

            var result = await _walletService.ClaimStreakBonusAsync(userId, workoutDates, ct: ct);
            return result.ToActionResult();
        }
    }
}

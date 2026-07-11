using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Trainers.Infrastructure.Services;
using WorkoutLogger.WebApi.Extensions;
using WorkoutLogger.WebApi.Services;

namespace WorkoutLogger.WebApi.Controllers
{
    /// <summary>
    /// Расписание тренера и бронирование слотов (M7).
    /// Тренер управляет слотами; ученик бронирует, тренер подтверждает.
    /// </summary>
    [ApiController]
    [Route("api/schedule")]
    [Authorize]
    public class ScheduleController(IScheduleService scheduleService, ICurrentUser currentUser) : ControllerBase
    {
        // ─── Слоты ────────────────────────────────────────────────────────────

        /// <summary>Тренер добавляет слот в расписание.</summary>
        [HttpPost("slots")]
        public async Task<IActionResult> AddSlot([FromBody] CreateSlotRequest request, CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await scheduleService.AddSlotAsync(userId, request, ct);
            return result.ToActionResult();
        }

        /// <summary>
        /// Список свободных слотов тренера для ученика.
        /// from/to — ISO-8601 UTC; по умолчанию: сегодня + 30 дней.
        /// </summary>
        [HttpGet("slots")]
        public async Task<IActionResult> GetAvailableSlots(
            [FromQuery] string trainerId,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            CancellationToken ct = default)
        {
            var fromUtc = from ?? DateTime.UtcNow;
            var toUtc = to ?? DateTime.UtcNow.AddDays(30);

            var slots = await scheduleService.GetAvailableSlotsAsync(trainerId, fromUtc, toUtc, ct);
            return Ok(slots);
        }

        /// <summary>Личное расписание тренера (все слоты, включая занятые).</summary>
        [HttpGet("slots/my")]
        public async Task<IActionResult> GetMySchedule(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            CancellationToken ct = default)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var fromUtc = from ?? DateTime.UtcNow;
            var toUtc = to ?? DateTime.UtcNow.AddDays(30);

            var slots = await scheduleService.GetMyScheduleAsync(userId, fromUtc, toUtc, ct);
            return Ok(slots);
        }

        /// <summary>Тренер удаляет незабронированный слот.</summary>
        [HttpDelete("slots/{slotId:guid}")]
        public async Task<IActionResult> DeleteSlot(Guid slotId, CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await scheduleService.DeleteSlotAsync(userId, slotId, ct);
            return result.ToActionResult();
        }

        // ─── Бронирования ─────────────────────────────────────────────────────

        /// <summary>Ученик бронирует слот.</summary>
        [HttpPost("bookings")]
        public async Task<IActionResult> Book([FromBody] CreateBookingRequest request, CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await scheduleService.BookAsync(userId, request, ct);
            return result.ToActionResult();
        }

        /// <summary>Бронирования текущего ученика.</summary>
        [HttpGet("bookings/my")]
        public async Task<IActionResult> GetMyBookings(CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var bookings = await scheduleService.GetStudentBookingsAsync(userId, ct);
            return Ok(bookings);
        }

        /// <summary>Бронирования для тренера (входящие).</summary>
        [HttpGet("bookings/trainer")]
        public async Task<IActionResult> GetTrainerBookings(CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var bookings = await scheduleService.GetTrainerBookingsAsync(userId, ct);
            return Ok(bookings);
        }

        /// <summary>Тренер подтверждает бронирование.</summary>
        [HttpPost("bookings/{bookingId:guid}/confirm")]
        public async Task<IActionResult> Confirm(Guid bookingId, CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await scheduleService.ConfirmAsync(userId, bookingId, ct);
            return result.ToActionResult();
        }

        /// <summary>Отмена бронирования (учеником или тренером).</summary>
        [HttpPost("bookings/{bookingId:guid}/cancel")]
        public async Task<IActionResult> Cancel(
            Guid bookingId, [FromBody] CancelBookingRequest request, CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await scheduleService.CancelAsync(userId, bookingId, request, ct);
            return result.ToActionResult();
        }

        /// <summary>Тренер отмечает тренировку состоявшейся.</summary>
        [HttpPost("bookings/{bookingId:guid}/complete")]
        public async Task<IActionResult> Complete(Guid bookingId, CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await scheduleService.CompleteAsync(userId, bookingId, ct);
            return result.ToActionResult();
        }

        /// <summary>Тренер отмечает no-show (ученик не явился).</summary>
        [HttpPost("bookings/{bookingId:guid}/no-show")]
        public async Task<IActionResult> NoShow(Guid bookingId, CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await scheduleService.MarkNoShowAsync(userId, bookingId, ct);
            return result.ToActionResult();
        }
    }
}

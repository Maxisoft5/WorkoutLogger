using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Results;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    public class ScheduleService(TrainersDbContext db) : IScheduleService
    {
        private const int MinDurationMinutes = 15;
        private const int MaxDurationMinutes = 8 * 60;

        // ─── Слоты ────────────────────────────────────────────────────────────

        public async Task<Result<SlotDto>> AddSlotAsync(
            string trainerUserId, CreateSlotRequest request, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            if (request.StartUtc < now)
                return new Result<SlotDto>(TrainerErrors.SlotInPast());

            if (request.EndUtc <= request.StartUtc)
                return new Result<SlotDto>(TrainerErrors.SlotEndBeforeStart());

            var durationMinutes = (int)(request.EndUtc - request.StartUtc).TotalMinutes;

            if (durationMinutes < MinDurationMinutes)
                return new Result<SlotDto>(TrainerErrors.SlotTooShort());

            if (durationMinutes > MaxDurationMinutes)
                return new Result<SlotDto>(TrainerErrors.SlotTooLong());

            var slot = new AvailabilitySlot
            {
                Id = Guid.NewGuid(),
                TrainerUserId = trainerUserId,
                StartUtc = request.StartUtc,
                EndUtc = request.EndUtc,
                DurationMinutes = durationMinutes,
                Note = request.Note?.Trim(),
                IsBooked = false,
                CreatedAtUtc = now,
            };

            db.AvailabilitySlots.Add(slot);
            await db.SaveChangesAsync(ct);
            return new Result<SlotDto>(ScheduleMapper.ToDto(slot));
        }

        public async Task<List<SlotDto>> GetAvailableSlotsAsync(
            string trainerUserId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var effectiveFrom = fromUtc < now ? now : fromUtc;

            var slots = await db.AvailabilitySlots
                .Where(s => s.TrainerUserId == trainerUserId
                            && !s.IsBooked
                            && s.StartUtc >= effectiveFrom
                            && s.StartUtc <= toUtc)
                .OrderBy(s => s.StartUtc)
                .ToListAsync(ct);

            return slots.Select(ScheduleMapper.ToDto).ToList();
        }

        public async Task<List<SlotDto>> GetMyScheduleAsync(
            string trainerUserId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
        {
            var slots = await db.AvailabilitySlots
                .Where(s => s.TrainerUserId == trainerUserId
                            && s.StartUtc >= fromUtc
                            && s.StartUtc <= toUtc)
                .OrderBy(s => s.StartUtc)
                .ToListAsync(ct);

            return slots.Select(ScheduleMapper.ToDto).ToList();
        }

        public async Task<Result> DeleteSlotAsync(
            string trainerUserId, Guid slotId, CancellationToken ct = default)
        {
            var slot = await db.AvailabilitySlots.FindAsync([slotId], ct);

            if (slot is null)
                return new Result(TrainerErrors.SlotNotFound());

            if (slot.TrainerUserId != trainerUserId)
                return new Result(TrainerErrors.SlotBelongsToAnotherTrainer());

            if (slot.IsBooked)
                return new Result(TrainerErrors.CannotDeleteBookedSlot());

            db.AvailabilitySlots.Remove(slot);
            await db.SaveChangesAsync(ct);
            return Result.Success;
        }

        // ─── Бронирования ─────────────────────────────────────────────────────

        public async Task<Result<BookingDto>> BookAsync(
            string studentUserId, CreateBookingRequest request, CancellationToken ct = default)
        {
            var slot = await db.AvailabilitySlots.FindAsync([request.SlotId], ct);

            if (slot is null)
                return new Result<BookingDto>(TrainerErrors.SlotNotFound());

            if (slot.TrainerUserId == studentUserId)
                return new Result<BookingDto>(TrainerErrors.CannotBookOwnSlot());

            if (slot.IsBooked)
                return new Result<BookingDto>(TrainerErrors.SlotAlreadyBooked());

            if (slot.StartUtc <= DateTime.UtcNow)
                return new Result<BookingDto>(TrainerErrors.SlotInPast());

            var now = DateTime.UtcNow;
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                StudentUserId = studentUserId,
                TrainerUserId = slot.TrainerUserId,
                SlotId = slot.Id,
                Status = BookingStatus.Pending,
                StudentNote = request.Note?.Trim(),
                CreatedAtUtc = now,
            };

            slot.IsBooked = true;
            slot.BookingId = booking.Id;

            db.Bookings.Add(booking);
            await db.SaveChangesAsync(ct);

            return new Result<BookingDto>(ScheduleMapper.ToDto(booking, slot));
        }

        public async Task<List<BookingDto>> GetStudentBookingsAsync(
            string studentUserId, CancellationToken ct = default)
        {
            var bookings = await db.Bookings
                .Where(b => b.StudentUserId == studentUserId)
                .OrderByDescending(b => b.CreatedAtUtc)
                .ToListAsync(ct);

            return await EnrichWithSlotsAsync(bookings, ct);
        }

        public async Task<List<BookingDto>> GetTrainerBookingsAsync(
            string trainerUserId, CancellationToken ct = default)
        {
            var bookings = await db.Bookings
                .Where(b => b.TrainerUserId == trainerUserId)
                .OrderByDescending(b => b.CreatedAtUtc)
                .ToListAsync(ct);

            return await EnrichWithSlotsAsync(bookings, ct);
        }

        public async Task<Result<BookingDto>> ConfirmAsync(
            string trainerUserId, Guid bookingId, CancellationToken ct = default)
        {
            var (booking, slot, error) = await LoadBookingAndSlotAsync(bookingId, ct);
            if (error is not null) return new Result<BookingDto>(error);

            if (booking!.TrainerUserId != trainerUserId)
                return new Result<BookingDto>(TrainerErrors.NotBookingTrainer());

            if (booking.Status != BookingStatus.Pending)
                return new Result<BookingDto>(TrainerErrors.BookingNotConfirmable());

            booking.Status = BookingStatus.Confirmed;
            booking.ConfirmedAtUtc = DateTime.UtcNow;

            await db.SaveChangesAsync(ct);
            return new Result<BookingDto>(ScheduleMapper.ToDto(booking, slot));
        }

        public async Task<Result<BookingDto>> CancelAsync(
            string userId, Guid bookingId, CancelBookingRequest request, CancellationToken ct = default)
        {
            var (booking, slot, error) = await LoadBookingAndSlotAsync(bookingId, ct);
            if (error is not null) return new Result<BookingDto>(error);

            bool isStudent = booking!.StudentUserId == userId;
            bool isTrainer = booking.TrainerUserId == userId;

            if (!isStudent && !isTrainer)
                return new Result<BookingDto>(TrainerErrors.NotBookingStudent());

            if (booking.Status is not (BookingStatus.Pending or BookingStatus.Confirmed))
                return new Result<BookingDto>(TrainerErrors.BookingNotCancellable());

            var now = DateTime.UtcNow;
            bool isLate = slot is not null && (slot.StartUtc - now).TotalHours < 24;

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAtUtc = now;
            booking.CancelledBy = isStudent ? BookingCancelledBy.Student : BookingCancelledBy.Trainer;
            booking.CancellationReason = request.Reason?.Trim();
            booking.IsLateCancel = isLate;

            // Освобождаем слот.
            if (slot is not null)
            {
                slot.IsBooked = false;
                slot.BookingId = null;
            }

            await db.SaveChangesAsync(ct);
            return new Result<BookingDto>(ScheduleMapper.ToDto(booking, slot));
        }

        public async Task<Result<BookingDto>> CompleteAsync(
            string trainerUserId, Guid bookingId, CancellationToken ct = default)
        {
            var (booking, slot, error) = await LoadBookingAndSlotAsync(bookingId, ct);
            if (error is not null) return new Result<BookingDto>(error);

            if (booking!.TrainerUserId != trainerUserId)
                return new Result<BookingDto>(TrainerErrors.NotBookingTrainer());

            if (booking.Status != BookingStatus.Confirmed)
                return new Result<BookingDto>(TrainerErrors.BookingNotCompletable());

            booking.Status = BookingStatus.Completed;
            booking.CompletedAtUtc = DateTime.UtcNow;

            await db.SaveChangesAsync(ct);
            return new Result<BookingDto>(ScheduleMapper.ToDto(booking, slot));
        }

        public async Task<Result<BookingDto>> MarkNoShowAsync(
            string trainerUserId, Guid bookingId, CancellationToken ct = default)
        {
            var (booking, slot, error) = await LoadBookingAndSlotAsync(bookingId, ct);
            if (error is not null) return new Result<BookingDto>(error);

            if (booking!.TrainerUserId != trainerUserId)
                return new Result<BookingDto>(TrainerErrors.NotBookingTrainer());

            if (booking.Status != BookingStatus.Confirmed)
                return new Result<BookingDto>(TrainerErrors.BookingNotNoShowable());

            booking.Status = BookingStatus.NoShow;

            await db.SaveChangesAsync(ct);
            return new Result<BookingDto>(ScheduleMapper.ToDto(booking, slot));
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private async Task<(Booking? booking, AvailabilitySlot? slot, Error? error)>
            LoadBookingAndSlotAsync(Guid bookingId, CancellationToken ct)
        {
            var booking = await db.Bookings.FindAsync([bookingId], ct);
            if (booking is null)
                return (null, null, TrainerErrors.BookingNotFound());

            var slot = await db.AvailabilitySlots.FindAsync([booking.SlotId], ct);
            return (booking, slot, null);
        }

        private async Task<List<BookingDto>> EnrichWithSlotsAsync(
            List<Booking> bookings, CancellationToken ct)
        {
            if (bookings.Count == 0) return [];

            var slotIds = bookings.Select(b => b.SlotId).Distinct().ToList();
            var slots = await db.AvailabilitySlots
                .Where(s => slotIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, ct);

            return bookings
                .Select(b => ScheduleMapper.ToDto(b, slots.GetValueOrDefault(b.SlotId)))
                .ToList();
        }
    }
}

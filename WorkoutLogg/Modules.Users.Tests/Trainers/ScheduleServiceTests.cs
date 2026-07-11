using Microsoft.EntityFrameworkCore;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;
using Modules.Trainers.Infrastructure.Services;

namespace Modules.Users.Tests.Trainers;

[TestFixture]
public class ScheduleServiceTests
{
    private TrainersDbContext _db = null!;
    private ScheduleService _service = null!;

    private const string TrainerId = "trainer-1";
    private const string StudentId = "student-1";

    private static DateTime FutureUtc(int hours = 24) => DateTime.UtcNow.AddHours(hours);

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<TrainersDbContext>()
            .UseInMemoryDatabase($"schedule-{Guid.NewGuid()}")
            .Options;
        _db = new TrainersDbContext(options);
        _service = new ScheduleService(_db);
    }

    [TearDown]
    public void TearDown() => _db.Dispose();

    // ─── AddSlot ──────────────────────────────────────────────────────────────

    [Test]
    public async Task AddSlot_ValidRequest_CreatesSlot()
    {
        var start = FutureUtc(2);
        var end = start.AddHours(1);

        var result = await _service.AddSlotAsync(TrainerId, new CreateSlotRequest
        {
            StartUtc = start,
            EndUtc = end,
            Note = "онлайн",
        });

        Assert.That(result.IsSuccess, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.TrainerUserId, Is.EqualTo(TrainerId));
            Assert.That(result.Value.DurationMinutes, Is.EqualTo(60));
            Assert.That(result.Value.IsBooked, Is.False);
            Assert.That(result.Value.Note, Is.EqualTo("онлайн"));
            Assert.That(_db.AvailabilitySlots.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public async Task AddSlot_InPast_ReturnsValidationError()
    {
        var result = await _service.AddSlotAsync(TrainerId, new CreateSlotRequest
        {
            StartUtc = DateTime.UtcNow.AddHours(-1),
            EndUtc = DateTime.UtcNow.AddHours(1),
        });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("SlotInPast"));
    }

    [Test]
    public async Task AddSlot_EndBeforeStart_ReturnsValidationError()
    {
        var start = FutureUtc(2);

        var result = await _service.AddSlotAsync(TrainerId, new CreateSlotRequest
        {
            StartUtc = start,
            EndUtc = start.AddMinutes(-10),
        });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("SlotEndBeforeStart"));
    }

    [Test]
    public async Task AddSlot_TooShort_ReturnsValidationError()
    {
        var start = FutureUtc(2);

        var result = await _service.AddSlotAsync(TrainerId, new CreateSlotRequest
        {
            StartUtc = start,
            EndUtc = start.AddMinutes(10), // 10 < 15 min minimum
        });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("SlotTooShort"));
    }

    [Test]
    public async Task AddSlot_TooLong_ReturnsValidationError()
    {
        var start = FutureUtc(2);

        var result = await _service.AddSlotAsync(TrainerId, new CreateSlotRequest
        {
            StartUtc = start,
            EndUtc = start.AddHours(9), // 9h > 8h maximum
        });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("SlotTooLong"));
    }

    // ─── DeleteSlot ───────────────────────────────────────────────────────────

    [Test]
    public async Task DeleteSlot_UnbookedOwnSlot_Succeeds()
    {
        var slot = await AddSlotAsync(TrainerId);

        var result = await _service.DeleteSlotAsync(TrainerId, slot.Id);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(_db.AvailabilitySlots.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task DeleteSlot_AnotherTrainersSlot_ReturnsForbidden()
    {
        var slot = await AddSlotAsync(TrainerId);

        var result = await _service.DeleteSlotAsync("other-trainer", slot.Id);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("SlotBelongsToAnotherTrainer"));
    }

    [Test]
    public async Task DeleteSlot_BookedSlot_ReturnsConflict()
    {
        var slot = await AddSlotAsync(TrainerId);
        slot.IsBooked = true;
        await _db.SaveChangesAsync();

        var result = await _service.DeleteSlotAsync(TrainerId, slot.Id);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("CannotDeleteBookedSlot"));
    }

    // ─── Book ─────────────────────────────────────────────────────────────────

    [Test]
    public async Task Book_FreeSlot_CreatesBookingAndMarksSlotBooked()
    {
        var slot = await AddSlotAsync(TrainerId);

        var result = await _service.BookAsync(StudentId, new CreateBookingRequest
        {
            SlotId = slot.Id,
            Note = "жду",
        });

        Assert.That(result.IsSuccess, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.StudentUserId, Is.EqualTo(StudentId));
            Assert.That(result.Value.TrainerUserId, Is.EqualTo(TrainerId));
            Assert.That(result.Value.Status, Is.EqualTo("Pending"));
            Assert.That(result.Value.StudentNote, Is.EqualTo("жду"));
            Assert.That(_db.AvailabilitySlots.Single().IsBooked, Is.True);
        });
    }

    [Test]
    public async Task Book_AlreadyBooked_ReturnsConflict()
    {
        var slot = await AddSlotAsync(TrainerId);
        await _service.BookAsync(StudentId, new CreateBookingRequest { SlotId = slot.Id });

        var result = await _service.BookAsync("student-2", new CreateBookingRequest { SlotId = slot.Id });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("SlotAlreadyBooked"));
    }

    [Test]
    public async Task Book_OwnSlot_ReturnsValidationError()
    {
        var slot = await AddSlotAsync(TrainerId);

        var result = await _service.BookAsync(TrainerId, new CreateBookingRequest { SlotId = slot.Id });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("CannotBookOwnSlot"));
    }

    [Test]
    public async Task Book_NonExistentSlot_ReturnsNotFound()
    {
        var result = await _service.BookAsync(StudentId, new CreateBookingRequest { SlotId = Guid.NewGuid() });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("SlotNotFound"));
    }

    // ─── Confirm ──────────────────────────────────────────────────────────────

    [Test]
    public async Task Confirm_PendingBooking_ChangesStatusToConfirmed()
    {
        var booking = await BookSlotAsync();

        var result = await _service.ConfirmAsync(TrainerId, booking.Id);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!.Status, Is.EqualTo("Confirmed"));
    }

    [Test]
    public async Task Confirm_ByWrongTrainer_ReturnsForbidden()
    {
        var booking = await BookSlotAsync();

        var result = await _service.ConfirmAsync("other-trainer", booking.Id);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("NotBookingTrainer"));
    }

    [Test]
    public async Task Confirm_AlreadyConfirmed_ReturnsConflict()
    {
        var booking = await BookSlotAsync();
        await _service.ConfirmAsync(TrainerId, booking.Id);

        var result = await _service.ConfirmAsync(TrainerId, booking.Id);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("BookingNotConfirmable"));
    }

    // ─── Cancel ───────────────────────────────────────────────────────────────

    [Test]
    public async Task Cancel_ByStudent_ReleasesSlot()
    {
        var booking = await BookSlotAsync();

        var result = await _service.CancelAsync(StudentId, booking.Id, new CancelBookingRequest { Reason = "передумал" });

        Assert.That(result.IsSuccess, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.Status, Is.EqualTo("Cancelled"));
            Assert.That(result.Value.CancelledBy, Is.EqualTo("Student"));
            Assert.That(result.Value.CancellationReason, Is.EqualTo("передумал"));
            Assert.That(_db.AvailabilitySlots.Single().IsBooked, Is.False);
        });
    }

    [Test]
    public async Task Cancel_ByTrainer_ReleasesSlot()
    {
        var booking = await BookSlotAsync();
        await _service.ConfirmAsync(TrainerId, booking.Id);

        var result = await _service.CancelAsync(TrainerId, booking.Id, new CancelBookingRequest());

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!.CancelledBy, Is.EqualTo("Trainer"));
        Assert.That(_db.AvailabilitySlots.Single().IsBooked, Is.False);
    }

    [Test]
    public async Task Cancel_CompletedBooking_ReturnsConflict()
    {
        var booking = await BookSlotAsync();
        await _service.ConfirmAsync(TrainerId, booking.Id);
        await _service.CompleteAsync(TrainerId, booking.Id);

        var result = await _service.CancelAsync(StudentId, booking.Id, new CancelBookingRequest());

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("BookingNotCancellable"));
    }

    [Test]
    public async Task Cancel_ByUnrelatedUser_ReturnsError()
    {
        var booking = await BookSlotAsync();

        var result = await _service.CancelAsync("random-user", booking.Id, new CancelBookingRequest());

        Assert.That(result.IsSuccess, Is.False);
    }

    // ─── Complete / NoShow ────────────────────────────────────────────────────

    [Test]
    public async Task Complete_ConfirmedBooking_Succeeds()
    {
        var booking = await BookSlotAsync();
        await _service.ConfirmAsync(TrainerId, booking.Id);

        var result = await _service.CompleteAsync(TrainerId, booking.Id);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!.Status, Is.EqualTo("Completed"));
    }

    [Test]
    public async Task Complete_PendingBooking_ReturnsConflict()
    {
        var booking = await BookSlotAsync();

        var result = await _service.CompleteAsync(TrainerId, booking.Id);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("BookingNotCompletable"));
    }

    [Test]
    public async Task MarkNoShow_ConfirmedBooking_Succeeds()
    {
        var booking = await BookSlotAsync();
        await _service.ConfirmAsync(TrainerId, booking.Id);

        var result = await _service.MarkNoShowAsync(TrainerId, booking.Id);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!.Status, Is.EqualTo("NoShow"));
    }

    [Test]
    public async Task MarkNoShow_PendingBooking_ReturnsConflict()
    {
        var booking = await BookSlotAsync();

        var result = await _service.MarkNoShowAsync(TrainerId, booking.Id);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("BookingNotNoShowable"));
    }

    // ─── GetAvailableSlots ────────────────────────────────────────────────────

    [Test]
    public async Task GetAvailableSlots_ExcludesBookedAndPast()
    {
        await AddSlotAsync(TrainerId);           // free future slot → should appear
        var bookedSlot = await AddSlotAsync(TrainerId);
        await _service.BookAsync(StudentId, new CreateBookingRequest { SlotId = bookedSlot.Id }); // booked → excluded

        var slots = await _service.GetAvailableSlotsAsync(TrainerId, DateTime.UtcNow, DateTime.UtcNow.AddDays(30));

        Assert.That(slots, Has.Count.EqualTo(1));
        Assert.That(slots[0].IsBooked, Is.False);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private async Task<AvailabilitySlot> AddSlotAsync(string trainerId)
    {
        var start = FutureUtc(24 + _db.AvailabilitySlots.Count()); // avoid overlaps
        var slot = new AvailabilitySlot
        {
            Id = Guid.NewGuid(),
            TrainerUserId = trainerId,
            StartUtc = start,
            EndUtc = start.AddHours(1),
            DurationMinutes = 60,
            IsBooked = false,
            CreatedAtUtc = DateTime.UtcNow,
        };
        _db.AvailabilitySlots.Add(slot);
        await _db.SaveChangesAsync();
        return slot;
    }

    private async Task<Booking> BookSlotAsync()
    {
        var slot = await AddSlotAsync(TrainerId);
        var result = await _service.BookAsync(StudentId, new CreateBookingRequest { SlotId = slot.Id });
        Assert.That(result.IsSuccess, Is.True, "test setup: booking should succeed");
        return _db.Bookings.Single(b => b.Id == result.Value!.Id);
    }
}

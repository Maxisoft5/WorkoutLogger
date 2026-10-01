using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Services;
using Modules.Users.Infrastructure.Database;
using WorkoutLogger.WebApi.Extensions;
using WorkoutLogger.WebApi.Services;

namespace WorkoutLogger.WebApi.Controllers;

[ApiController, Authorize]
public sealed class WebGymController(UsersDbContext users, TrainersDbContext trainers, ICurrentUser current,
    ITrainerProfileService profiles) : ControllerBase
{
    [HttpGet("api/web/conversations")]
    public async Task<IActionResult> Conversations([FromServices] IChatService chat, CancellationToken ct)
    {
        if (current.UserId is null) return Unauthorized();
        var conversations = await chat.GetConversationsAsync(current.UserId, ct);
        var ids = conversations.Select(x => x.StudentUserId == current.UserId ? x.TrainerUserId : x.StudentUserId).Distinct().ToArray();
        var names = await users.Users.AsNoTracking().Where(x => ids.Contains(x.Id))
            .Select(x => new { x.Id, x.UserName }).ToDictionaryAsync(x => x.Id, ct);
        return Ok(conversations.Select(c => new { c.Id, c.StudentUserId, c.TrainerUserId, c.LastMessageText, c.UnreadCount,
            OtherName = names.GetValueOrDefault(c.StudentUserId == current.UserId ? c.TrainerUserId : c.StudentUserId)?.UserName ?? "Участник клуба" }));
    }
    [HttpGet("api/web/progress")]
    public async Task<IActionResult> Progress(CancellationToken ct)
    {
        var mine = users.Workouts.AsNoTracking().Where(x => x.UserId == current.UserId);
        var today = DateTime.UtcNow.Date;
        var month = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthSessions = await mine.Where(x => x.StartDate >= month && x.StartDate < month.AddMonths(1))
            .Select(x => new { x.StartDate, x.EndDate }).ToListAsync(ct);
        var days = (await mine.Where(x => x.StartDate <= DateTime.UtcNow).Select(x => x.StartDate.Date).Distinct().ToListAsync(ct)).ToHashSet();
        var day = days.Contains(today) ? today : today.AddDays(-1);
        var streak = 0;
        while (days.Contains(day)) { streak++; day = day.AddDays(-1); }
        var records = await users.ExerciseSets.AsNoTracking().Where(s => s.Exercise.Workout.UserId == current.UserId && !s.IsWarmup)
            .GroupBy(s => s.Exercise.Name).Select(g => new { Name = g.Key, WeightKg = g.Max(s => s.WeightKg) })
            .OrderByDescending(x => x.WeightKg).Take(20).ToListAsync(ct);
        return Ok(new { totalSessions = await mine.CountAsync(ct), monthSessions = monthSessions.Count,
            monthMinutes = monthSessions.Sum(x => Math.Max(0, (int)(x.EndDate - x.StartDate).TotalMinutes)), currentStreak = streak, records });
    }
    [HttpGet("api/web/workouts/{id:guid}")]
    public async Task<IActionResult> Workout(Guid id, CancellationToken ct)
    {
        var workout = await users.Workouts.AsNoTracking().Where(x => x.Id == id && x.UserId == current.UserId)
            .Select(x => new { x.Id, x.WorkoutType, x.StartDate, x.EndDate,
                Exercises = x.Exercises.Select(e => new { e.Name, e.Description, Complexity = e.ExerciseComplexity,
                    Sets = e.Sets.OrderBy(s => s.SetNumber).Select(s => new { s.SetNumber, s.Reps, s.WeightKg, s.RestSeconds, s.IsWarmup }) }) })
            .SingleOrDefaultAsync(ct);
        return workout is null ? NotFound() : Ok(workout);
    }

    [HttpGet("api/web/trainers")]
    public Task<IActionResult> Trainers(CancellationToken ct) => ListTrainers(false, ct);

    [Authorize(Policy = "GymAdmin"), HttpGet("api/admin/trainers")]
    public Task<IActionResult> AdminTrainers(CancellationToken ct) => ListTrainers(true, ct);

    private async Task<IActionResult> ListTrainers(bool includeInactive, CancellationToken ct)
    {
        var profilesList = await trainers.TrainerProfiles.AsNoTracking().Where(x => includeInactive || x.IsActive)
            .OrderBy(x => x.UserId).Take(200).ToListAsync(ct);
        var ids = profilesList.Select(x => x.UserId).ToArray();
        var names = await users.Users.AsNoTracking().Where(x => ids.Contains(x.Id)).Select(x => new { x.Id, x.UserName }).ToDictionaryAsync(x => x.Id, ct);
        return Ok(profilesList.Select(x => new { x.Id, x.UserId, Name = names.GetValueOrDefault(x.UserId)?.UserName ?? "Тренер",
            x.About, x.PricePerSession, x.IsActive, x.Specializations, x.Experience, x.Formats }));
    }

    [Authorize(Policy = "GymAdmin"), HttpGet("api/admin/users")]
    public async Task<IActionResult> Members(string? search, CancellationToken ct) => Ok(await users.Users.AsNoTracking()
        .Where(x => search == null || (x.Email != null && x.Email.Contains(search)) || (x.PhoneNumber != null && x.PhoneNumber.Contains(search)) || (x.UserName != null && x.UserName.Contains(search)))
        .OrderBy(x => x.UserName).Take(100).Select(x => new { x.Id, Name = x.UserName, x.Email, x.PhoneNumber }).ToListAsync(ct));

    [Authorize(Policy = "GymAdmin"), HttpPut("api/admin/trainers/{userId}")]
    public async Task<IActionResult> SaveTrainer(string userId, UpsertTrainerProfileRequest request, CancellationToken ct)
    {
        if (!await users.Users.AnyAsync(x => x.Id == userId, ct)) return NotFound();
        return (await profiles.UpsertAsync(userId, request, ct)).ToActionResult();
    }
}

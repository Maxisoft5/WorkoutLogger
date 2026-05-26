using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moduels.Workouts.DTO.Requests;
using Moduels.Workouts.DTO.Responses;
using Modules.Users.Domain.Exercies;
using Modules.Users.Domain.Workout;
using Modules.Users.Infrastructure.Database;

namespace WorkoutLogger.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class WorkoutsController(UsersDbContext dbContext) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateWorkout([FromBody] CreateWorkoutRequest request, CancellationToken ct)
    {
        var userId = UserId();
        if (userId is null) return Unauthorized();

        var workout = new WorkoutModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WorkoutType = request.WorkoutType,
            StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc),
            EndDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc),
            CreatedAtUtc = DateTime.UtcNow,
            Exercises = request.Exercises.Select(MapExercise).ToList(),
        };

        dbContext.Workouts.Add(workout);
        await dbContext.SaveChangesAsync(ct);

        return Ok(ToResponse(workout, request.LocalId));
    }

    [HttpGet]
    public async Task<IActionResult> GetWorkouts(CancellationToken ct)
    {
        var userId = UserId();
        if (userId is null) return Unauthorized();

        var workouts = await dbContext.Workouts
            .Where(w => w.UserId == userId)
            .Include(w => w.Exercises)
            .OrderByDescending(w => w.StartDate)
            .ToListAsync(ct);

        return Ok(workouts.Select(w => ToResponse(w, Guid.Empty)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateWorkout(Guid id, [FromBody] UpdateWorkoutRequest request, CancellationToken ct)
    {
        var userId = UserId();
        if (userId is null) return Unauthorized();

        var workout = await dbContext.Workouts
            .Include(w => w.Exercises).ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId, ct);

        if (workout is null) return NotFound();

        workout.WorkoutType = request.WorkoutType;
        workout.StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
        workout.EndDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc);
        workout.UpdatedAtUtc = DateTime.UtcNow;

        // Remove old exercises one-by-one so EF cascade-deletes their Sets automatically.
        // Calling RemoveRange on Sets separately causes a double-delete concurrency exception
        // because DeleteBehavior.Cascade already marks Sets as Deleted when their Exercise is removed.
        foreach (var ex in workout.Exercises.ToList())
            dbContext.Remove(ex);

        foreach (var req in request.Exercises)
        {
            var ex = MapExercise(req);
            ex.WorkoutId = workout.Id;
            dbContext.Exercises.Add(ex);
        }

        await dbContext.SaveChangesAsync(ct);

        return Ok(ToResponse(workout, Guid.Empty));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWorkout(Guid id, CancellationToken ct)
    {
        var userId = UserId();
        if (userId is null) return Unauthorized();

        var workout = await dbContext.Workouts
            .Include(w => w.Exercises).ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId, ct);

        if (workout is null) return NotFound();

        dbContext.Workouts.Remove(workout);
        await dbContext.SaveChangesAsync(ct);

        return NoContent();
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private string? UserId() =>
        User.Claims.FirstOrDefault(c => c.Type == "userid")?.Value;

    private static Exercise MapExercise(CreateExerciseRequest e)
    {
        var exerciseId = Guid.NewGuid();
        return new Exercise
        {
            Id = exerciseId,
            Name = e.Name,
            Description = e.Description,
            ExerciesComplexity = e.Complexity,
            CreatedAtUtc = DateTime.UtcNow,
            Sets = e.Sets.Select((s, i) => new ExerciseSet
            {
                Id = Guid.NewGuid(),
                ExerciseId = exerciseId,
                SetNumber = s.SetNumber > 0 ? s.SetNumber : i + 1,
                Reps = s.Reps,
                WeightKg = s.WeightKg,
                RestSeconds = s.RestSeconds,
                IsWarmup = s.IsWarmup,
                CreatedAtUtc = DateTime.UtcNow,
            }).ToList(),
        };
    }

    private static WorkoutResponse ToResponse(WorkoutModel w, Guid localId) => new()
    {
        Id = w.Id,
        LocalId = localId,
        WorkoutType = w.WorkoutType,
        StartDate = w.StartDate,
        EndDate = w.EndDate,
        ExerciseCount = w.Exercises.Count,
    };
}

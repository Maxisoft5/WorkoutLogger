using Microsoft.EntityFrameworkCore;
using Modules.Workouts.DTO.Requests;
using Modules.Workouts.DTO.Responses;
using Modules.Users.Domain.Exercises;
using Modules.Users.Domain.Workout;
using Modules.Users.Infrastructure.Database;

namespace Modules.Users.Infrastructure.Workouts;

public class WorkoutService(UsersDbContext dbContext) : IWorkoutService
{
    public async Task<WorkoutResponse> CreateAsync(string userId, CreateWorkoutRequest request, CancellationToken ct = default)
    {
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

        return ToResponse(workout, request.LocalId);
    }

    public async Task<List<WorkoutResponse>> GetAsync(string userId, int page, int pageSize, CancellationToken ct = default)
    {
        var workouts = await dbContext.Workouts
            .Where(w => w.UserId == userId)
            .Include(w => w.Exercises)
            .OrderByDescending(w => w.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return workouts.Select(w => ToResponse(w, Guid.Empty)).ToList();
    }

    public async Task<WorkoutResponse?> UpdateAsync(string userId, Guid workoutId, UpdateWorkoutRequest request, CancellationToken ct = default)
    {
        var workout = await dbContext.Workouts
            .Include(w => w.Exercises).ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId, ct);

        if (workout is null) return null;

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

        return ToResponse(workout, Guid.Empty);
    }

    public async Task<bool> DeleteAsync(string userId, Guid workoutId, CancellationToken ct = default)
    {
        var workout = await dbContext.Workouts
            .Include(w => w.Exercises).ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId, ct);

        if (workout is null) return false;

        dbContext.Workouts.Remove(workout);
        await dbContext.SaveChangesAsync(ct);

        return true;
    }

    private static Exercise MapExercise(CreateExerciseRequest e)
    {
        var exerciseId = Guid.NewGuid();
        return new Exercise
        {
            Id = exerciseId,
            Name = e.Name,
            Description = e.Description,
            ExerciseComplexity = e.Complexity,
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

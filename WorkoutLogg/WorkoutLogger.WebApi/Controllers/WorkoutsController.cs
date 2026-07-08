using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Workouts.DTO.Requests;
using Modules.Users.Infrastructure.Workouts;
using WorkoutLogger.WebApi.Services;

namespace WorkoutLogger.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class WorkoutsController(IWorkoutService workoutService, ICurrentUser currentUser) : ControllerBase
{
    private const int DefaultPageSize = 100;
    private const int MaxPageSize = 200;

    [HttpPost]
    public async Task<IActionResult> CreateWorkout([FromBody] CreateWorkoutRequest request, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        if (userId is null) return Unauthorized();

        var response = await workoutService.CreateAsync(userId, request, ct);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetWorkouts(CancellationToken ct, int page = 1, int pageSize = DefaultPageSize)
    {
        var userId = currentUser.UserId;
        if (userId is null) return Unauthorized();

        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var workouts = await workoutService.GetAsync(userId, page, pageSize, ct);
        return Ok(workouts);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateWorkout(Guid id, [FromBody] UpdateWorkoutRequest request, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        if (userId is null) return Unauthorized();

        var response = await workoutService.UpdateAsync(userId, id, request, ct);
        if (response is null) return NotFound();

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWorkout(Guid id, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        if (userId is null) return Unauthorized();

        var deleted = await workoutService.DeleteAsync(userId, id, ct);
        if (!deleted) return NotFound();

        return NoContent();
    }
}

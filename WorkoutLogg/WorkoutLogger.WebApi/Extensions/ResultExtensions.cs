using Microsoft.AspNetCore.Mvc;
using Modules.Common.Domain.Results;

namespace WorkoutLogger.WebApi.Extensions;

/// <summary>
/// Maps domain <see cref="Result"/> objects to HTTP responses:
/// the raw Result wrapper (Errors/IsSuccess/NumericType) never leaves the API.
/// Success returns the value (or 204 for void results), errors return ProblemDetails
/// with a status code derived from <see cref="ErrorType"/>.
/// </summary>
public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result) =>
        result.IsSuccess ? new NoContentResult() : Problem(result.Errors);

    public static IActionResult ToActionResult<T>(this Result<T> result) =>
        result.IsSuccess ? new OkObjectResult(result.Value) : Problem(result.Errors);

    /// <summary>Success returns <paramref name="map"/>(Value) so controllers can expose a response DTO.</summary>
    public static IActionResult ToActionResult<T, TResponse>(this Result<T> result, Func<T, TResponse> map) =>
        result.IsSuccess ? new OkObjectResult(map(result.Value!)) : Problem(result.Errors);

    private static ObjectResult Problem(IReadOnlyList<Error>? errors)
    {
        var first = errors is { Count: > 0 } ? errors[0] : null;
        var status = first?.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError,
        };

        return new ObjectResult(new ProblemDetails
        {
            Status = status,
            Title = first?.Code ?? "Error",
            Detail = first?.Description,
        })
        { StatusCode = status };
    }
}

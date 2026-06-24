using JobTracker.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Extensions;

public static class ResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        return controller.Problem(result.Error!);
    }

    public static ActionResult<T> CreatedAtActionResult<T>(
        this ControllerBase controller,
        Result<T> result,
        string actionName,
        object? routeValues)
    {
        if (result.IsSuccess)
        {
            return controller.CreatedAtAction(actionName, routeValues, result.Value);
        }

        return controller.Problem(result.Error!);
    }

    public static ObjectResult Problem(this ControllerBase controller, Error error)
    {
        var statusCode = error.Type.ToStatusCode();
        var problemDetails = new ProblemDetails
        {
            Type = $"https://errors.jobtracker.dev/{error.Code}",
            Title = error.Title,
            Detail = error.Detail,
            Status = statusCode
        };

        problemDetails.Extensions["code"] = error.Code;
        problemDetails.Extensions["traceId"] = controller.HttpContext.TraceIdentifier;

        return controller.StatusCode(statusCode, problemDetails);
    }

    public static int ToStatusCode(this ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        _ => StatusCodes.Status500InternalServerError
    };
}

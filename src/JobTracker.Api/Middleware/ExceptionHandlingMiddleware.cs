using System.Net;
using JobTracker.Application.Common.Exceptions;
using JobTracker.Domain.Common;

namespace JobTracker.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            if (exception is not AppValidationException and not DomainException)
            {
                logger.LogError(exception, "Unhandled request exception.");
            }

            await WriteErrorResponse(context, exception);
        }
    }

    private static Task WriteErrorResponse(HttpContext context, Exception exception)
    {
        if (exception is AppValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return context.Response.WriteAsJsonAsync(new
            {
                title = "Validation failed.",
                status = StatusCodes.Status400BadRequest,
                errors = validationException.Errors
            });
        }

        var (statusCode, message) = exception switch
        {
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, exception.Message),
            DomainException => (HttpStatusCode.BadRequest, exception.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsJsonAsync(new { error = message });
    }
}

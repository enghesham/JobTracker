using JobTracker.Application.Common.Exceptions;
using JobTracker.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly Action<ILogger, Exception?> UnhandledRequestException =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(4000, nameof(UnhandledRequestException)),
            "Unhandled request exception.");

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
                UnhandledRequestException(logger, exception);
            }

            await WriteErrorResponse(context, exception);
        }
    }

    private static Task WriteErrorResponse(HttpContext context, Exception exception)
    {
        var (statusCode, code, title, detail) = exception switch
        {
            AppValidationException => (
                StatusCodes.Status400BadRequest,
                "validation-failed",
                "Validation failed",
                "One or more validation errors occurred."),
            DomainException => (
                StatusCodes.Status400BadRequest,
                "domain-rule-violated",
                "Domain rule violated",
                exception.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "unexpected-error",
                "Unexpected error",
                "An unexpected error occurred.")
        };

        var problemDetails = new ProblemDetails
        {
            Type = $"https://errors.jobtracker.dev/{code}",
            Title = title,
            Status = statusCode,
            Detail = detail
        };

        problemDetails.Extensions["code"] = code;
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        if (exception is AppValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}

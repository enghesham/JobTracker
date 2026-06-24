using System.Diagnostics;
using JobTracker.Application.Common.Observability;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JobTracker.Application.Common.Behaviors;

public sealed class RequestLoggingBehavior<TRequest, TResponse>(ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestType = GetRequestType(requestName);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next(cancellationToken);
            stopwatch.Stop();

            ApplicationDiagnostics.HandlerDuration.Record(
                stopwatch.Elapsed.TotalMilliseconds,
                new KeyValuePair<string, object?>("handler", requestName),
                new KeyValuePair<string, object?>("request_type", requestType),
                new KeyValuePair<string, object?>("status", "success"));

            logger.LogInformation(
                "Handled {RequestType} {HandlerName} in {ElapsedMilliseconds} ms.",
                requestType,
                requestName,
                stopwatch.Elapsed.TotalMilliseconds);

            return response;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            ApplicationDiagnostics.HandlerDuration.Record(
                stopwatch.Elapsed.TotalMilliseconds,
                new KeyValuePair<string, object?>("handler", requestName),
                new KeyValuePair<string, object?>("request_type", requestType),
                new KeyValuePair<string, object?>("status", "failure"));

            logger.LogWarning(
                exception,
                "Handler {RequestType} {HandlerName} failed in {ElapsedMilliseconds} ms.",
                requestType,
                requestName,
                stopwatch.Elapsed.TotalMilliseconds);

            throw;
        }
    }

    private static string GetRequestType(string requestName)
    {
        if (requestName.EndsWith("Command", StringComparison.Ordinal))
        {
            return "command";
        }

        if (requestName.EndsWith("Query", StringComparison.Ordinal))
        {
            return "query";
        }

        return "request";
    }
}

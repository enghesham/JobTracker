using System.Diagnostics;
using JobTracker.Application.Common.Observability;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JobTracker.Application.Common.Behaviors;

public sealed class RequestLoggingBehavior<TRequest, TResponse>(ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly Action<ILogger, string, string, double, Exception?> HandlerCompleted =
        LoggerMessage.Define<string, string, double>(
            LogLevel.Information,
            new EventId(1000, nameof(HandlerCompleted)),
            "Handled {RequestType} {HandlerName} in {ElapsedMilliseconds} ms.");

    private static readonly Action<ILogger, string, string, double, Exception?> HandlerFailed =
        LoggerMessage.Define<string, string, double>(
            LogLevel.Warning,
            new EventId(1001, nameof(HandlerFailed)),
            "Handler {RequestType} {HandlerName} failed in {ElapsedMilliseconds} ms.");

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

            HandlerCompleted(logger, requestType, requestName, stopwatch.Elapsed.TotalMilliseconds, null);

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

            HandlerFailed(logger, requestType, requestName, stopwatch.Elapsed.TotalMilliseconds, exception);

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

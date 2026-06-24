using System.Diagnostics.Metrics;

namespace JobTracker.Application.Common.Observability;

public static class ApplicationDiagnostics
{
    public const string MeterName = "JobTracker.Application";

    public static readonly Meter Meter = new(MeterName);

    public static readonly Histogram<double> HandlerDuration = Meter.CreateHistogram<double>(
        "jobtracker.handler.duration",
        unit: "ms",
        description: "Duration of MediatR command and query handlers.");
}

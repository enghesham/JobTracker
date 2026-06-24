namespace JobTracker.Domain.Common;

public abstract record DomainEvent(DateTimeOffset OccurredAtUtc);

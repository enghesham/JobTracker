namespace JobTracker.Domain.Common;

public abstract record DomainEvent(DateTime OccurredAtUtc)
{
    protected DomainEvent() : this(DateTime.UtcNow)
    {
    }
}

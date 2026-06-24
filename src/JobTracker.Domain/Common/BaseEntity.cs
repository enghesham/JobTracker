namespace JobTracker.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    internal void MarkAsCreated(DateTimeOffset createdAtUtc)
    {
        CreatedAtUtc = createdAtUtc.ToUniversalTime();
    }

    internal void MarkAsUpdated(DateTimeOffset updatedAtUtc)
    {
        UpdatedAtUtc = updatedAtUtc.ToUniversalTime();
    }
}

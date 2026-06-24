namespace JobTracker.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; private set; }

    public void MarkAsCreated(DateTime createdAtUtc)
    {
        CreatedAtUtc = createdAtUtc;
    }

    public void MarkAsUpdated(DateTime? updatedAtUtc = null)
    {
        UpdatedAtUtc = updatedAtUtc ?? DateTime.UtcNow;
    }
}

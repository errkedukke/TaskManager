namespace TaskManager.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime DateCreated { get; private set; } = DateTime.UtcNow;

    public DateTime? DateModified { get; private set; }

    public void SetModified() => DateModified = DateTime.UtcNow;
}
using TaskManager.Domain.Common;

namespace TaskManager.Domain;

public sealed class TaskAssignmentRecord : BaseEntity
{
    public Guid TaskItemId { get; set; }

    public TaskItem TaskItem { get; set; } = default!;

    public Guid UserId { get; set; }

    public User User { get; set; } = default!;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}

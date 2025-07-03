using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain;

public sealed class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public TaskState State { get; set; } = TaskState.Waiting;

    public Guid? AssignedUserId { get; set; }

    public User? AssignedUser { get; set; }

    public Guid? PreviouslyAssignedUserId { get; set; }

    public ICollection<TaskAssignmentRecord> TaskAssignmentRecords { get; set; } = [];
}
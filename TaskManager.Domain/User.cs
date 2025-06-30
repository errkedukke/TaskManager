using TaskManager.Domain.Common;

namespace TaskManager.Domain;

public sealed class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<TaskItem> Tasks { get; set; } = [];
}
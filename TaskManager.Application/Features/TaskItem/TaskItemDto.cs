namespace TaskManager.Application.Features.TaskItem;

public sealed record TaskItemDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public Guid? AssignedUserId { get; set; }

    public string AssignedUserName { get; set; } = string.Empty;
}

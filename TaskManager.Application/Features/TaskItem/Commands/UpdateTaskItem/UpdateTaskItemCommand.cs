using MediatR;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.TaskItem.Commands.UpdateTaskItem;

public class UpdateTaskItemCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public TaskState State { get; set; }

    public Guid? AssignedUserId { get; set; }
}

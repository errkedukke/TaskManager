using MediatR;

namespace TaskManager.Application.Features.TaskItem.Commands.CreateTaskItem;

public sealed class CreateTaskItemCommand : IRequest<Guid>
{
    public string Title { get; set; } = string.Empty;
}

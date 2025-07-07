using MediatR;

namespace TaskManager.Application.Features.TaskItem.Commands.DeleteTaskItem;

public sealed class DeleteTaskItemCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}

using MediatR;

namespace TaskManager.Application.Features.TaskItem.Commands.DeleteTaskItem;

public class DeleteTaskItemCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}

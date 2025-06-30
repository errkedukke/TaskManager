using MediatR;

namespace TaskManager.Application.Features.TaskItem.Commands.DeleteTaskItem;

public class DeleteTaskItemCommandHandler : IRequestHandler<DeleteTaskItemCommand, Unit>
{
    public Task<Unit> Handle(DeleteTaskItemCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
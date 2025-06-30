using MediatR;

namespace TaskManager.Application.Features.TaskItem.Commands.UpdateTaskItem;

public class UpdateTaskItemCommandHandler : IRequestHandler<UpdateTaskItemCommand, Unit>
{
    public Task<Unit> Handle(UpdateTaskItemCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

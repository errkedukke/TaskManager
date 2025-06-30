using MediatR;

namespace TaskManager.Application.Features.TaskItem.Commands.CreateTaskItem;

public class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, Guid>
{
    public Task<Guid> Handle(CreateTaskItemCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

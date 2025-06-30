using MediatR;

namespace TaskManager.Application.Features.TaskItem.Queries.GetTaskItem;

public class GetTaskItemQueryHandler : IRequestHandler<GetTaskItemQuery, TaskItemDto>
{
    public Task<TaskItemDto> Handle(GetTaskItemQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
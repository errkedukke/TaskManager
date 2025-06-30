using MediatR;

namespace TaskManager.Application.Features.TaskItem.Queries.GetTaskItems;

public class GetTaskItemsQueryHandler : IRequestHandler<GetTaskItemsQuery, List<TaskItemDto>>
{
    public Task<List<TaskItemDto>> Handle(GetTaskItemsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

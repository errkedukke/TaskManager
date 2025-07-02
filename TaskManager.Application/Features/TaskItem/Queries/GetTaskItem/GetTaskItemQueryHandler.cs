using AutoMapper;
using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.Contracts.Persistance;

namespace TaskManager.Application.Features.TaskItem.Queries.GetTaskItem;

public class GetTaskItemQueryHandler : IRequestHandler<GetTaskItemQuery, TaskItemDto>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IMapper _mapper;

    public GetTaskItemQueryHandler(ITaskItemRepository taskItemRepository, IMapper mapper)
    {
        _taskItemRepository = taskItemRepository;
        _mapper = mapper;
    }

    public async Task<TaskItemDto> Handle(GetTaskItemQuery request, CancellationToken cancellationToken)
    {
        var taskItem = await _taskItemRepository.GetByIdAsync(request.Id, cancellationToken);

        if (taskItem == null)
        {
            throw new NotFoundException(nameof(TaskItem), request.Id);
        }

        return _mapper.Map<TaskItemDto>(taskItem);
    }
}

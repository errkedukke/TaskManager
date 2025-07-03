using AutoMapper;
using MediatR;
using TaskManager.Application.Contracts.Persistance;

namespace TaskManager.Application.Features.TaskItem.Queries.GetTaskItems;

public class GetTaskItemsQueryHandler : IRequestHandler<GetTaskItemsQuery, List<TaskItemDto>>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IMapper _mapper;

    public GetTaskItemsQueryHandler(ITaskItemRepository taskItemRepository, IMapper mapper)
    {
        _taskItemRepository = taskItemRepository;
        _mapper = mapper;
    }

    public async Task<List<TaskItemDto>> Handle(GetTaskItemsQuery request, CancellationToken cancellationToken)
    {
        var taskItems = await _taskItemRepository.GetAsync(cancellationToken);

        return _mapper.Map<List<TaskItemDto>>(taskItems);
    }
}
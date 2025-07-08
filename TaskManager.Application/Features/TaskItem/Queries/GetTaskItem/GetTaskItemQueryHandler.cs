using AutoMapper;
using MediatR;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Application.Exceptions;

namespace TaskManager.Application.Features.TaskItem.Queries.GetTaskItem;

public sealed class GetTaskItemQueryHandler : IRequestHandler<GetTaskItemQuery, TaskItemDto>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public GetTaskItemQueryHandler(ITaskItemRepository taskItemRepository, IMapper mapper, IUserRepository userRepository)
    {
        _taskItemRepository = taskItemRepository;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<TaskItemDto> Handle(GetTaskItemQuery request, CancellationToken cancellationToken)
    {
        var taskItem = await _taskItemRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskItem), request.Id);

        var result = _mapper.Map<TaskItemDto>(taskItem);

        if (taskItem.AssignedUserId is Guid userId)
        {
            var assignedUser = await _userRepository.GetByIdAsync(userId, cancellationToken);
            result.AssignedUserName = assignedUser.Name;
        }

        return result;
    }
}

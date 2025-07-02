using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Common;
using TaskManager.Application.Contracts.Persistance;

namespace TaskManager.Application.Features.TaskItem.Commands.UpdateTaskItem;

public class UpdateTaskItemCommandHandler : IRequestHandler<UpdateTaskItemCommand, Unit>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ILogger<UpdateTaskItemCommandHandler> _logger;

    public UpdateTaskItemCommandHandler(ITaskItemRepository taskItemRepository, ILogger<UpdateTaskItemCommandHandler> logger)
    {
        _taskItemRepository = taskItemRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateTaskItemCommand request, CancellationToken cancellationToken)
    {
        var taskItem = await _taskItemRepository.GetByIdAsync(request.Id, cancellationToken);
        var originalTitle = taskItem.Title;

        if (taskItem == null)
        {
            throw new NotFoundException(nameof(TaskItem), request.Id);
        }

        taskItem.Title = request.Title;
        taskItem.State = request.State;
        taskItem.AssignedUserId = request.AssignedUserId;

        await _taskItemRepository.UpdateAsync(taskItem, cancellationToken);
        _logger.LogInformation($"TaskItem {originalTitle} was updated.");

        return Unit.Value;
    }
}

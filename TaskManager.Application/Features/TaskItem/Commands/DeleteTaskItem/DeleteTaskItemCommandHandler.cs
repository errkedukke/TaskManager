using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Common;
using TaskManager.Application.Contracts.Persistance;

namespace TaskManager.Application.Features.TaskItem.Commands.DeleteTaskItem;

public class DeleteTaskItemCommandHandler : IRequestHandler<DeleteTaskItemCommand, Unit>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ILogger<DeleteTaskItemCommandHandler> _logger;

    public DeleteTaskItemCommandHandler(ITaskItemRepository taskItemRepository, ILogger<DeleteTaskItemCommandHandler> logger)
    {
        _taskItemRepository = taskItemRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteTaskItemCommand request, CancellationToken cancellationToken)
    {
        var taskItem = await _taskItemRepository.GetByIdAsync(request.Id, cancellationToken);

        if (taskItem == null)
        {
            throw new NotFoundException(nameof(TaskItem), request.Id);
        }

        await _taskItemRepository.DeleteAsync(taskItem, cancellationToken);
        _logger.LogInformation($"Task {taskItem.Title} deleted.");

        return Unit.Value;
    }
}
using MediatR;
using TaskManager.Application.Contracts.BackgroundServices;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Domain.Events;

namespace TaskManager.Application.EventHandlers;

public sealed class TaskItemCreatedHandler : INotificationHandler<TaskItemCreatedDomainEvent>
{
    private readonly ITaskItemAssignmentService _taskItemAssignmentService;

    public TaskItemCreatedHandler(ITaskItemAssignmentService taskItemAssignmentService, ITaskItemRepository taskItemRepository)
    {
        _taskItemAssignmentService = taskItemAssignmentService;
    }

    public async Task Handle(TaskItemCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _taskItemAssignmentService.AssignInitialUserAsync(notification.TaskItem, cancellationToken);
    }
}

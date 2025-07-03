using MediatR;
using TaskManager.Domain.Events;

namespace TaskManager.Application.EventHandlers;

public class TaskItemCreatedHandler : INotificationHandler<TaskItemCreatedDomainEvent>
{
    public Task Handle(TaskItemCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

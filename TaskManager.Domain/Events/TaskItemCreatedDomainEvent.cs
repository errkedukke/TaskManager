using MediatR;

namespace TaskManager.Domain.Events;

public record class TaskItemCreatedDomainEvent(Guid TaskItemId) : INotification;

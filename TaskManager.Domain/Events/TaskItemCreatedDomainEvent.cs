using MediatR;

namespace TaskManager.Domain.Events;

public record class TaskItemCreatedDomainEvent(TaskItem TaskItem) : INotification;

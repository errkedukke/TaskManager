using MediatR;

namespace TaskManager.Application.Features.TaskItem.Queries.GetTaskItem;

public record GetTaskItemQuery(Guid Id) : IRequest<TaskItemDto>;

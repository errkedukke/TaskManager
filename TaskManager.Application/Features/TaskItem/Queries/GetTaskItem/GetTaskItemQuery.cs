using MediatR;

namespace TaskManager.Application.Features.TaskItem.Queries.GetTaskItem;

public record GetTaskItemQuery : IRequest<TaskItemDto>;

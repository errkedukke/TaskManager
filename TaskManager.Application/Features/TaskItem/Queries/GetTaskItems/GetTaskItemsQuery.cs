using MediatR;

namespace TaskManager.Application.Features.TaskItem.Queries.GetTaskItems;

public record GetTaskItemsQuery : IRequest<List<TaskItemDto>>;

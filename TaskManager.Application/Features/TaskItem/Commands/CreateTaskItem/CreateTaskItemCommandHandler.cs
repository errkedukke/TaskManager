using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Domain.Events;

namespace TaskManager.Application.Features.TaskItem.Commands.CreateTaskItem;

public class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, Guid>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ILogger<CreateTaskItemCommandHandler> _logger;
    private readonly IMediator _mediator;

    public CreateTaskItemCommandHandler(ITaskItemRepository taskItemRepository, ILogger<CreateTaskItemCommandHandler> logger, IMediator mediator)
    {
        _taskItemRepository = taskItemRepository;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(CreateTaskItemCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentNullException(nameof(request.Title));
        }

        if (await IsTaskItemUniuqe(request.Title, cancellationToken))
        {
            throw new InvalidOperationException("A task should be unique.");
        }

        var taskItem = new Domain.TaskItem
        {
            Title = request.Title,
        };

        await _taskItemRepository.CreateAsync(taskItem, cancellationToken);
        await _mediator.Publish(new TaskItemCreatedDomainEvent(taskItem), cancellationToken);

        _logger.LogInformation($"Task {taskItem.Title} created.");

        return taskItem.Id;
    }

    private async Task<bool> IsTaskItemUniuqe(string title, CancellationToken cancellationToken) =>
         await _taskItemRepository.IsTaskItemUniqueAsync(title, cancellationToken);
}
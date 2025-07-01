using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Contracts.Persistance;

namespace TaskManager.Application.Features.TaskItem.Commands.CreateTaskItem;

public class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, Guid>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ILogger<CreateTaskItemCommandHandler> _logger;

    public CreateTaskItemCommandHandler(ITaskItemRepository taskItemRepository, ILogger<CreateTaskItemCommandHandler> logger)
    {
        _taskItemRepository = taskItemRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateTaskItemCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            _logger.LogWarning("Attempted to create task with empty title.");
            throw new ArgumentException("Title cannot be empty", nameof(request.Title));
        }

        var taskItem = new Domain.TaskItem
        {
            Title = request.Title,
        };

        await _taskItemRepository.CreateAsync(taskItem, cancellationToken);

        _logger.LogInformation($"Task {taskItem.Title} created successfully.");
        return taskItem.Id;
    }
}
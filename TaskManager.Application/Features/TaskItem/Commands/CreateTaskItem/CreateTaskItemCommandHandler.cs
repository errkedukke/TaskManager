using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Application.Exceptions;
using TaskManager.Domain.Events;

namespace TaskManager.Application.Features.TaskItem.Commands.CreateTaskItem;

public sealed class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, Guid>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ILogger<CreateTaskItemCommandHandler> _logger;
    private readonly IMediator _mediator;
    private readonly IValidator<CreateTaskItemCommand> _validator;

    public CreateTaskItemCommandHandler(ITaskItemRepository taskItemRepository,
        ILogger<CreateTaskItemCommandHandler> logger,
        IMediator mediator,
        IValidator<CreateTaskItemCommand> validator)
    {
        _taskItemRepository = taskItemRepository;
        _logger = logger;
        _mediator = mediator;
        _validator = validator;
    }

    public async Task<Guid> Handle(CreateTaskItemCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid TaskItem", validationResult);
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
}
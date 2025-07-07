using FluentValidation;
using TaskManager.Application.Contracts.Persistance;

namespace TaskManager.Application.Features.TaskItem.Commands.CreateTaskItem;

public sealed class CreateTaskItemCommandValidator : AbstractValidator<CreateTaskItemCommand>
{
    private readonly ITaskItemRepository _taskItemRepository;

    public CreateTaskItemCommandValidator(ITaskItemRepository taskItemRepository)
    {
        _taskItemRepository = taskItemRepository;

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title name must not exceed 100 characters.");

        RuleFor(x => x.Title)
            .MaximumLength(250).WithMessage("Title must not exceed 250 characters.");

        RuleFor(x => x)
            .MustAsync(TaskItemTitleUnique).WithMessage("Title with this name already exists");
    }

    private Task<bool> TaskItemTitleUnique(CreateTaskItemCommand command, CancellationToken cancellationToken) =>
        _taskItemRepository.IsTaskItemUniqueAsync(command.Title, cancellationToken);
}
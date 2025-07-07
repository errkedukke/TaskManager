using FluentValidation;
using TaskManager.Application.Contracts.Persistance;

namespace TaskManager.Application.Features.User.Commands.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("User name is required")
            .MaximumLength(50).WithMessage("User name must not exceed 50 characters");

        RuleFor(x => x)
            .MustAsync(UserIsUnique).WithMessage("User name should be unique");
    }

    private Task<bool> UserIsUnique(CreateUserCommand command, CancellationToken cancellationToken) =>
        _userRepository.IsUserUniqueAsync(command.Name, cancellationToken);
}

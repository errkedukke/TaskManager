using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Application.Exceptions;

namespace TaskManager.Application.Features.User.Commands.CreateUser;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly IValidator<CreateUserCommand> _validator;

    public CreateUserCommandHandler(IUserRepository userRepository, ILogger<CreateUserCommandHandler> logger, IValidator<CreateUserCommand> validator)
    {
        _userRepository = userRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid TaskItem", validationResult);
        }

        var user = new Domain.User
        {
            Name = request.Name
        };

        await _userRepository.CreateAsync(user, cancellationToken);
        _logger.LogInformation($"User {user.Name} created.");

        return user.Id;
    }
}

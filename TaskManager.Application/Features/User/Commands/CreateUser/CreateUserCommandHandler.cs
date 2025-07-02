using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Contracts.Persistance;

namespace TaskManager.Application.Features.User.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(IUserRepository userRepository, ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentNullException(nameof(request.Name));
        }

        if (await IsUserUnique(request.Name, cancellationToken))
        {
            throw new InvalidOperationException("A user should be unique.");
        }

        var user = new Domain.User
        {
            Name = request.Name
        };

        await _userRepository.CreateAsync(user, cancellationToken);
        _logger.LogInformation($"User {user.Name} created.");

        return user.Id;
    }

    private async Task<bool> IsUserUnique(string name, CancellationToken cancellationToken) =>
        await _userRepository.IsUserUniqueAsync(name, cancellationToken);
}
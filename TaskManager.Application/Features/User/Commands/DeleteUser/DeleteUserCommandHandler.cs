using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Application.Exceptions;

namespace TaskManager.Application.Features.User.Commands.DeleteUser;

public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(IUserRepository userRepository, ILogger<DeleteUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.Id);
        }

        await _userRepository.DeleteAsync(user, cancellationToken);
        _logger.LogInformation($"User {user.Name} deleted.");

        return Unit.Value;
    }
}
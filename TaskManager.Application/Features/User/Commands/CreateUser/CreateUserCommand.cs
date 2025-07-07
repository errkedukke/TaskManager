using MediatR;

namespace TaskManager.Application.Features.User.Commands.CreateUser;

public sealed class CreateUserCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
}

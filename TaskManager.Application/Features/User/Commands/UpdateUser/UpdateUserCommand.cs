using MediatR;

namespace TaskManager.Application.Features.User.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

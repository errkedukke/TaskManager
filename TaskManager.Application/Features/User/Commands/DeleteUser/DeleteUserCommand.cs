using MediatR;

namespace TaskManager.Application.Features.User.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}

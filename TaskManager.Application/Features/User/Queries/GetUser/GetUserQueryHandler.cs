using MediatR;

namespace TaskManager.Application.Features.User.Queries.GetUser;

public record GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    public Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

using MediatR;

namespace TaskManager.Application.Features.User.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    public Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

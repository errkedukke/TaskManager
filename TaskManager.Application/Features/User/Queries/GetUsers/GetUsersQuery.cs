using MediatR;

namespace TaskManager.Application.Features.User.Queries.GetUsers;

public record GetUsersQuery : IRequest<List<UserDto>>;

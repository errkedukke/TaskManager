using MediatR;

namespace TaskManager.Application.Features.User.Queries.GetUser;

public record GetUserQuery(Guid Id) : IRequest<UserDto>;

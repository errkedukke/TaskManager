using MediatR;

namespace TaskManager.Application.Features.User.Queries.GetUser;

public record GetUserQuery : IRequest<UserDto>;

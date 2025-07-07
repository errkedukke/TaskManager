using AutoMapper;
using MediatR;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Application.Exceptions;

namespace TaskManager.Application.Features.User.Queries.GetUser;

public sealed class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.Id);
        }

        return _mapper.Map<UserDto>(user);
    }
}

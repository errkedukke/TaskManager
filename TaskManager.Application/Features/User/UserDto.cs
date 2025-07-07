namespace TaskManager.Application.Features.User;

public sealed class UserDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

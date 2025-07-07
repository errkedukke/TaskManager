using AutoMapper;
using TaskManager.Application.Features.TaskItem;
using TaskManager.Application.Features.User;
using TaskManager.Domain;

namespace TaskManager.Application.Mapping_Profiles;

public sealed class TaskManagerProfile : Profile
{
    public TaskManagerProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<TaskItem, TaskItemDto>().ReverseMap();
    }
}

using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TaskManager.Application.Contracts.BackgroundServices;
using TaskManager.Application.Mapping_Profiles;
using TaskManager.Application.Service;

namespace TaskManager.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var executionAssembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(typeof(TaskManagerProfile).Assembly);
        });

        services.AddMediatR(x => x.RegisterServicesFromAssembly(executionAssembly));
        services.AddScoped<ITaskItemAssignmentService, TaskItemAssignmentService>();
        services.AddValidatorsFromAssembly(executionAssembly);

        return services;
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TaskManager.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var executionAssembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(x => x.RegisterServicesFromAssembly(executionAssembly));
        services.AddAutoMapper(executionAssembly);

        return services;
    }
}

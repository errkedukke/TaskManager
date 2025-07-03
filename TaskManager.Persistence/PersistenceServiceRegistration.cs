using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Application.Contracts.Persistance.Common;
using TaskManager.Persistence.DatabaseContext;
using TaskManager.Persistence.Repositories;
using TaskManager.Persistence.Repositories.Common;

namespace TaskManager.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TaskManagerDbContext>(options =>
        {
            options.UseInMemoryDatabase("TaskManagerDb");
        });

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDbContextSeedService, DbContextSeedService>();

        return services;
    }
}
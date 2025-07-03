using System.Text.Json;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Domain;

namespace TaskManager.Persistence.DatabaseContext;

public class DbContextSeedService : IDbContextSeedService
{
    private readonly TaskManagerDbContext _dbContext;

    public DbContextSeedService(TaskManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedDatabaseAsync()
    {
        if (_dbContext.Users.Any() || _dbContext.TaskItems.Any())
        {
            return;
        }

        var basePath = Path.Combine("..", "TaskManager.Persistence", "Data");
        var taskItemsPath = Path.Combine(basePath, "TaskItems.json");
        var usersPath = Path.Combine(basePath, "Users.json");

        if (!File.Exists(taskItemsPath) || !File.Exists(usersPath))
        {
            throw new FileNotFoundException("Seed data files not found");
        }

        var taskItemsAsJson = await File.ReadAllTextAsync(taskItemsPath);
        var usersAsJson = await File.ReadAllTextAsync(usersPath);

        var tasks = JsonSerializer.Deserialize<List<TaskItem>>(taskItemsAsJson);
        var users = JsonSerializer.Deserialize<List<User>>(usersAsJson);

        if (tasks is null || users is null)
        {
            throw new Exception("Failed to deserialize seed data");
        }

        await _dbContext.TaskItems.AddRangeAsync(tasks);
        await _dbContext.Users.AddRangeAsync(users);
    }
}

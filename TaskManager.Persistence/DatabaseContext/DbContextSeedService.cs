using System.Text.Json;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Domain;

namespace TaskManager.Persistence.DatabaseContext;

public sealed class DbContextSeedService : IDbContextSeedService
{
    private readonly TaskManagerDbContext _dbContext;
    private readonly string _basePath = "..\\TaskManager.Persistence\\Data";

    public DbContextSeedService(TaskManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedDatabaseAsync()
    {
        if (DatabaseAlreadySeeded())
        {
            return;
        }

        var users = await LoadSeedDataAsync<User>("Users.json");
        var taskItems = await LoadSeedDataAsync<TaskItem>("TaskItems.json");
        var taskAssignments = await LoadSeedDataAsync<TaskAssignmentRecord>("TaskAssignmentRecords.json");

        await SeedEntitiesAsync(users);
        await SeedEntitiesAsync(taskItems);
        await SeedEntitiesAsync(taskAssignments);

        await _dbContext.SaveChangesAsync();
    }

    private bool DatabaseAlreadySeeded()
    {
        return _dbContext.Users.Any() || _dbContext.TaskItems.Any() || _dbContext.TaskAssignmentRecords.Any();
    }

    private async Task<List<T>> LoadSeedDataAsync<T>(string fileName)
    {
        var filePath = Path.Combine(_basePath, fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Seed data file '{fileName}' not found at '{filePath}'.");

        var jsonContent = await File.ReadAllTextAsync(filePath);
        var data = JsonSerializer.Deserialize<List<T>>(jsonContent);

        if (data == null)
            throw new InvalidDataException($"Failed to deserialize '{fileName}'.");

        return data;
    }

    private async Task SeedEntitiesAsync<T>(IEnumerable<T> entities) where T : class
    {
        await _dbContext.Set<T>().AddRangeAsync(entities);
    }

    public async Task SeedDatabaseAsyncw()
    {
        if (_dbContext.Users.Any() || _dbContext.TaskItems.Any() || _dbContext.TaskAssignmentRecords.Any())
        {
            return;
        }

        var taskItemsPath = Path.Combine(_basePath, "TaskItems.json");
        var usersPath = Path.Combine(_basePath, "Users.json");
        var taskAssignmentRecordPath = Path.Combine(_basePath, "TaskAssignmentRecords.json");

        if (!File.Exists(taskItemsPath) || !File.Exists(usersPath) || !File.Exists(taskAssignmentRecordPath))
        {
            throw new FileNotFoundException("Seed data files not found");
        }

        var taskItemsAsJson = await File.ReadAllTextAsync(taskItemsPath);
        var usersAsJson = await File.ReadAllTextAsync(usersPath);
        var taskAssignmentRecordAsJson = await File.ReadAllTextAsync(usersPath);

        var tasks = JsonSerializer.Deserialize<List<TaskItem>>(taskItemsAsJson);
        var users = JsonSerializer.Deserialize<List<User>>(usersAsJson);
        var taskASsignments = JsonSerializer.Deserialize<List<TaskAssignmentRecord>>(taskItemsAsJson);

        await _dbContext.TaskItems.AddRangeAsync(tasks);
        await _dbContext.Users.AddRangeAsync(users);
        await _dbContext.TaskAssignmentRecords.AddRangeAsync(taskASsignments);
    }
}

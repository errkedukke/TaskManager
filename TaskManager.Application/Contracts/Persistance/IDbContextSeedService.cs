namespace TaskManager.Application.Contracts.Persistance;

public interface IDbContextSeedService
{
    Task SeedDatabaseAsync();
}

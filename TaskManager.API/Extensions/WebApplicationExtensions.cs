using TaskManager.Application.Contracts.Persistance;

namespace TaskManager.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task SeedDatabase(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var contextSeeder = scope.ServiceProvider.GetRequiredService<IDbContextSeedService>();

            await contextSeeder.SeedDatabaseAsync();
        }
    }
}

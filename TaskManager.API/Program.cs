using TaskManager.API.Extensions;
using TaskManager.Application;
using TaskManager.Infrastructure;
using TaskManager.Persistence;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddApplicationServices(configuration);
builder.Services.AddInfrastructureServices(configuration);
builder.Services.AddPersistenceServices(configuration);

builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "TaskManager API";
    settings.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
    await app.SeedDatabase();
}

app.UseAuthorization();
app.MapControllers();
app.Run();

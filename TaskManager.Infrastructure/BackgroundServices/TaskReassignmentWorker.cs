using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskManager.Application.Contracts.BackgroundServices;

namespace TaskManager.Infrastructure.BackgroundServices;

public sealed class TaskReassignmentWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public TaskReassignmentWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await ProcessTasksOnceAsync(cancellationToken);
            await Task.Delay(TimeSpan.FromMinutes(2), cancellationToken);
        }
    }

    private async Task ProcessTasksOnceAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var taskItemAssignmentService = scope.ServiceProvider.GetRequiredService<ITaskItemAssignmentService>();
        await taskItemAssignmentService.ProcessReassignmentsAsync(cancellationToken);
    }
}

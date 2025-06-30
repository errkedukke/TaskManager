namespace TaskManager.Application.Contracts.BackgroundServices;

public interface ITaskAssignmentService
{
    Task ReassignTasksAsync(CancellationToken cancellationToken);
}
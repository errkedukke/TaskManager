namespace TaskManager.Application.Contracts.BackgroundServices;

public interface ITaskItemAssignmentService
{
    Task ReassignTaskItemssAsync(CancellationToken cancellationToken);
}
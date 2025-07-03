using TaskManager.Domain;

namespace TaskManager.Application.Contracts.BackgroundServices;

public interface ITaskItemAssignmentService
{
    /// <summary>
    /// Assigns inital user to a Task executed only once
    /// </summary>
    /// <param name="task"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AssignInitialUserAsync(TaskItem task, CancellationToken cancellationToken);

    /// <summary>
    /// This method would run once in 2 minutes
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ProcessReassignmentsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Used for determining if task has completed or not
    /// </summary>
    /// <param name="task"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> HasBeenAssignedToAllUsersAsync(TaskItem task, CancellationToken cancellationToken);
}
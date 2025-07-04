using TaskManager.Application.Contracts.Persistance.Common;
using TaskManager.Domain;

namespace TaskManager.Application.Contracts.Persistance;

public interface ITaskItemRepository : IGenericRepository<TaskItem>
{
    Task<List<TaskItem>> GetActiveTasksAsync(CancellationToken cancellationToken);

    Task<bool> IsTaskItemUniqueAsync(string title, CancellationToken cancellationToken);
}

using TaskManager.Application.Contracts.Persistance.Common;
using TaskManager.Domain;

namespace TaskManager.Application.Contracts.Persistance;

public interface ITaskItemRepository : IGenericRepository<TaskItem>
{
    Task<bool> IsTaskItemUniqueAsync(string title);
}

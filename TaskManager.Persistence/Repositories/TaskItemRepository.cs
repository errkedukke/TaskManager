using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Domain;
using TaskManager.Persistence.DatabaseContext;
using TaskManager.Persistence.Repositories.Common;

namespace TaskManager.Persistence.Repositories;

public class TaskItemRepository(TaskManagerDbContext dbContext)
    : GenericRepository<TaskItem>(dbContext), ITaskItemRepository
{
    public async Task<bool> IsTaskItemUniqueAsync(string title)
    {
        return await dbContext.TaskItems.AsNoTracking().AnyAsync(x => x.Title == title);
    }
}
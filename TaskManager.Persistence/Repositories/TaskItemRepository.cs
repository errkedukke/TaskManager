using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Domain;
using TaskManager.Domain.Enums;
using TaskManager.Persistence.DatabaseContext;
using TaskManager.Persistence.Repositories.Common;

namespace TaskManager.Persistence.Repositories;

public class TaskItemRepository(TaskManagerDbContext dbContext)
    : GenericRepository<TaskItem>(dbContext), ITaskItemRepository
{
    public async Task<List<TaskItem>> GetActiveTasksAsync(CancellationToken cancellationToken)
    {
        var taskItems = dbContext.TaskItems.Where(x => x.State != TaskState.Completed)
            .Include(x => x.TaskAssignmentRecords);

        return await taskItems.ToListAsync(cancellationToken);
    }

    public async Task<bool> IsTaskItemUniqueAsync(string title, CancellationToken cancellationToken)
    {
        return await dbContext.TaskItems.AsNoTracking().AnyAsync(x => x.Title == title, cancellationToken);
    }
}
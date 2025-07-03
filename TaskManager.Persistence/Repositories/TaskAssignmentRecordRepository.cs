using TaskManager.Application.Contracts.Persistance;
using TaskManager.Domain;
using TaskManager.Persistence.DatabaseContext;
using TaskManager.Persistence.Repositories.Common;

namespace TaskManager.Persistence.Repositories;

public class TaskAssignmentRecordRepository(TaskManagerDbContext context)
    : GenericRepository<TaskAssignmentRecord>(context), ITaskAssignmentRecordRepository;

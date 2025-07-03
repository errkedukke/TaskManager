using TaskManager.Application.Contracts.Persistance.Common;
using TaskManager.Domain;

namespace TaskManager.Application.Contracts.Persistance;

public interface ITaskAssignmentRecordRepository : IGenericRepository<TaskAssignmentRecord>;

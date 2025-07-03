using TaskManager.Application.Contracts.BackgroundServices;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Domain;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Service;

public class TaskItemAssignmentService : ITaskItemAssignmentService
{
    private readonly IUserRepository _userRepository;
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ITaskAssignmentRecordRepository _taskAssignmentRecordRepository;

    public TaskItemAssignmentService(IUserRepository userRepository, ITaskItemRepository taskItemRepository,
        ITaskAssignmentRecordRepository taskAssignmentRecordRepository)
    {
        _userRepository = userRepository;
        _taskItemRepository = taskItemRepository;
        _taskAssignmentRecordRepository = taskAssignmentRecordRepository;
    }

    public async Task AssignInitialUserAsync(TaskItem task, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAsync(cancellationToken);

        if (users.Count == 0)
        {
            task.State = TaskState.Waiting;
            return;
        }

        var random = PickRandom(users);

        task.AssignedUserId = random.Id;
        task.State = TaskState.InProgress;

        var taskAssignmentRecord = new TaskAssignmentRecord
        {
            TaskItemId = task.Id,
            UserId = random.Id,
        };

        await _taskAssignmentRecordRepository.CreateAsync(taskAssignmentRecord, cancellationToken);
        await _taskItemRepository.UpdateAsync(task, cancellationToken);
    }

    public Task<bool> HasBeenAssignedToAllUsersAsync(TaskItem task, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task ProcessReassignmentsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static T PickRandom<T>(IReadOnlyList<T> items)
    {
        return items[new Random().Next(items.Count)];
    }
}

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

    public TaskItemAssignmentService(IUserRepository userRepository, ITaskItemRepository taskItemRepository, ITaskAssignmentRecordRepository taskAssignmentRecordRepository)
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
            await _taskItemRepository.UpdateAsync(task, cancellationToken);
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

    public async Task ProcessReassignmentsAsync(CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAsync(cancellationToken);
        var activeTasks = await _taskItemRepository.GetActiveTasksAsync(cancellationToken);

        if (users.Count == 0 || activeTasks.Count == 0)
        {
            return;
        }

        foreach (var task in activeTasks)
        {
            var eligibleUsers = GetEligibleUsers(task, users);

            if (TaskAssignedToAllUsers(task, users))
            {
                CompleteTask(task);
            }
            else if (!eligibleUsers.Any())
            {
                PutTaskInWaitingState(task);
            }
            else
            {
                await ReassignTaskAsync(task, eligibleUsers, cancellationToken);
            }

            await _taskItemRepository.UpdateAsync(task, cancellationToken);
        }
    }

    private static T PickRandom<T>(IReadOnlyList<T> items)
    {
        return items[Random.Shared.Next(items.Count)];
    }

    private static List<User> GetEligibleUsers(TaskItem task, IReadOnlyList<User> users)
    {
        var excludeIds = new HashSet<Guid>();

        if (task.AssignedUserId.HasValue)
            excludeIds.Add(task.AssignedUserId.Value);

        if (task.PreviouslyAssignedUserId.HasValue)
            excludeIds.Add(task.PreviouslyAssignedUserId.Value);

        var alreadyAssignedIds = task.TaskAssignmentRecords
            .Select(x => x.UserId)
            .ToHashSet();

        return users
            .Where(x => !excludeIds.Contains(x.Id))
            .Where(x => !alreadyAssignedIds.Contains(x.Id))
            .ToList();
    }

    private static bool TaskAssignedToAllUsers(TaskItem task, IReadOnlyList<User> users)
    {
        var assignedUserIds = task.TaskAssignmentRecords
            .Select(r => r.UserId)
            .Distinct()
            .ToHashSet();

        return assignedUserIds.Count >= users.Count;
    }

    private static void CompleteTask(TaskItem task)
    {
        task.AssignedUserId = null;
        task.PreviouslyAssignedUserId = null;
        task.State = TaskState.Completed;
    }

    private static void PutTaskInWaitingState(TaskItem task)
    {
        task.AssignedUserId = null;
        task.State = TaskState.Waiting;
    }

    private async Task ReassignTaskAsync(TaskItem task, List<User> eligibleUsers, CancellationToken cancellationToken)
    {
        var newUser = PickRandom(eligibleUsers);

        task.PreviouslyAssignedUserId = task.AssignedUserId;
        task.AssignedUserId = newUser.Id;
        task.State = TaskState.InProgress;

        var assignmentRecord = new TaskAssignmentRecord
        {
            TaskItemId = task.Id,
            UserId = newUser.Id
        };

        await _taskAssignmentRecordRepository.CreateAsync(assignmentRecord, cancellationToken);
    }
}

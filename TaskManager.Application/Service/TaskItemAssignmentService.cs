using TaskManager.Application.Contracts.BackgroundServices;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Domain;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Service;

public sealed class TaskItemAssignmentService : ITaskItemAssignmentService
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

        if (users.Any())
        {
            var randomUser = PickRandom(users);
            await AssignTaskItemAsync(task, randomUser, cancellationToken);
        }
        else
        {
            await UnAssignTaskItemAsync(task, cancellationToken);
        }
    }

    public async Task ProcessReassignmentsAsync(CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAsync(cancellationToken);
        var activeTasks = await _taskItemRepository.GetActiveTasksAsync(cancellationToken);

        if (!users.Any())
        {
            foreach (var task in activeTasks)
            {
                await UnAssignTaskItemAsync(task, cancellationToken);
            }

            return;
        }

        foreach (var task in activeTasks)
        {
            await ProcessReAssignmentAsync(task, users, cancellationToken);
        }

    }

    private async Task ProcessReAssignmentAsync(TaskItem task, IReadOnlyList<User> users, CancellationToken cancellationToken)
    {
        if (TaskHasBeenAssignedToAllExistingUsers(task, users))
        {
            await CompleteTaskAync(task, cancellationToken);
            return;
        }

        var eligibleUsers = GetEligibleUsers(task, users);

        if (eligibleUsers.Any())
        {
            var randomUser = PickRandom(eligibleUsers);
            await AssignTaskItemAsync(task, randomUser, cancellationToken);
        }
        else
        {
            await UnAssignTaskItemAsync(task, cancellationToken);
        }
    }

    private async Task CompleteTaskAync(TaskItem task, CancellationToken cancellationToken)
    {
        task.AssignedUserId = null;
        task.AssignedUser = null;
        task.PreviouslyAssignedUserId = null;
        task.State = TaskState.Completed;

        await _taskItemRepository.UpdateAsync(task, cancellationToken);
    }

    private async Task UnAssignTaskItemAsync(TaskItem task, CancellationToken cancellationToken)
    {
        task.State = TaskState.Waiting;
        task.AssignedUserId = null;
        task.AssignedUser = null;

        await _taskItemRepository.UpdateAsync(task, cancellationToken);
    }

    private async Task AssignTaskItemAsync(TaskItem task, User user, CancellationToken cancellationToken)
    {
        task.PreviouslyAssignedUserId = task.AssignedUserId;
        task.AssignedUserId = user.Id;
        task.State = TaskState.InProgress;

        var taskAssignmentRecord = new TaskAssignmentRecord
        {
            TaskItemId = task.Id,
            UserId = user.Id,
        };

        await _taskAssignmentRecordRepository.CreateAsync(taskAssignmentRecord, cancellationToken);
        await _taskItemRepository.UpdateAsync(task, cancellationToken);
    }

    private static T PickRandom<T>(IReadOnlyList<T> items)
    {
        return items[Random.Shared.Next(items.Count)];
    }

    private static IReadOnlyList<User> GetEligibleUsers(TaskItem task, IReadOnlyList<User> users)
    {
        var excludeIds = new HashSet<Guid>();

        if (task.AssignedUserId.HasValue)
            excludeIds.Add(task.AssignedUserId.Value);

        if (task.PreviouslyAssignedUserId.HasValue)
            excludeIds.Add(task.PreviouslyAssignedUserId.Value);

        return users.Where(x => !excludeIds.Contains(x.Id)).ToList();
    }

    private static bool TaskHasBeenAssignedToAllExistingUsers(TaskItem task, IReadOnlyList<User> users)
    {
        var assignedUserIds = task.TaskAssignmentRecords.Select(x => x.UserId)
           .ToHashSet();

        return users.All(x => assignedUserIds.Contains(x.Id));
    }
}

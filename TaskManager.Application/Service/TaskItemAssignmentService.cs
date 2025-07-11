using Microsoft.Extensions.Logging;
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
    private readonly ILogger<TaskItemAssignmentService> _logger;

    public TaskItemAssignmentService(IUserRepository userRepository,
        ITaskItemRepository taskItemRepository,
        ITaskAssignmentRecordRepository taskAssignmentRecordRepository,
        ILogger<TaskItemAssignmentService> logger)
    {
        _userRepository = userRepository;
        _taskItemRepository = taskItemRepository;
        _taskAssignmentRecordRepository = taskAssignmentRecordRepository;
        _logger = logger;
    }

    public async Task AssignInitialUserAsync(TaskItem task, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting initial assignment for Task {TaskId}", task.Id);

        var users = await _userRepository.GetAsync(cancellationToken);
        _logger.LogInformation("Retrieved {UserCount} users from repository", users.Count);

        if (users.Any())
        {
            var randomUser = PickRandom(users);
            _logger.LogInformation("Assigning Task {TaskId} to random User {UserId}", task.Id, randomUser.Id);

            await AssignTaskItemAsync(task, randomUser, cancellationToken);

            _logger.LogInformation("Successfully assigned Task {TaskId} to User {UserId}", task.Id, randomUser.Id);
        }
        else
        {
            _logger.LogWarning("No users available for initial assignment. Unassigning Task {TaskId}", task.Id);

            await UnAssignTaskItemAsync(task, cancellationToken);

            _logger.LogInformation("Task {TaskId} unassigned due to lack of users", task.Id);
        }
    }

    public async Task ProcessReassignmentsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting task reassignment process");

        var users = await _userRepository.GetAsync(cancellationToken);
        _logger.LogInformation("Retrieved {UserCount} users from repository", users.Count);

        var activeTasks = await _taskItemRepository.GetActiveTasksAsync(cancellationToken);
        _logger.LogInformation("Retrieved {TaskCount} active tasks from repository", activeTasks.Count);

        if (!users.Any())
        {
            _logger.LogWarning("No users available. Unassigning all active tasks");

            foreach (var task in activeTasks)
            {
                _logger.LogInformation("Unassigning Task {TaskId} due to empty user list", task.Id);
                await UnAssignTaskItemAsync(task, cancellationToken);
            }

            _logger.LogInformation("Completed unassigning all tasks");
            return;
        }

        foreach (var task in activeTasks)
        {
            _logger.LogInformation("Processing reassignment for Task {TaskId}", task.Id);
            await ProcessReAssignmentAsync(task, users, cancellationToken);
        }

        _logger.LogInformation("Completed task reassignment process");
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

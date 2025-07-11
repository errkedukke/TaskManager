using Microsoft.Extensions.Logging;

namespace TaskManager.Application.Tests.ServiceTests;

[TestFixture]
public class TaskItemAssignmentServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<ITaskItemRepository> _taskItemRepositoryMock = null!;
    private Mock<ITaskAssignmentRecordRepository> _taskAssignmentRecordRepositoryMock = null!;
    private TaskItemAssignmentService _service = null!;
    private Mock<ILogger<TaskItemAssignmentService>> _loggerMock = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _taskItemRepositoryMock = new Mock<ITaskItemRepository>();
        _taskAssignmentRecordRepositoryMock = new Mock<ITaskAssignmentRecordRepository>();
        _loggerMock = new Mock<ILogger<TaskItemAssignmentService>>();
        _service = new TaskItemAssignmentService(_userRepositoryMock.Object, _taskItemRepositoryMock.Object, _taskAssignmentRecordRepositoryMock.Object, _loggerMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _userRepositoryMock = null!;
        _taskItemRepositoryMock = null!;
        _taskAssignmentRecordRepositoryMock = null!;
        _service = null!;
    }

    [Test]
    public async Task AssignInitialUserAsync_AssignsUser_WhenUsersExist()
    {
        // Arrange
        var task = new TaskItem { Id = Guid.NewGuid() };
        var user = new User { Id = Guid.NewGuid(), Name = "Test User" };
        var users = new List<User> { user };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        _taskAssignmentRecordRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<TaskAssignmentRecord>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _taskItemRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.AssignInitialUserAsync(task, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(task.AssignedUserId, Is.EqualTo(user.Id));
            Assert.That(task.State, Is.EqualTo(TaskState.InProgress));
        }

        _taskAssignmentRecordRepositoryMock.Verify(x => x.CreateAsync(
            It.Is<TaskAssignmentRecord>(x => x.TaskItemId == task.Id && x.UserId == user.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _taskItemRepositoryMock.Verify(x => x.UpdateAsync(task,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task AssignInitialUserAsync_SetsTaskToWaiting_WhenNoUsersExist()
    {
        // Arrange
        var task = new TaskItem { Id = Guid.NewGuid() };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _taskItemRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.AssignInitialUserAsync(task, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(task.AssignedUserId, Is.Null);
            Assert.That(task.State, Is.EqualTo(TaskState.Waiting));
        });

        _taskItemRepositoryMock.Verify(x => x.UpdateAsync(task,
            It.IsAny<CancellationToken>()), Times.Once);

        _taskAssignmentRecordRepositoryMock.Verify(x => x.CreateAsync(
            It.IsAny<TaskAssignmentRecord>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task AssignInitialUserAsync_CreatesTaskAssignmentRecord()
    {
        // Arrange
        var task = new TaskItem { Id = Guid.NewGuid() };
        var user = new User { Id = Guid.NewGuid(), Name = "Test" };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([user]);

        _taskAssignmentRecordRepositoryMock.Setup(x =>
            x.CreateAsync(It.IsAny<TaskAssignmentRecord>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _taskItemRepositoryMock.Setup(x =>
            x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.AssignInitialUserAsync(task, CancellationToken.None);

        // Assert
        _taskAssignmentRecordRepositoryMock.Verify(x => x.CreateAsync(
            It.Is<TaskAssignmentRecord>(x => x.TaskItemId == task.Id && x.UserId == user.Id),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task AssignInitialUserAsync_UpdatesTaskInRepository()
    {
        // Arrange
        var task = new TaskItem { Id = Guid.NewGuid() };
        var user = new User { Id = Guid.NewGuid(), Name = "Test" };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([user]);

        _taskAssignmentRecordRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<TaskAssignmentRecord>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _taskItemRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.AssignInitialUserAsync(task, CancellationToken.None);

        // Assert
        _taskItemRepositoryMock.Verify(x => x.UpdateAsync(task,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ProcessReassignmentsAsync_OneUserExists_TaskGetsCompleted()
    {
        // Arrange

        var taskItemId = Guid.NewGuid();
        var user = new User { Id = Guid.NewGuid(), Name = "Test" };
        var task = new TaskItem
        {
            Id = taskItemId,
            Title = "Test",
            AssignedUserId = user.Id,
            State = TaskState.InProgress,
            TaskAssignmentRecords = [
                new TaskAssignmentRecord { TaskItemId = taskItemId, UserId = user.Id }
            ]
        };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([user]);

        _taskItemRepositoryMock.Setup(x => x.GetActiveTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([task]);

        _taskItemRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ProcessReassignmentsAsync(CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(task.State, Is.EqualTo(TaskState.Completed));
            Assert.That(task.AssignedUserId, Is.Null);
        });

        _taskItemRepositoryMock.Verify(x => x.UpdateAsync(
            It.Is<TaskItem>(x => x.State == TaskState.Completed && x.AssignedUserId == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ProcessReassignmentsAsync_TwoUsersExist_TaskIsReassignedToSecondUser()
    {
        // Arrange
        var taskItemId = Guid.NewGuid();

        var userA = new User { Id = Guid.NewGuid(), Name = "User A" };
        var userB = new User { Id = Guid.NewGuid(), Name = "User B" };

        var task = new TaskItem
        {
            Id = taskItemId,
            Title = "Test Task",
            AssignedUserId = userA.Id,
            State = TaskState.InProgress,
            TaskAssignmentRecords = [
                new TaskAssignmentRecord { TaskItemId = taskItemId, UserId = userA.Id }
            ]
        };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([userA, userB]);

        _taskItemRepositoryMock.Setup(x => x.GetActiveTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([task]);

        _taskAssignmentRecordRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<TaskAssignmentRecord>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _taskItemRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ProcessReassignmentsAsync(CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(task.State, Is.EqualTo(TaskState.InProgress));
            Assert.That(task.AssignedUserId, Is.EqualTo(userB.Id));
        });

        _taskAssignmentRecordRepositoryMock.Verify(x => x.CreateAsync(
            It.Is<TaskAssignmentRecord>(r => r.UserId == userB.Id && r.TaskItemId == task.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _taskItemRepositoryMock.Verify(x => x.UpdateAsync(
            It.Is<TaskItem>(t => t.AssignedUserId == userB.Id && t.State == TaskState.InProgress),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ProcessReassignmentsAsync_ThreeUsersExist_PreviouslyAssignedUserAndAssignedUserAreExcluded()
    {
        // Arrange
        var taskItemId = Guid.NewGuid();

        var userA = new User { Id = Guid.NewGuid(), Name = "User A" };
        var userB = new User { Id = Guid.NewGuid(), Name = "User B" };
        var userC = new User { Id = Guid.NewGuid(), Name = "User C" };
        var allUsers = new List<User> { userA, userB, userC };

        var task = new TaskItem
        {
            Id = taskItemId,
            Title = "Test Task",
            AssignedUserId = userA.Id,
            PreviouslyAssignedUserId = null,
            State = TaskState.InProgress,
            TaskAssignmentRecords = [
                new TaskAssignmentRecord { TaskItemId = taskItemId, UserId = userA.Id }
            ]
        };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(allUsers);

        _taskItemRepositoryMock.Setup(x => x.GetActiveTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([task]);

        _taskItemRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Callback<TaskItem, CancellationToken>((updatedTask, _) => { });

        _taskAssignmentRecordRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<TaskAssignmentRecord>(), It.IsAny<CancellationToken>()))
            .Callback<TaskAssignmentRecord, CancellationToken>((record, _) =>
            {
                task.TaskAssignmentRecords.Add(record);
            })
            .Returns(Task.CompletedTask);


        // Act – First reassignment (User A → User B)
        await _service.ProcessReassignmentsAsync(CancellationToken.None);
        var firstAssigned = task.AssignedUserId;

        // Act – Second reassignment (User B → User C; A is previous, B is current → only C is eligible)
        await _service.ProcessReassignmentsAsync(CancellationToken.None);
        var secondAssigned = task.AssignedUserId;

        // Act – Third reassignment (all 3 used → task completed)
        await _service.ProcessReassignmentsAsync(CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(task.State, Is.EqualTo(TaskState.Completed));
            Assert.That(task.AssignedUserId, Is.Null);
        });

        _taskAssignmentRecordRepositoryMock.Verify(x => x.CreateAsync(
            It.IsAny<TaskAssignmentRecord>(),
            It.IsAny<CancellationToken>()), Times.Exactly(2));

        _taskItemRepositoryMock.Verify(x => x.UpdateAsync(
            It.IsAny<TaskItem>(),
            It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Test]
    public async Task ProcessReassignmentsAsync_MarksTaskCompleted_WhenAllUsersAssigned()
    {
        // Arrange
        var user1 = new User { Id = Guid.NewGuid() };
        var user2 = new User { Id = Guid.NewGuid() };

        var taskitemId = Guid.NewGuid();

        var task = new TaskItem
        {
            Id = taskitemId,
            State = TaskState.InProgress,
            AssignedUserId = user1.Id,
            PreviouslyAssignedUserId = user2.Id,
            TaskAssignmentRecords = [
                new() { TaskItemId = taskitemId, UserId = user1.Id },
                new() { TaskItemId = taskitemId, UserId = user2.Id },
            ]
        };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([user1, user2]);

        _taskItemRepositoryMock.Setup(x => x.GetActiveTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([task]);

        _taskItemRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ProcessReassignmentsAsync(CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(task.State, Is.EqualTo(TaskState.Completed));
            Assert.That(task.AssignedUserId, Is.Null);
            Assert.That(task.PreviouslyAssignedUserId, Is.Null);
        });

        _taskItemRepositoryMock.Verify(x => x.UpdateAsync(task,
            It.IsAny<CancellationToken>()), Times.Once);

        _taskAssignmentRecordRepositoryMock.Verify(x => x.CreateAsync(
            It.IsAny<TaskAssignmentRecord>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task ProcessReassignmentsAsync_CreatesAssignmentRecord_OnReassignment()
    {
        // Arrange
        var currentUser = new User { Id = Guid.NewGuid() };
        var eligibleUser = new User { Id = Guid.NewGuid() };

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            State = TaskState.InProgress,
            AssignedUserId = currentUser.Id,
            PreviouslyAssignedUserId = null,
            TaskAssignmentRecords = new List<TaskAssignmentRecord>
        {
            new() { TaskItemId = Guid.NewGuid(), UserId = currentUser.Id }
        }
        };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { currentUser, eligibleUser });

        _taskItemRepositoryMock.Setup(x => x.GetActiveTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem> { task });

        _taskAssignmentRecordRepositoryMock.Setup(x =>
            x.CreateAsync(It.IsAny<TaskAssignmentRecord>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _taskItemRepositoryMock.Setup(x =>
            x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ProcessReassignmentsAsync(CancellationToken.None);

        // Assert
        _taskAssignmentRecordRepositoryMock.Verify(x => x.CreateAsync(
            It.Is<TaskAssignmentRecord>(x => x.TaskItemId == task.Id && x.UserId == eligibleUser.Id),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ProcessReassignmentsAsync_UpdatesTask_AfterReassignment()
    {
        // Arrange
        var currentUser = new User { Id = Guid.NewGuid() };
        var eligibleUser = new User { Id = Guid.NewGuid() };

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            State = TaskState.InProgress,
            AssignedUserId = currentUser.Id,
            TaskAssignmentRecords = new List<TaskAssignmentRecord>
        {
            new() { TaskItemId = Guid.NewGuid(), UserId = currentUser.Id }
        }
        };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { currentUser, eligibleUser });

        _taskItemRepositoryMock.Setup(x => x.GetActiveTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem> { task });

        _taskAssignmentRecordRepositoryMock.Setup(x =>
            x.CreateAsync(It.IsAny<TaskAssignmentRecord>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _taskItemRepositoryMock.Setup(x =>
            x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ProcessReassignmentsAsync(CancellationToken.None);

        // Assert
        _taskItemRepositoryMock.Verify(x => x.UpdateAsync(task,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ProcessReassignmentsAsync_PutsTaskInWaitingState_WhenNoUsersExist()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            State = TaskState.InProgress,
            AssignedUserId = Guid.NewGuid(),
            PreviouslyAssignedUserId = Guid.NewGuid(),
        };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _taskItemRepositoryMock.Setup(x => x.GetActiveTasksAsync(It.IsAny<CancellationToken>())).ReturnsAsync([task]);

        // Act
        await _service.ProcessReassignmentsAsync(CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(task.State, Is.EqualTo(TaskState.Waiting));
            Assert.That(task.AssignedUserId, Is.Null);
        });

        _taskItemRepositoryMock.Verify(x => x.UpdateAsync(
            It.Is<TaskItem>(t => t.State == TaskState.Waiting && t.AssignedUserId == null),
            It.IsAny<CancellationToken>()), Times.Once);

        _taskAssignmentRecordRepositoryMock.Verify(x => x.CreateAsync(
            It.IsAny<TaskAssignmentRecord>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}

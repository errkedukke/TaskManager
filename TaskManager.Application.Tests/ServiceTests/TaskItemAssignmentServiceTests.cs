using Moq;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Application.Service;
using TaskManager.Domain;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tests.ServiceTests;

[TestFixture]
public class TaskItemAssignmentServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<ITaskItemRepository> _taskItemRepositoryMock = null!;
    private Mock<ITaskAssignmentRecordRepository> _taskAssignmentRecordRepositoryMock = null!;
    private TaskItemAssignmentService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _taskItemRepositoryMock = new Mock<ITaskItemRepository>();
        _taskAssignmentRecordRepositoryMock = new Mock<ITaskAssignmentRecordRepository>();

        _service = new TaskItemAssignmentService(_userRepositoryMock.Object, _taskItemRepositoryMock.Object, _taskAssignmentRecordRepositoryMock.Object);
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

        _userRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        _taskAssignmentRecordRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<TaskAssignmentRecord>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _taskItemRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.AssignInitialUserAsync(task, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(task.AssignedUserId, Is.EqualTo(user.Id));
            Assert.That(task.State, Is.EqualTo(TaskState.InProgress));
        });

        _taskAssignmentRecordRepositoryMock.Verify(x =>
            x.CreateAsync(It.Is<TaskAssignmentRecord>(x =>
                x.TaskItemId == task.Id && x.UserId == user.Id), It.IsAny<CancellationToken>()), Times.Once);

        _taskItemRepositoryMock.Verify(x => x.UpdateAsync(task, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task AssignInitialUserAsync_SetsTaskToWaiting_WhenNoUsersExist()
    {
        // Arrange
        var task = new TaskItem { Id = Guid.NewGuid() };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

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

        _taskItemRepositoryMock.Verify(x =>
            x.UpdateAsync(task, It.IsAny<CancellationToken>()), Times.Once);

        _taskAssignmentRecordRepositoryMock.Verify(x =>
            x.CreateAsync(It.IsAny<TaskAssignmentRecord>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task AssignInitialUserAsync_CreatesTaskAssignmentRecord()
    {
        // Arrange
        var task = new TaskItem { Id = Guid.NewGuid() };
        var user = new User { Id = Guid.NewGuid(), Name = "Test User" };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { user });

        _taskAssignmentRecordRepositoryMock.Setup(x =>
            x.CreateAsync(It.IsAny<TaskAssignmentRecord>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _taskItemRepositoryMock.Setup(x =>
            x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.AssignInitialUserAsync(task, CancellationToken.None);

        // Assert
        _taskAssignmentRecordRepositoryMock.Verify(x => x.CreateAsync(It.Is<TaskAssignmentRecord>(r => r.TaskItemId == task.Id && r.UserId == user.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task AssignInitialUserAsync_UpdatesTaskInRepository()
    {
        // Arrange
        var task = new TaskItem { Id = Guid.NewGuid() };
        var user = new User { Id = Guid.NewGuid(), Name = "Test User" };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { user });

        _taskAssignmentRecordRepositoryMock.Setup(x =>
            x.CreateAsync(It.IsAny<TaskAssignmentRecord>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _taskItemRepositoryMock.Setup(x =>
            x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.AssignInitialUserAsync(task, CancellationToken.None);

        // Assert
        _taskItemRepositoryMock.Verify(x =>
            x.UpdateAsync(task, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task ProcessReassignmentsAsync_SkipsCompletedTasks()
    {
        // Arrange
        var completedTask = new TaskItem
        {
            Id = Guid.NewGuid(),
            State = TaskState.Completed
        };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { new User { Id = Guid.NewGuid() } });

        _taskItemRepositoryMock.Setup(x => x.GetActiveTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem> { completedTask });

        // Act
        await _service.ProcessReassignmentsAsync(CancellationToken.None);

        // Assert
        _taskItemRepositoryMock.Verify(x =>
            x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);

        _taskAssignmentRecordRepositoryMock.Verify(x =>
            x.CreateAsync(It.IsAny<TaskAssignmentRecord>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task ProcessReassignmentsAsync_MarksTaskCompleted_WhenAllUsersAssigned()
    {
        // Arrange
        var user1 = new User { Id = Guid.NewGuid() };
        var user2 = new User { Id = Guid.NewGuid() };

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            State = TaskState.InProgress,
            AssignedUserId = user1.Id,
            PreviouslyAssignedUserId = user2.Id,
            TaskAssignmentRecords = new List<TaskAssignmentRecord>
        {
            new() { TaskItemId = Guid.NewGuid(), UserId = user1.Id },
            new() { TaskItemId = Guid.NewGuid(), UserId = user2.Id }
        }
        };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { user1, user2 });

        _taskItemRepositoryMock.Setup(x => x.GetActiveTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem> { task });

        _taskItemRepositoryMock.Setup(x =>
            x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
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

        _taskItemRepositoryMock.Verify(x =>
            x.UpdateAsync(task, It.IsAny<CancellationToken>()),
            Times.Once);

        _taskAssignmentRecordRepositoryMock.Verify(x =>
            x.CreateAsync(It.IsAny<TaskAssignmentRecord>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task ProcessReassignmentsAsync_MarksTaskWaiting_WhenNoEligibleUsers()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            State = TaskState.InProgress,
            AssignedUserId = user.Id,
            PreviouslyAssignedUserId = user.Id,
            TaskAssignmentRecords = new List<TaskAssignmentRecord>
        {
            new() { TaskItemId = Guid.NewGuid(), UserId = user.Id }
        }
        };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { user });

        _taskItemRepositoryMock.Setup(x => x.GetActiveTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem> { task });

        _taskItemRepositoryMock.Setup(x =>
            x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ProcessReassignmentsAsync(CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(task.State, Is.EqualTo(TaskState.Waiting));
            Assert.That(task.AssignedUserId, Is.Null);
        });

        _taskItemRepositoryMock.Verify(x =>
            x.UpdateAsync(task, It.IsAny<CancellationToken>()),
            Times.Once);

        _taskAssignmentRecordRepositoryMock.Verify(x =>
            x.CreateAsync(It.IsAny<TaskAssignmentRecord>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task ProcessReassignmentsAsync_ReassignsToNewUser()
    {
        // Arrange
        var currentUser = new User { Id = Guid.NewGuid() };
        var newUser = new User { Id = Guid.NewGuid() };

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
            .ReturnsAsync(new List<User> { currentUser, newUser });

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
        Assert.Multiple(() =>
        {
            Assert.That(task.AssignedUserId, Is.EqualTo(newUser.Id));
            Assert.That(task.PreviouslyAssignedUserId, Is.EqualTo(currentUser.Id));
            Assert.That(task.State, Is.EqualTo(TaskState.InProgress));
        });

        _taskAssignmentRecordRepositoryMock.Verify(x =>
            x.CreateAsync(It.Is<TaskAssignmentRecord>(
                r => r.TaskItemId == task.Id && r.UserId == newUser.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _taskItemRepositoryMock.Verify(x =>
            x.UpdateAsync(task, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task ProcessReassignmentsAsync_DoesNotReassignToCurrentOrPreviousUser()
    {
        // Arrange
        var currentUser = new User { Id = Guid.NewGuid() };
        var previousUser = new User { Id = Guid.NewGuid() };
        var eligibleUser = new User { Id = Guid.NewGuid() };

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            State = TaskState.InProgress,
            AssignedUserId = currentUser.Id,
            PreviouslyAssignedUserId = previousUser.Id,
            TaskAssignmentRecords = new List<TaskAssignmentRecord>
        {
            new() { TaskItemId = Guid.NewGuid(), UserId = currentUser.Id },
            new() { TaskItemId = Guid.NewGuid(), UserId = previousUser.Id }
        }
        };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { currentUser, previousUser, eligibleUser });

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
        Assert.Multiple(() =>
        {
            Assert.That(task.AssignedUserId, Is.EqualTo(eligibleUser.Id));
            Assert.That(task.PreviouslyAssignedUserId, Is.EqualTo(currentUser.Id));
        });

        _taskAssignmentRecordRepositoryMock.Verify(x =>
            x.CreateAsync(It.Is<TaskAssignmentRecord>(
                r => r.UserId == eligibleUser.Id), It.IsAny<CancellationToken>()),
            Times.Once);
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
        _taskAssignmentRecordRepositoryMock.Verify(x =>
            x.CreateAsync(It.Is<TaskAssignmentRecord>(
                r => r.TaskItemId == task.Id && r.UserId == eligibleUser.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
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
        _taskItemRepositoryMock.Verify(x =>
            x.UpdateAsync(task, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task ProcessReassignmentsAsync_DoesNothing_WhenNoUsersExist()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            State = TaskState.InProgress,
            AssignedUserId = Guid.NewGuid()
        };

        _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        _taskItemRepositoryMock.Setup(x => x.GetActiveTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem> { task });

        // Act
        await _service.ProcessReassignmentsAsync(CancellationToken.None);

        // Assert
        _taskItemRepositoryMock.Verify(x =>
            x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);

        _taskAssignmentRecordRepositoryMock.Verify(x =>
            x.CreateAsync(It.IsAny<TaskAssignmentRecord>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

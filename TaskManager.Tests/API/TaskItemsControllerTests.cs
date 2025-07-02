using TaskManager.Domain.Enums;

namespace TaskManager.Tests.API;

[TestFixture]
public class TaskItemsControllerTests
{
    private Mock<IMediator> _mediatorMock = null!;
    private Mock<ILogger<TaskItemsController>> _loggerMock = null!;
    private TaskItemsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<TaskItemsController>>();
        _controller = new TaskItemsController(_loggerMock.Object, _mediatorMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mediatorMock = null!;
        _loggerMock = null!;
        _controller = null!;
    }

    [Test]
    public async Task GetTaskItem_ShouldReturnOk_WhenTaskExists()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var expectedTask = new TaskItemDto { Id = taskId, Title = "Test Task" };

        _mediatorMock.Setup(m => m.Send(It.Is<GetTaskItemQuery>(q => q.Id == taskId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _controller.GetTaskItem(taskId, CancellationToken.None);
        var okResult = result.Result as OkObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(expectedTask));
        });
    }

    [Test]
    public async Task GetTaskItem_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetTaskItemQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItemDto?)null);

        // Act
        var result = await _controller.GetTaskItem(taskId, CancellationToken.None);
        var notFoundResult = result.Result as NotFoundResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));
        });
    }

    [Test]
    public async Task GetTaskItem_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetTaskItemQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.GetTaskItem(Guid.NewGuid(), CancellationToken.None);
        var objectResult = result.Result as ObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult.Value, Is.EqualTo("An error occurred while processing your request."));
        });
    }

    [Test]
    public async Task GetTaskItems_ShouldReturnOk_WhenTasksExist()
    {
        // Arrange
        var tasks = new List<TaskItemDto>
        {
            new() { Id = Guid.NewGuid(), Title = "T1" },
            new() { Id = Guid.NewGuid(), Title = "T2" }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetTaskItemsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetTaskItems(CancellationToken.None);
        var okResult = result.Result as OkObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(tasks));
        });
    }

    [Test]
    public async Task GetTaskItems_ShouldReturnNoContent_WhenEmpty()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetTaskItemsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItemDto>());

        // Act
        var result = await _controller.GetTaskItems(CancellationToken.None);
        var noContentResult = result.Result as NoContentResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(noContentResult, Is.Not.Null);
            Assert.That(noContentResult!.StatusCode, Is.EqualTo(204));
        });
    }

    [Test]
    public async Task GetTaskItems_ShouldReturnNoContent_WhenResponseIsNull()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetTaskItemsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<TaskItemDto>?)null);

        // Act
        var result = await _controller.GetTaskItems(CancellationToken.None);
        var noContentResult = result.Result as NoContentResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(noContentResult, Is.Not.Null);
            Assert.That(noContentResult!.StatusCode, Is.EqualTo(204));
        });
    }

    [Test]
    public async Task GetTaskItems_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetTaskItemsQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.GetTaskItems(CancellationToken.None);
        var objectResult = result.Result as ObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult.Value, Is.EqualTo("An error occurred while processing your request."));
        });
    }

    [Test]
    public async Task CreateTask_ShouldReturnCreated_WhenSuccessful()
    {
        // Arrange
        var command = new CreateTaskItemCommand
        {
            Title = "New Task"
        };

        var newId = Guid.NewGuid();

        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newId);

        // Act
        var result = await _controller.CreateTask(command, CancellationToken.None);
        var createdResult = result.Result as CreatedAtActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult!.StatusCode, Is.EqualTo(201));
            Assert.That(createdResult.RouteValues?["id"], Is.EqualTo(newId));
            Assert.That(createdResult.Value, Is.EqualTo(newId));
        });
    }

    [Test]
    public async Task CreateTask_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var command = new CreateTaskItemCommand
        {
            Title = "Task"
        };

        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.CreateTask(command, CancellationToken.None);
        var objectResult = result.Result as ObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult.Value, Is.EqualTo("An error occurred while processing your request."));
        });
    }

    [Test]
    public async Task UpdateTask_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new UpdateTaskItemCommand
        {
            Id = taskId,
            Title = "Updated",
            State = Domain.Enums.TaskState.Completed
        };

        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

        // Act
        var result = await _controller.UpdateTask(taskId, command, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            Assert.That(((NoContentResult)result).StatusCode, Is.EqualTo(204));
        });
    }

    [Test]
    public async Task UpdateTaskItems_ShouldReturnBadRequest_WhenIdsDoNotMatch()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var taskIdBody = Guid.NewGuid();

        var command = new UpdateTaskItemCommand
        {
            Id = taskIdBody,
            Title = "Mismatched Title",
            State = TaskState.Completed,
            AssignedUserId = Guid.NewGuid()
        };

        // Act
        var result = await _controller.UpdateTask(taskId, command, CancellationToken.None);
        var badRequestResult = result as BadRequestObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("ID in URL does not match ID in body."));
        });
    }


    [Test]
    public async Task UpdateTask_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new UpdateTaskItemCommand
        {
            Id = taskId,
            Title = "Bad Update"
        };

        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.UpdateTask(taskId, command, CancellationToken.None);
        var objectResult = result as ObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult.Value, Is.EqualTo("An error occurred while processing your request."));
        });
    }

    [Test]
    public async Task DeleteTask_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mediatorMock.Setup(m => m.Send(It.Is<DeleteTaskItemCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _controller.DeleteTask(id, CancellationToken.None);
        var noContentResult = result as NoContentResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(noContentResult, Is.Not.Null);
            Assert.That(noContentResult!.StatusCode, Is.EqualTo(204));
        });
    }

    [Test]
    public async Task DeleteTask_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteTaskItemCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.DeleteTask(Guid.NewGuid(), CancellationToken.None);
        var objectResult = result as ObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult.Value, Is.EqualTo("An error occurred while processing your request."));
        });
    }
}

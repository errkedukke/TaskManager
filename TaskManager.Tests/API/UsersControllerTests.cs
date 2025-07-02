namespace TaskManager.Tests.API;

[TestFixture]
public class UsersControllerTests
{
    private Mock<IMediator> _mediatorMock = null!;
    private Mock<ILogger<UsersController>> _loggerMock = null!;
    private UsersController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<UsersController>>();
        _controller = new UsersController(_loggerMock.Object, _mediatorMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mediatorMock = null!;
        _loggerMock = null!;
        _controller = null!;
    }

    [Test]
    public async Task GetUser_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new UserDto { Id = userId, Name = "Test User" };

        _mediatorMock.Setup(m => m.Send(It.Is<GetUserQuery>(q => q.Id == userId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _controller.GetUser(userId, CancellationToken.None);
        var okResult = result.Result as OkObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(expectedUser));
        });
    }

    [Test]
    public async Task GetUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mediatorMock.Setup(m => m.Send(It.Is<GetUserQuery>(q => q.Id == userId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDto?)null);

        // Act
        var result = await _controller.GetUser(userId, CancellationToken.None);
        var notFoundResult = result.Result as NotFoundResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));
        });
    }

    [Test]
    public async Task GetUser_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.GetUser(userId, CancellationToken.None);
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
    public async Task GetUsers_ShouldReturnOk_WhenUsersExist()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Alice" },
            new() { Id = Guid.NewGuid(), Name = "Bob" }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _controller.GetUsers(CancellationToken.None);
        var okResult = result.Result as OkObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(users));
        });
    }

    [Test]
    public async Task GetUsers_ShouldReturnNoContent_WhenListIsEmpty()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserDto>());

        // Act
        var result = await _controller.GetUsers(CancellationToken.None);
        var noContentResult = result.Result as NoContentResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(noContentResult, Is.Not.Null);
            Assert.That(noContentResult!.StatusCode, Is.EqualTo(204));
        });
    }

    [Test]
    public async Task GetUsers_ShouldReturnNoContent_WhenResponseIsNull()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<UserDto>?)null);

        // Act
        var result = await _controller.GetUsers(CancellationToken.None);
        var noContentResult = result.Result as NoContentResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(noContentResult, Is.Not.Null);
            Assert.That(noContentResult!.StatusCode, Is.EqualTo(204));
        });
    }

    [Test]
    public async Task GetUsers_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.GetUsers(CancellationToken.None);
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
    public async Task CreateUser_ShouldReturnCreated_WhenUserIsSuccessfullyCreated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateUserCommand { Name = "Test User" };

        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        // Act
        var result = await _controller.CreateUser(command, CancellationToken.None);
        var createdResult = result.Result as CreatedAtActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult!.StatusCode, Is.EqualTo(201));
            Assert.That(createdResult.RouteValues?["id"], Is.EqualTo(userId));
            Assert.That(createdResult.Value, Is.EqualTo(userId));
        });
    }

    [Test]
    public async Task CreateUser_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var command = new CreateUserCommand { Name = "Test User" };

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Simulated error"));

        // Act
        var result = await _controller.CreateUser(command, CancellationToken.None);
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
    public async Task UpdateUser_ShouldReturnNoContent_WhenUpdateIsSuccessful()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand { Id = userId, Name = "Updated Name" };

        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _controller.UpdateUser(userId, command, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            Assert.That(((NoContentResult)result).StatusCode, Is.EqualTo(204));
        });
    }

    [Test]
    public async Task UpdateUser_ShouldReturnBadRequest_WhenIdsDoNotMatch()
    {
        // Arrange
        var routeId = Guid.NewGuid();
        var bodyId = Guid.NewGuid(); // Different from routeId

        var command = new UpdateUserCommand { Id = bodyId, Name = "Mismatch User" };

        // Act
        var result = await _controller.UpdateUser(routeId, command, CancellationToken.None);
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
    public async Task UpdateUser_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand { Id = userId, Name = "Failing Update" };

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.UpdateUser(userId, command, CancellationToken.None);
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
    public async Task DeleteUser_ShouldReturnNoContent_WhenDeletionIsSuccessful()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mediatorMock.Setup(m => m.Send(It.Is<DeleteUserCommand>(c => c.Id == userId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _controller.DeleteUser(userId, CancellationToken.None);
        var noContentResult = result as NoContentResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(noContentResult, Is.Not.Null);
            Assert.That(noContentResult!.StatusCode, Is.EqualTo(204));
        });
    }

    [Test]
    public async Task DeleteUser_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.DeleteUser(userId, CancellationToken.None);
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

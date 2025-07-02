namespace TaskManager.Tests.API;

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
            .ThrowsAsync(new Exception("Unexpected error"));

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
}

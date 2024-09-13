using Microsoft.Extensions.Logging;

namespace NimblePros.MediatR.Contrib.Test.Behaviors;

public class LoggingBehaviorTests
{
  private readonly Mock<INext> _nextMock;
  private readonly Mock<ILogger<TestCommand>> _loggerMock;
  private readonly LoggingBehavior<TestCommand, bool> _loggingBehavior;

  private TestCommand? _command;

  private const string TestName = "Test name";

  public LoggingBehaviorTests()
  {
    _nextMock = new Mock<INext>();
    _loggerMock = new Mock<ILogger<TestCommand>>();
    _loggingBehavior = new LoggingBehavior<TestCommand, bool>(_loggerMock.Object);
  }

  [Fact]
  public async Task Should_ReturnSuccess_GivenValidInput()
  {
    // Arrange
    _command = new TestCommand() { Name = TestName };

    _nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await _loggingBehavior.Handle(_command, _nextMock.Object.Next,
      CancellationToken.None);

    // Assert
    isSuccess.Should().BeTrue();

    _nextMock.Verify(m => m.Next(), Times.Once());
    _nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ThrowArgumentNullException_GivenNullInput()
  {
#nullable disable
    // Arrange
    _command = null;

    _nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var testCode = () =>
      _loggingBehavior.Handle(_command, _nextMock.Object.Next, CancellationToken.None);

    // Assert
    await testCode
      .Should().ThrowAsync<ArgumentNullException>()
      .WithMessage("Value cannot be null. (Parameter 'request')");

    _nextMock.Verify(m => m.Next(), Times.Never());
    _nextMock.VerifyNoOtherCalls();
#nullable enable
  }
}

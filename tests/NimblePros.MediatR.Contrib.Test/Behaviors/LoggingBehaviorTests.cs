using Microsoft.Extensions.Logging;

namespace NimblePros.MediatR.Contrib.Test.Behaviors;

public class LoggingBehaviorTests
{
  [Fact]
  public async Task Should_ReturnSuccess_GivenValidInput()
  {
    // Arrange
    var nextMock = new Mock<INext>();
    var loggerMock = new Mock<ILogger<TestCommand>>();
    var command = new TestCommand() { Name = "Test name" };
    var loggingBehavior = new LoggingBehavior<TestCommand, bool>(loggerMock.Object);

    nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await loggingBehavior.Handle(command, nextMock.Object.Next,
      CancellationToken.None);

    // Assert
    Assert.True(isSuccess);

    nextMock.Verify(m => m.Next(), Times.Once);
    nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ThrowArgumentNullException_GivenNullInput()
  {
#nullable disable
    // Arrange
    var nextMock = new Mock<INext>();
    var loggerMock = new Mock<ILogger<TestCommand>>();
    var loggingBehavior = new LoggingBehavior<TestCommand, bool>(loggerMock.Object);
    TestCommand command = null;

    nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    Task<bool> testCode() => loggingBehavior.Handle(command, nextMock.Object.Next,
      CancellationToken.None);

    // Assert
    var exception = await Assert.ThrowsAsync<ArgumentNullException>(testCode);
    Assert.Equal("Value cannot be null. (Parameter 'request')", exception.Message);

    nextMock.Verify(m => m.Next(), Times.Never);
    nextMock.VerifyNoOtherCalls();
#nullable enable
  }
}

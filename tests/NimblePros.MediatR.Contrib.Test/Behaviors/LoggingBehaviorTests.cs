using Microsoft.Extensions.Logging;
using Moq;

namespace NimblePros.MediatR.Contrib.Test.Behaviors;

public class LoggingBehaviorTests
{
  [Fact]
  public async Task Should_ReturnSuccess_GivenValidInput()
  {
    // Arrange
    var loggerMock = new Mock<ILogger<TestCommand>>();
    var command = new TestCommand() { Name = "Test name" };
    var loggingBehavior = new LoggingBehavior<TestCommand, bool>(loggerMock.Object);

    // Act
    var isSuccess = await loggingBehavior.Handle(command, Pipeline.Next, CancellationToken.None);

    // Assert
    Assert.True(isSuccess);
  }

  [Fact]
  public async Task Should_ThrowArgumentNullException_GivenNullInput()
  {
#nullable disable
    // Arrange
    var loggerMock = new Mock<ILogger<TestCommand>>();
    var loggingBehavior = new LoggingBehavior<TestCommand, bool>(loggerMock.Object);
    TestCommand command = null;

    // Act
    Task<bool> testCode() => loggingBehavior.Handle(command, Pipeline.Next, CancellationToken.None);

    // Assert
    var exception = await Assert.ThrowsAsync<ArgumentNullException>(testCode);
    Assert.Equal("Value cannot be null. (Parameter 'request')", exception.Message);
#nullable enable
  }
}

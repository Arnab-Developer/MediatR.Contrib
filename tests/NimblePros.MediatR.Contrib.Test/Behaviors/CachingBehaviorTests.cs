using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace NimblePros.MediatR.Contrib.Test.Behaviors;

public class CachingBehaviorTests
{
  [Fact]
  public async Task Should_ReturnSuccess_GivenValidInputWithCacheMiss()
  {
    // Arrange
    var memoryCacheMock = new Mock<IMemoryCache>();
    var loggerMock = new Mock<ILogger<Mediator>>();
    var cacheEntryMock = new Mock<ICacheEntry>();
    var cachingBehavior = new CachingBehavior<TestCommand, bool>(memoryCacheMock.Object, loggerMock.Object);
    var command = new TestCommand() { Name = "Test name" };
    object? result;

    memoryCacheMock
      .Setup(m => m.TryGetValue("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand", out result))
      .Returns(false);

    memoryCacheMock
      .Setup(m => m.CreateEntry("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand"))
      .Returns(cacheEntryMock.Object);

    // Act
    var isSuccess = await cachingBehavior.Handle(command, Pipeline.Next, CancellationToken.None);

    // Assert
    Assert.True(isSuccess);

    memoryCacheMock
      .Verify(m => m.TryGetValue("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand", out result),
        Times.Once);

    memoryCacheMock
      .Verify(m => m.CreateEntry("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand"),
        Times.Once);
  }

  [Fact]
  public async Task Should_ReturnSuccess_GivenValidInputWithCacheHit()
  {
    // Arrange
    var memoryCacheMock = new Mock<IMemoryCache>();
    var loggerMock = new Mock<ILogger<Mediator>>();
    var cacheEntryMock = new Mock<ICacheEntry>();
    var cachingBehavior = new CachingBehavior<TestCommand, bool>(memoryCacheMock.Object, loggerMock.Object);
    var command = new TestCommand() { Name = "Test name" };
    object? result = true;

    memoryCacheMock
      .Setup(m => m.TryGetValue("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand", out result))
      .Returns(true);

    memoryCacheMock
      .Setup(m => m.CreateEntry("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand"))
      .Returns(cacheEntryMock.Object);

    // Act
    var isSuccess = await cachingBehavior.Handle(command, Pipeline.Next, CancellationToken.None);

    // Assert
    Assert.True(isSuccess);

    memoryCacheMock
      .Verify(m => m.TryGetValue("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand", out result),
        Times.Once);

    memoryCacheMock
      .Verify(m => m.CreateEntry("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand"),
        Times.Never);
  }

  [Fact]
  public async Task Should_ThrowArgumentNullException_GivenNullInput()
  {
#nullable disable
    // Arrange
    var memoryCacheMock = new Mock<IMemoryCache>();
    var loggerMock = new Mock<ILogger<Mediator>>();
    var cacheEntryMock = new Mock<ICacheEntry>();
    var cachingBehavior = new CachingBehavior<TestCommand, bool>(memoryCacheMock.Object, loggerMock.Object);
    TestCommand command = null;
    object result;

    memoryCacheMock
      .Setup(m => m.TryGetValue("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand", out result))
      .Returns(false);

    memoryCacheMock
      .Setup(m => m.CreateEntry("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand"))
      .Returns(cacheEntryMock.Object);

    // Act
    Task<bool> testCode() => cachingBehavior.Handle(command, Pipeline.Next, CancellationToken.None);

    // Assert
    var exception = await Assert.ThrowsAsync<ArgumentNullException>(testCode);
    Assert.Equal("Value cannot be null. (Parameter 'request')", exception.Message);

    memoryCacheMock
      .Verify(m => m.TryGetValue("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand", out result),
        Times.Never);

    memoryCacheMock
      .Verify(m => m.CreateEntry("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand"),
        Times.Never);

    memoryCacheMock.VerifyNoOtherCalls();
#nullable enable
  }
}

using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace NimblePros.MediatR.Contrib.Test.Behaviors;

public class CachingBehaviorTests
{
  [Fact]
  public async Task Should_ReturnSuccess_GivenValidInputWithCacheMiss()
  {
    // Arrange
    var nextMock = new Mock<INext>();
    var memoryCacheMock = new Mock<IMemoryCache>();
    var loggerMock = new Mock<ILogger<Mediator>>();
    var cacheEntryMock = new Mock<ICacheEntry>();

    var cachingBehavior = new CachingBehavior<TestCommand, bool>(memoryCacheMock.Object, loggerMock.Object);
    var command = new TestCommand() { Name = "Test name" };

    var cacheKey = "NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand";
    object? result;

    memoryCacheMock
      .Setup(m => m.TryGetValue(cacheKey, out result))
      .Returns(false);

    memoryCacheMock
      .Setup(m => m.CreateEntry(cacheKey))
      .Returns(cacheEntryMock.Object);

    nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await cachingBehavior.Handle(command, nextMock.Object.Next, CancellationToken.None);

    // Assert
    Assert.True(isSuccess);

    memoryCacheMock.Verify(m => m.TryGetValue(cacheKey, out result), Times.Once);
    memoryCacheMock.Verify(m => m.CreateEntry(cacheKey), Times.Once);

    nextMock.Verify(m => m.Next(), Times.Once);
    nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnSuccess_GivenValidInputWithCacheHit()
  {
    // Arrange
    var nextMock = new Mock<INext>();
    var memoryCacheMock = new Mock<IMemoryCache>();
    var loggerMock = new Mock<ILogger<Mediator>>();
    var cacheEntryMock = new Mock<ICacheEntry>();

    var cachingBehavior = new CachingBehavior<TestCommand, bool>(memoryCacheMock.Object, loggerMock.Object);
    var command = new TestCommand() { Name = "Test name" };

    var cacheKey = "NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand";
    object? result = true;

    memoryCacheMock
      .Setup(m => m.TryGetValue(cacheKey, out result))
      .Returns(true);

    memoryCacheMock
      .Setup(m => m.CreateEntry(cacheKey))
      .Returns(cacheEntryMock.Object);

    nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await cachingBehavior.Handle(command, nextMock.Object.Next, CancellationToken.None);

    // Assert
    Assert.True(isSuccess);

    memoryCacheMock.Verify(m => m.TryGetValue(cacheKey, out result), Times.Once);
    memoryCacheMock.Verify(m => m.CreateEntry(cacheKey), Times.Never);

    nextMock.Verify(m => m.Next(), Times.Never);
    nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ThrowArgumentNullException_GivenNullInput()
  {
#nullable disable
    // Arrange
    var nextMock = new Mock<INext>();
    var memoryCacheMock = new Mock<IMemoryCache>();
    var loggerMock = new Mock<ILogger<Mediator>>();
    var cacheEntryMock = new Mock<ICacheEntry>();

    var cachingBehavior = new CachingBehavior<TestCommand, bool>(memoryCacheMock.Object, loggerMock.Object);
    TestCommand command = null;

    var cacheKey = "NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand";
    object result;

    memoryCacheMock
      .Setup(m => m.TryGetValue(cacheKey, out result))
      .Returns(false);

    memoryCacheMock
      .Setup(m => m.CreateEntry(cacheKey))
      .Returns(cacheEntryMock.Object);

    nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    Task<bool> testCode() => cachingBehavior.Handle(command, nextMock.Object.Next, CancellationToken.None);

    // Assert
    var exception = await Assert.ThrowsAsync<ArgumentNullException>(testCode);
    Assert.Equal("Value cannot be null. (Parameter 'request')", exception.Message);

    memoryCacheMock.Verify(m => m.TryGetValue(cacheKey, out result), Times.Never);
    memoryCacheMock.Verify(m => m.CreateEntry(cacheKey), Times.Never);

    memoryCacheMock.VerifyNoOtherCalls();

    nextMock.Verify(m => m.Next(), Times.Never);
    nextMock.VerifyNoOtherCalls();
#nullable enable
  }
}

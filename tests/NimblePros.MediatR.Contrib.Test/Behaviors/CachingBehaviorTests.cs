using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace NimblePros.MediatR.Contrib.Test.Behaviors;

public class CachingBehaviorTests
{
  private readonly Mock<INext> _nextMock;
  private readonly Mock<IMemoryCache> _memoryCacheMock;
  private readonly Mock<ILogger<Mediator>> _loggerMock;
  private readonly Mock<ICacheEntry> _cacheEntryMock;
  private readonly CachingBehavior<TestCommand, bool> _cachingBehavior;

  private TestCommand? _command;

  private const string TestCacheKey =
    "NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.TestCommand";

  private const string TestName = "Test name";

  public CachingBehaviorTests()
  {
    _nextMock = new Mock<INext>();
    _memoryCacheMock = new Mock<IMemoryCache>();
    _loggerMock = new Mock<ILogger<Mediator>>();
    _cacheEntryMock = new Mock<ICacheEntry>();

    _cachingBehavior = new CachingBehavior<TestCommand, bool>(_memoryCacheMock.Object,
      _loggerMock.Object);
  }

  [Fact]
  public async Task Should_ReturnSuccess_GivenValidInputWithCacheMiss()
  {
    // Arrange
    _command = new TestCommand() { Name = TestName };
    object? result;

    _memoryCacheMock
      .Setup(m => m.TryGetValue(TestCacheKey, out result))
      .Returns(false);

    _memoryCacheMock
      .Setup(m => m.CreateEntry(TestCacheKey))
      .Returns(_cacheEntryMock.Object);

    _nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await _cachingBehavior.Handle(_command, _nextMock.Object.Next,
      CancellationToken.None);

    // Assert
    isSuccess.Should().BeTrue();

    _memoryCacheMock.Verify(m => m.TryGetValue(TestCacheKey, out result), Times.Once());
    _memoryCacheMock.Verify(m => m.CreateEntry(TestCacheKey), Times.Once());

    _nextMock.Verify(m => m.Next(), Times.Once());
    _nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnSuccess_GivenValidInputWithCacheHit()
  {
    // Arrange
    _command = new TestCommand() { Name = TestName };
    object? result = true;

    _memoryCacheMock
      .Setup(m => m.TryGetValue(TestCacheKey, out result))
      .Returns(true);

    _memoryCacheMock
      .Setup(m => m.CreateEntry(TestCacheKey))
      .Returns(_cacheEntryMock.Object);

    _nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await _cachingBehavior.Handle(_command, _nextMock.Object.Next,
      CancellationToken.None);

    // Assert
    isSuccess.Should().BeTrue();

    _memoryCacheMock.Verify(m => m.TryGetValue(TestCacheKey, out result), Times.Once());
    _memoryCacheMock.Verify(m => m.CreateEntry(TestCacheKey), Times.Never());

    _nextMock.Verify(m => m.Next(), Times.Never());
    _nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ThrowArgumentNullException_GivenNullInput()
  {
#nullable disable
    // Arrange
    _command = null;
    object result;

    _memoryCacheMock
      .Setup(m => m.TryGetValue(TestCacheKey, out result))
      .Returns(false);

    _memoryCacheMock
      .Setup(m => m.CreateEntry(TestCacheKey))
      .Returns(_cacheEntryMock.Object);

    _nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var testCode = () =>
      _cachingBehavior.Handle(_command, _nextMock.Object.Next, CancellationToken.None);

    // Assert
    await testCode
      .Should().ThrowAsync<ArgumentNullException>()
      .WithMessage("Value cannot be null. (Parameter 'request')");

    _memoryCacheMock.Verify(m => m.TryGetValue(TestCacheKey, out result), Times.Never());
    _memoryCacheMock.Verify(m => m.CreateEntry(TestCacheKey), Times.Never());

    _memoryCacheMock.VerifyNoOtherCalls();

    _nextMock.Verify(m => m.Next(), Times.Never());
    _nextMock.VerifyNoOtherCalls();
#nullable enable
  }
}

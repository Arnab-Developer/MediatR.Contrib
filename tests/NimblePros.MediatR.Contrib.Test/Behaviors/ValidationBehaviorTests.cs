using Ardalis.Result;
using FluentValidation;

namespace NimblePros.MediatR.Contrib.Test.Behaviors;

public class ValidationBehaviorTests
{
  [Fact]
  public async Task Should_ReturnSuccess_GivenValidInput()
  {
    // Arrange
    var nextMock = new Mock<INext>();
    var command = new TestCommand() { Name = "Test name" };
    var validators = new List<IValidator<TestCommand>>() { new TestCommandValidator() };
    var validationBehavior = new ValidationBehavior<TestCommand, bool>(validators);

    nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await validationBehavior.Handle(command, nextMock.Object.Next,
      CancellationToken.None);

    // Assert
    Assert.True(isSuccess);

    nextMock.Verify(m => m.Next(), Times.Once);
    nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ThrowValidationException_GivenEmptyStringInput()
  {
    // Arrange
    var nextMock = new Mock<INext>();
    var command = new TestCommand() { Name = string.Empty };
    var validators = new List<IValidator<TestCommand>>() { new TestCommandValidator() };
    var validationBehavior = new ValidationBehavior<TestCommand, bool>(validators);

    nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    Task<bool> testCode() => validationBehavior.Handle(command, nextMock.Object.Next,
      CancellationToken.None);

    // Assert
    var exception = await Assert.ThrowsAsync<ValidationException>(testCode);

    Assert.Equal(
      "Validation failed: \r\n -- Name: 'Name' must not be empty. Severity: Error",
      exception.Message);

    nextMock.Verify(m => m.Next(), Times.Never);
    nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnInvalidResult_GivenEmptyStringInput()
  {
    // Arrange
    var nextMock = new Mock<INext>();
    var command = new ResultTestCommand() { Name = string.Empty };
    var validators = new List<IValidator<ResultTestCommand>>() { new ResultTestCommandValidator() };
    var validationBehavior = new ValidationBehavior<ResultTestCommand, Result>(validators);

    nextMock
      .Setup(m => m.ResultNext())
      .ReturnsAsync(await Task.FromResult(Result.Success()));

    // Act
    var result = await validationBehavior.Handle(command, nextMock.Object.ResultNext,
      CancellationToken.None);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Single(result.ValidationErrors);
    Assert.Equal("'Name' must not be empty.", result.ValidationErrors[0].ErrorMessage);
    Assert.Equal("Ardalis.Result.Result", result.ValueType.FullName);

    nextMock.Verify(m => m.ResultNext(), Times.Never);
    nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnInvalidGenericResult_GivenEmptyStringInput()
  {
    // Arrange
    var nextMock = new Mock<INext>();
    var command = new GenericResultTestCommand() { Name = string.Empty };
    var validators = new List<IValidator<GenericResultTestCommand>>() { new GenericResultTestCommandValidator() };
    var validationBehavior = new ValidationBehavior<GenericResultTestCommand, Result<Value>>(validators);

    nextMock
      .Setup(m => m.GenericResultNext())
      .ReturnsAsync(await Task.FromResult(Result.Success(new Value())));

    // Act
    var result = await validationBehavior.Handle(command, nextMock.Object.GenericResultNext,
      CancellationToken.None);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Single(result.ValidationErrors);
    Assert.Equal("'Name' must not be empty.", result.ValidationErrors[0].ErrorMessage);
    Assert.Equal("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.Value", result.ValueType.FullName);

    nextMock.Verify(m => m.GenericResultNext(), Times.Never);
    nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnSuccess_GivenEmptyValidators()
  {
    // Arrange
    var nextMock = new Mock<INext>();
    var command = new TestCommand() { Name = "Test name" };
    var validators = new List<IValidator<TestCommand>>();
    var validationBehavior = new ValidationBehavior<TestCommand, bool>(validators);

    nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await validationBehavior.Handle(command, nextMock.Object.Next,
      CancellationToken.None);

    // Assert
    Assert.True(isSuccess);

    nextMock.Verify(m => m.Next(), Times.Once);
    nextMock.VerifyNoOtherCalls();
  }
}

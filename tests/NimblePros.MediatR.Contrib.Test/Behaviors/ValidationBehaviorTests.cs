using Ardalis.Result;
using FluentValidation;

namespace NimblePros.MediatR.Contrib.Test.Behaviors;

public class ValidationBehaviorTests
{
  [Fact]
  public async Task Should_ReturnSuccess_GivenValidInput()
  {
    // Arrange
    var validators = new List<IValidator<TestCommand>>() { new TestCommandValidator() };
    var validationBehavior = new ValidationBehavior<TestCommand, bool>(validators);
    var command = new TestCommand() { Name = "Test name" };
    var nextMock = new Mock<INext>();

    nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await validationBehavior.Handle(command, nextMock.Object.Next,
      CancellationToken.None);

    // Assert
    isSuccess.Should().BeTrue();

    nextMock.Verify(m => m.Next(), Times.Once());
    nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ThrowValidationException_GivenEmptyStringInput()
  {
    // Arrange
    var validators = new List<IValidator<TestCommand>>() { new TestCommandValidator() };
    var validationBehavior = new ValidationBehavior<TestCommand, bool>(validators);
    var command = new TestCommand() { Name = string.Empty };
    var nextMock = new Mock<INext>();

    nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    Task<bool> testCode() => validationBehavior.Handle(command, nextMock.Object.Next,
      CancellationToken.None);

    // Assert
    var exception = await Assert.ThrowsAsync<ValidationException>(testCode);

    exception.Message
      .Should()
      .Be("Validation failed: \r\n -- Name: 'Name' must not be empty. Severity: Error");

    nextMock.Verify(m => m.Next(), Times.Never());
    nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnInvalidResult_GivenEmptyStringInput()
  {
    // Arrange
    var validators = new List<IValidator<ResultTestCommand>>() { new ResultTestCommandValidator() };
    var validationBehavior = new ValidationBehavior<ResultTestCommand, Result>(validators);
    var command = new ResultTestCommand() { Name = string.Empty };
    var nextMock = new Mock<INext>();

    nextMock
      .Setup(m => m.ResultNext())
      .ReturnsAsync(await Task.FromResult(Result.Success()));

    // Act
    var result = await validationBehavior.Handle(command, nextMock.Object.ResultNext,
      CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.ValidationErrors.Should().ContainSingle();
    result.ValidationErrors[0].ErrorMessage.Should().Be("'Name' must not be empty.");
    result.ValueType.FullName.Should().Be("Ardalis.Result.Result");

    nextMock.Verify(m => m.ResultNext(), Times.Never());
    nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnInvalidGenericResult_GivenEmptyStringInput()
  {
    // Arrange
    var validators = new List<IValidator<GenericResultTestCommand>>() { new GenericResultTestCommandValidator() };
    var validationBehavior = new ValidationBehavior<GenericResultTestCommand, Result<Value>>(validators);
    var command = new GenericResultTestCommand() { Name = string.Empty };
    var nextMock = new Mock<INext>();

    nextMock
      .Setup(m => m.GenericResultNext())
      .ReturnsAsync(await Task.FromResult(Result.Success(new Value())));

    // Act
    var result = await validationBehavior.Handle(command, nextMock.Object.GenericResultNext,
      CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.ValidationErrors.Should().ContainSingle();
    result.ValidationErrors[0].ErrorMessage.Should().Be("'Name' must not be empty.");
    result.ValueType.FullName.Should().Be("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.Value");

    nextMock.Verify(m => m.GenericResultNext(), Times.Never());
    nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnSuccess_GivenEmptyValidators()
  {
    // Arrange
    var validators = new List<IValidator<TestCommand>>();
    var validationBehavior = new ValidationBehavior<TestCommand, bool>(validators);
    var command = new TestCommand() { Name = "Test name" };
    var nextMock = new Mock<INext>();

    nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await validationBehavior.Handle(command, nextMock.Object.Next,
      CancellationToken.None);

    // Assert
    isSuccess.Should().BeTrue();

    nextMock.Verify(m => m.Next(), Times.Once());
    nextMock.VerifyNoOtherCalls();
  }
}

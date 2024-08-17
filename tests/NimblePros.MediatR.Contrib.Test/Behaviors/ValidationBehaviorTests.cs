using Ardalis.Result;
using FluentValidation;

namespace NimblePros.MediatR.Contrib.Test.Behaviors;

public class ValidationBehaviorTests
{
  [Fact]
  public async Task Should_ReturnSuccess_GivenValidInput()
  {
    // Arrange
    var command = new TestCommand() { Name = "Test name" };
    var validators = new List<IValidator<TestCommand>>() { new TestCommandValidator() };
    var validationBehavior = new ValidationBehavior<TestCommand, bool>(validators);

    // Act
    var isSuccess = await validationBehavior.Handle(command, Pipeline.Next, CancellationToken.None);

    // Assert
    Assert.True(isSuccess);
  }

  [Fact]
  public async Task Should_ThrowValidationException_GivenEmptyStringInput()
  {
    // Arrange
    var command = new TestCommand() { Name = string.Empty };
    var validators = new List<IValidator<TestCommand>>() { new TestCommandValidator() };
    var validationBehavior = new ValidationBehavior<TestCommand, bool>(validators);

    // Act
    Task<bool> testCode() => validationBehavior.Handle(command, Pipeline.Next, CancellationToken.None);

    // Assert
    var exception = await Assert.ThrowsAsync<ValidationException>(testCode);
    Assert.Equal("Validation failed: \r\n -- Name: 'Name' must not be empty. Severity: Error", exception.Message);
  }

  [Fact]
  public async Task Should_ReturnInvalidResult_GivenEmptyStringInput()
  {
    // Arrange
    var command = new ResultTestCommand() { Name = string.Empty };
    var validators = new List<IValidator<ResultTestCommand>>() { new ResultTestCommandValidator() };
    var validationBehavior = new ValidationBehavior<ResultTestCommand, Result>(validators);

    // Act
    var result = await validationBehavior.Handle(command, Pipeline.ResultNext, CancellationToken.None);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Single(result.ValidationErrors);
    Assert.Equal("'Name' must not be empty.", result.ValidationErrors[0].ErrorMessage);
    Assert.Equal("Ardalis.Result.Result", result.ValueType.FullName);
  }

  [Fact]
  public async Task Should_ReturnInvalidGenericResult_GivenEmptyStringInput()
  {
    // Arrange
    var command = new GenericResultTestCommand() { Name = string.Empty };
    var validators = new List<IValidator<GenericResultTestCommand>>() { new GenericResultTestCommandValidator() };
    var validationBehavior = new ValidationBehavior<GenericResultTestCommand, Result<Value>>(validators);

    // Act
    var result = await validationBehavior.Handle(command, Pipeline.GenericResultNext, CancellationToken.None);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Single(result.ValidationErrors);
    Assert.Equal("'Name' must not be empty.", result.ValidationErrors[0].ErrorMessage);
    Assert.Equal("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.Value", result.ValueType.FullName);
  }
}

using Ardalis.Result;
using FluentValidation;
using NimblePros.MediatR.Contrib.Abstractions;
using NimblePros.MediatR.Contrib.Behaviors;
using Xunit;

namespace NimblePros.MediatR.Contrib.Test.Behaviors;

public class ValidationBehaviorTests
{
  [Fact]
  public async Task Can_Handle_ReturnSuccess_GivenValidInput()
  {
    // Arrange
    var testCommand = new TestCommand() { Name = "Test name" };
    var validators = new List<IValidator<TestCommand>>() { new TestCommandValidator() };
    var validationBehavior = new ValidationBehavior<TestCommand, bool>(validators);

    // Act
    var result = await validationBehavior.Handle(testCommand, Pipeline.Next, CancellationToken.None);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public async Task Can_Handle_ThrowValidationException_GivenInvalidInput()
  {
    // Arrange
    var testCommand = new TestCommand() { Name = string.Empty };
    var validators = new List<IValidator<TestCommand>>() { new TestCommandValidator() };
    var validationBehavior = new ValidationBehavior<TestCommand, bool>(validators);

    // Act
    Task<bool> testCode() => validationBehavior.Handle(testCommand, Pipeline.Next, CancellationToken.None);

    // Assert
    var exception = await Assert.ThrowsAsync<ValidationException>(testCode);
    Assert.Equal("Validation failed: \r\n -- Name: 'Name' must not be empty. Severity: Error", exception.Message);
  }

  [Fact]
  public async Task Can_Handle_ThrowInvalidResult_GivenInvalidInput()
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
  public async Task Can_Handle_ThrowInvalidGenericResult_GivenInvalidInput()
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
    Assert.Equal("NimblePros.MediatR.Contrib.Test.Behaviors.Value", result.ValueType.FullName);
  }
}

internal static class Pipeline
{
  public static async Task<bool> Next() => 
    await Task.FromResult(true);

  public static async Task<Result> ResultNext() => 
    await Task.FromResult(Result.Success());

  public static async Task<Result<Value>> GenericResultNext() => 
    await Task.FromResult(Result.Success(new Value()));
}

internal class TestCommand : ICommand<bool>
{
  public string Name { get; set; } = string.Empty;
}

internal class TestCommandValidator : AbstractValidator<TestCommand>
{
  public TestCommandValidator()
  {
    RuleFor(c => c.Name).NotEmpty();
  }
}

internal class ResultTestCommand : ICommand<Result>
{
  public string Name { get; set; } = string.Empty;
}

internal class ResultTestCommandValidator : AbstractValidator<ResultTestCommand>
{
  public ResultTestCommandValidator()
  {
    RuleFor(c => c.Name).NotEmpty();
  }
}

internal class Value { }

internal class GenericResultTestCommand : ICommand<Result<Value>>
{
  public string Name { get; set; } = string.Empty;
}

internal class GenericResultTestCommandValidator : AbstractValidator<GenericResultTestCommand>
{
  public GenericResultTestCommandValidator()
  {
    RuleFor(c => c.Name).NotEmpty();
  }
}

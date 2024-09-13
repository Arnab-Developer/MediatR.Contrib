using Ardalis.Result;
using FluentValidation;

namespace NimblePros.MediatR.Contrib.Test.Behaviors;

public class ValidationBehaviorTests
{
  private readonly TestCommandValidator _testCommandValidator;
  private readonly List<IValidator<TestCommand>> _testCommandValidators;
  private readonly ValidationBehavior<TestCommand, bool> _testCommandValidationBehavior;
  private readonly TestCommand _testCommand;

  private readonly ResultTestCommandValidator _resultTestCommandValidator;
  private readonly List<IValidator<ResultTestCommand>> _resultTestCommandValidators;
  private readonly ValidationBehavior<ResultTestCommand, Result> _resultTestCommandValidationBehavior;
  private readonly ResultTestCommand _resultTestcommand;

  private readonly GenericResultTestCommandValidator _genericResultTestCommandValidator;
  private readonly List<IValidator<GenericResultTestCommand>> _genericResultTestCommandValidators;
  private readonly ValidationBehavior<GenericResultTestCommand, Result<Value>> _genericResultTestCommandValidationBehavior;
  private readonly GenericResultTestCommand _genericResultTestcommand;

  private readonly Mock<INext> _nextMock;

  private const string TestName = "Test name";

  public ValidationBehaviorTests()
  {
    _testCommandValidator = new TestCommandValidator();
    _testCommandValidators = new List<IValidator<TestCommand>>();
    _testCommandValidationBehavior = new ValidationBehavior<TestCommand, bool>(_testCommandValidators);
    _testCommand = new TestCommand();

    _resultTestCommandValidator = new ResultTestCommandValidator();
    _resultTestCommandValidators = new List<IValidator<ResultTestCommand>>();
    _resultTestCommandValidationBehavior = new ValidationBehavior<ResultTestCommand, Result>(_resultTestCommandValidators);
    _resultTestcommand = new ResultTestCommand();

    _genericResultTestCommandValidator = new GenericResultTestCommandValidator();
    _genericResultTestCommandValidators = new List<IValidator<GenericResultTestCommand>>();
    _genericResultTestCommandValidationBehavior = new ValidationBehavior<GenericResultTestCommand, Result<Value>>(_genericResultTestCommandValidators);
    _genericResultTestcommand = new GenericResultTestCommand();

    _nextMock = new Mock<INext>();
  }

  [Fact]
  public async Task Should_ReturnSuccess_GivenValidInput()
  {
    // Arrange
    _testCommandValidators.Add(_testCommandValidator);
    _testCommand.Name = TestName;

    _nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await _testCommandValidationBehavior.Handle(_testCommand, _nextMock.Object.Next,
      CancellationToken.None);

    // Assert
    isSuccess.Should().BeTrue();

    _nextMock.Verify(m => m.Next(), Times.Once());
    _nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ThrowValidationException_GivenEmptyStringInput()
  {
    // Arrange
    _testCommandValidators.Add(_testCommandValidator);
    _testCommand.Name = string.Empty;

    _nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var testCode = () =>
      _testCommandValidationBehavior.Handle(_testCommand, _nextMock.Object.Next, CancellationToken.None);

    // Assert
    await testCode
      .Should().ThrowAsync<ValidationException>()
      .WithMessage("Validation failed: \r\n -- Name: 'Name' must not be empty. Severity: Error");

    _nextMock.Verify(m => m.Next(), Times.Never());
    _nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnInvalidResult_GivenEmptyStringInput()
  {
    // Arrange
    _resultTestCommandValidators.Add(_resultTestCommandValidator);
    _resultTestcommand.Name = string.Empty;

    _nextMock
      .Setup(m => m.ResultNext())
      .ReturnsAsync(await Task.FromResult(Result.Success()));

    // Act
    var result = await _resultTestCommandValidationBehavior.Handle(
      _resultTestcommand, _nextMock.Object.ResultNext, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.ValidationErrors.Should().ContainSingle();
    result.ValidationErrors[0].ErrorMessage.Should().Be("'Name' must not be empty.");
    result.ValueType.FullName.Should().Be("Ardalis.Result.Result");

    _nextMock.Verify(m => m.ResultNext(), Times.Never());
    _nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnInvalidGenericResult_GivenEmptyStringInput()
  {
    // Arrange
    _genericResultTestCommandValidators.Add(_genericResultTestCommandValidator);
    _genericResultTestcommand.Name = string.Empty;

    _nextMock
      .Setup(m => m.GenericResultNext())
      .ReturnsAsync(await Task.FromResult(Result.Success(new Value())));

    // Act
    var result = await _genericResultTestCommandValidationBehavior.Handle(
      _genericResultTestcommand, _nextMock.Object.GenericResultNext, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.ValidationErrors.Should().ContainSingle();
    result.ValidationErrors[0].ErrorMessage.Should().Be("'Name' must not be empty.");
    result.ValueType.FullName.Should().Be("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.Value");

    _nextMock.Verify(m => m.GenericResultNext(), Times.Never());
    _nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnSuccess_GivenEmptyValidators()
  {
    // Arrange
    _testCommand.Name = TestName;

    _nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await _testCommandValidationBehavior.Handle(_testCommand, _nextMock.Object.Next,
      CancellationToken.None);

    // Assert
    isSuccess.Should().BeTrue();

    _nextMock.Verify(m => m.Next(), Times.Once());
    _nextMock.VerifyNoOtherCalls();
  }
}

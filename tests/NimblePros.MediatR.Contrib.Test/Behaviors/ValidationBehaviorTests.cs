using Ardalis.Result;
using FluentValidation;
using FluentValidation.Results;

namespace NimblePros.MediatR.Contrib.Test.Behaviors;

public class ValidationBehaviorTests
{
  private readonly Mock<IValidator<ICommand<bool>>> _commandValidatorMock;
  private readonly List<IValidator<ICommand<bool>>> _commandValidators;
  private readonly ValidationBehavior<ICommand<bool>, bool> _commandValidationBehavior;
  private readonly Mock<ICommand<bool>> _commandMock;

  private readonly Mock<IValidator<ICommand<Result>>> _resultCommandValidatorMock;
  private readonly List<IValidator<ICommand<Result>>> _resultCommandValidators;
  private readonly ValidationBehavior<ICommand<Result>, Result> _resultCommandValidationBehavior;
  private readonly Mock<ICommand<Result>> _resultCommandMock;

  private readonly Mock<IValidator<ICommand<Result<Value>>>> _genericResultCommandValidatorMock;
  private readonly List<IValidator<ICommand<Result<Value>>>> _genericResultCommandValidators;
  private readonly ValidationBehavior<ICommand<Result<Value>>, Result<Value>> _genericResultCommandValidationBehavior;
  private readonly Mock<ICommand<Result<Value>>> _genericResultCommandMock;

  private readonly Mock<INext> _nextMock;

  private readonly ValidationFailure _validationFailure;
  private readonly List<ValidationFailure> _validationFailures;
  private ValidationResult? _validationResult;

  public ValidationBehaviorTests()
  {
    _commandValidatorMock = new Mock<IValidator<ICommand<bool>>>();
    _commandValidators = [];
    _commandValidationBehavior = new ValidationBehavior<ICommand<bool>, bool>(_commandValidators);
    _commandMock = new Mock<ICommand<bool>>();

    _resultCommandValidatorMock = new Mock<IValidator<ICommand<Result>>>();
    _resultCommandValidators = [];
    _resultCommandValidationBehavior = new ValidationBehavior<ICommand<Result>, Result>(_resultCommandValidators);
    _resultCommandMock = new Mock<ICommand<Result>>();

    _genericResultCommandValidatorMock = new Mock<IValidator<ICommand<Result<Value>>>>();
    _genericResultCommandValidators = [];
    _genericResultCommandValidationBehavior = new ValidationBehavior<ICommand<Result<Value>>, Result<Value>>(_genericResultCommandValidators);
    _genericResultCommandMock = new Mock<ICommand<Result<Value>>>();

    _nextMock = new Mock<INext>();

    _validationFailure = new ValidationFailure("Name", "'Name' must not be empty.");
    _validationFailures = [_validationFailure];
  }

  [Fact]
  public async Task Should_ReturnSuccess_GivenSuccessValidationResult()
  {
    // Arrange
    _commandValidators.Add(_commandValidatorMock.Object);
    _validationResult = new ValidationResult();

    _commandValidatorMock
      .Setup(m => m.ValidateAsync(It.IsAny<ValidationContext<ICommand<bool>>>(), CancellationToken.None))
      .ReturnsAsync(await Task.FromResult(_validationResult));

    _nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await _commandValidationBehavior.Handle(
      _commandMock.Object, _nextMock.Object.Next, CancellationToken.None);

    // Assert
    isSuccess.Should().BeTrue();

    _commandValidatorMock.Verify(
      m => m.ValidateAsync(It.IsAny<ValidationContext<ICommand<bool>>>(), CancellationToken.None),
      Times.Once());

    _commandValidatorMock.VerifyNoOtherCalls();

    _nextMock.Verify(m => m.Next(), Times.Once());
    _nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ThrowValidationException_GivenFailValidationResult()
  {
    // Arrange
    _commandValidators.Add(_commandValidatorMock.Object);
    _validationResult = new ValidationResult(_validationFailures);

    _commandValidatorMock
      .Setup(m => m.ValidateAsync(It.IsAny<ValidationContext<ICommand<bool>>>(), CancellationToken.None))
      .ReturnsAsync(await Task.FromResult(_validationResult));

    _nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var testCode = () => _commandValidationBehavior.Handle(
      _commandMock.Object, _nextMock.Object.Next, CancellationToken.None);

    // Assert
    await testCode
      .Should().ThrowAsync<ValidationException>()
      .WithMessage("Validation failed: \r\n -- Name: 'Name' must not be empty. Severity: Error");

    _commandValidatorMock.Verify(
      m => m.ValidateAsync(It.IsAny<ValidationContext<ICommand<bool>>>(), CancellationToken.None),
      Times.Once());

    _commandValidatorMock.VerifyNoOtherCalls();

    _nextMock.Verify(m => m.Next(), Times.Never());
    _nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnInvalidResult_GivenFailValidationResult()
  {
    // Arrange
    _resultCommandValidators.Add(_resultCommandValidatorMock.Object);
    _validationResult = new ValidationResult(_validationFailures);

    _resultCommandValidatorMock
      .Setup(m => m.ValidateAsync(It.IsAny<ValidationContext<ICommand<Result>>>(), CancellationToken.None))
      .ReturnsAsync(await Task.FromResult(_validationResult));

    _nextMock
      .Setup(m => m.ResultNext())
      .ReturnsAsync(await Task.FromResult(Result.Success()));

    // Act
    var result = await _resultCommandValidationBehavior.Handle(
      _resultCommandMock.Object, _nextMock.Object.ResultNext, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.ValidationErrors.Should().ContainSingle();
    result.ValidationErrors[0].ErrorMessage.Should().Be("'Name' must not be empty.");
    result.ValueType.FullName.Should().Be("Ardalis.Result.Result");

    _resultCommandValidatorMock.Verify(
      m => m.ValidateAsync(It.IsAny<ValidationContext<ICommand<Result>>>(), CancellationToken.None),
      Times.Once());

    _resultCommandValidatorMock.VerifyNoOtherCalls();

    _nextMock.Verify(m => m.ResultNext(), Times.Never());
    _nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnInvalidGenericResult_GivenFailValidationResult()
  {
    // Arrange
    _genericResultCommandValidators.Add(_genericResultCommandValidatorMock.Object);
    _validationResult = new ValidationResult(_validationFailures);

    _genericResultCommandValidatorMock
      .Setup(m => m.ValidateAsync(It.IsAny<ValidationContext<ICommand<Result<Value>>>>(), CancellationToken.None))
      .ReturnsAsync(await Task.FromResult(_validationResult));

    _nextMock
      .Setup(m => m.GenericResultNext())
      .ReturnsAsync(await Task.FromResult(Result.Success(new Value())));

    // Act
    var result = await _genericResultCommandValidationBehavior.Handle(
      _genericResultCommandMock.Object, _nextMock.Object.GenericResultNext, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.ValidationErrors.Should().ContainSingle();
    result.ValidationErrors[0].ErrorMessage.Should().Be("'Name' must not be empty.");
    result.ValueType.FullName.Should().Be("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.Value");

    _genericResultCommandValidatorMock.Verify(
      m => m.ValidateAsync(It.IsAny<ValidationContext<ICommand<Result<Value>>>>(), CancellationToken.None),
      Times.Once());

    _genericResultCommandValidatorMock.VerifyNoOtherCalls();

    _nextMock.Verify(m => m.GenericResultNext(), Times.Never());
    _nextMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnSuccess_GivenEmptyValidators()
  {
    // Arrange
    _nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await _commandValidationBehavior.Handle(
      _commandMock.Object, _nextMock.Object.Next, CancellationToken.None);

    // Assert
    isSuccess.Should().BeTrue();

    _commandValidatorMock.Verify(
      m => m.ValidateAsync(It.IsAny<ValidationContext<ICommand<bool>>>(), CancellationToken.None),
      Times.Never());

    _commandValidatorMock.VerifyNoOtherCalls();

    _nextMock.Verify(m => m.Next(), Times.Once());
    _nextMock.VerifyNoOtherCalls();
  }
}

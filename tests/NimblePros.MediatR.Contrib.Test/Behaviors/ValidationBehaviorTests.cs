using Ardalis.Result;
using FluentValidation;
using FluentValidation.Results;

namespace NimblePros.MediatR.Contrib.Test.Behaviors;

public class ValidationBehaviorTests
{
  private readonly Mock<IValidator<ICommand<bool>>> _testCommandValidatorMock;
  private readonly List<IValidator<ICommand<bool>>> _testCommandValidators;
  private readonly ValidationBehavior<ICommand<bool>, bool> _testCommandValidationBehavior;
  private readonly Mock<ICommand<bool>> _testCommandMock;

  private readonly Mock<IValidator<ICommand<Result>>> _resultTestCommandValidatorMock;
  private readonly List<IValidator<ICommand<Result>>> _resultTestCommandValidators;
  private readonly ValidationBehavior<ICommand<Result>, Result> _resultTestCommandValidationBehavior;
  private readonly Mock<ICommand<Result>> _resultTestcommandMock;

  private readonly Mock<IValidator<ICommand<Result<Value>>>> _genericResultTestCommandValidatorMock;
  private readonly List<IValidator<ICommand<Result<Value>>>> _genericResultTestCommandValidators;
  private readonly ValidationBehavior<ICommand<Result<Value>>, Result<Value>> _genericResultTestCommandValidationBehavior;
  private readonly Mock<ICommand<Result<Value>>> _genericResultTestcommandMock;

  private readonly ValidationResult _failedValidationResult;

  private readonly Mock<INext> _nextMock;

  public ValidationBehaviorTests()
  {
    _testCommandValidatorMock = new Mock<IValidator<ICommand<bool>>>();
    _testCommandValidators = new List<IValidator<ICommand<bool>>>();
    _testCommandValidationBehavior = new ValidationBehavior<ICommand<bool>, bool>(_testCommandValidators);
    _testCommandMock = new Mock<ICommand<bool>>();

    _resultTestCommandValidatorMock = new Mock<IValidator<ICommand<Result>>>();
    _resultTestCommandValidators = new List<IValidator<ICommand<Result>>>();
    _resultTestCommandValidationBehavior = new ValidationBehavior<ICommand<Result>, Result>(_resultTestCommandValidators);
    _resultTestcommandMock = new Mock<ICommand<Result>>();

    _genericResultTestCommandValidatorMock = new Mock<IValidator<ICommand<Result<Value>>>>();
    _genericResultTestCommandValidators = new List<IValidator<ICommand<Result<Value>>>>();
    _genericResultTestCommandValidationBehavior = new ValidationBehavior<ICommand<Result<Value>>, Result<Value>>(_genericResultTestCommandValidators);
    _genericResultTestcommandMock = new Mock<ICommand<Result<Value>>>();

    _failedValidationResult = new ValidationResult(
      new List<ValidationFailure>()
      {
        new ValidationFailure("Name", "'Name' must not be empty.")
      });

    _nextMock = new Mock<INext>();
  }

  [Fact]
  public async Task Should_ReturnSuccess_GivenSuccessValidationResult()
  {
    // Arrange
    _testCommandValidators.Add(_testCommandValidatorMock.Object);

    _testCommandValidatorMock
      .Setup(m => m.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(await Task.FromResult(new ValidationResult()));

    _nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await _testCommandValidationBehavior.Handle(_testCommandMock.Object, _nextMock.Object.Next,
      CancellationToken.None);

    // Assert
    isSuccess.Should().BeTrue();

    _nextMock.Verify(m => m.Next(), Times.Once());
    _nextMock.VerifyNoOtherCalls();

    _testCommandValidatorMock.Verify(m =>
      m.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()), Times.Once());

    _testCommandValidatorMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ThrowValidationException_GivenFailedValidationResultWithBoolValidator()
  {
    // Arrange
    _testCommandValidators.Add(_testCommandValidatorMock.Object);

    _testCommandValidatorMock
      .Setup(m => m.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(await Task.FromResult(_failedValidationResult));

    _nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var testCode = () =>
      _testCommandValidationBehavior.Handle(_testCommandMock.Object, _nextMock.Object.Next, CancellationToken.None);

    // Assert
    await testCode
      .Should().ThrowAsync<ValidationException>()
      .WithMessage("Validation failed: \r\n -- Name: 'Name' must not be empty. Severity: Error");

    _nextMock.Verify(m => m.Next(), Times.Never());
    _nextMock.VerifyNoOtherCalls();

    _testCommandValidatorMock.Verify(m =>
      m.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()), Times.Once());

    _testCommandValidatorMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnInvalidResult_GivenFailedValidationResultWithResultValidator()
  {
    // Arrange
    _resultTestCommandValidators.Add(_resultTestCommandValidatorMock.Object);

    _resultTestCommandValidatorMock
      .Setup(m => m.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(await Task.FromResult(_failedValidationResult));

    _nextMock
      .Setup(m => m.ResultNext())
      .ReturnsAsync(await Task.FromResult(Result.Success()));

    // Act
    var result = await _resultTestCommandValidationBehavior.Handle(
      _resultTestcommandMock.Object, _nextMock.Object.ResultNext, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.ValidationErrors.Should().ContainSingle();
    result.ValidationErrors[0].ErrorMessage.Should().Be("'Name' must not be empty.");
    result.ValueType.FullName.Should().Be("Ardalis.Result.Result");

    _nextMock.Verify(m => m.ResultNext(), Times.Never());
    _nextMock.VerifyNoOtherCalls();

    _resultTestCommandValidatorMock.Verify(m =>
      m.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()), Times.Once());

    _resultTestCommandValidatorMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnInvalidGenericResult_GivenFailedValidationResultWithGenericResultValidator()
  {
    // Arrange
    _genericResultTestCommandValidators.Add(_genericResultTestCommandValidatorMock.Object);

    _genericResultTestCommandValidatorMock
      .Setup(m => m.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(await Task.FromResult(_failedValidationResult));

    _nextMock
      .Setup(m => m.GenericResultNext())
      .ReturnsAsync(await Task.FromResult(Result.Success(new Value())));

    // Act
    var result = await _genericResultTestCommandValidationBehavior.Handle(
      _genericResultTestcommandMock.Object, _nextMock.Object.GenericResultNext, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.ValidationErrors.Should().ContainSingle();
    result.ValidationErrors[0].ErrorMessage.Should().Be("'Name' must not be empty.");
    result.ValueType.FullName.Should().Be("NimblePros.MediatR.Contrib.Test.Behaviors.Helpers.Value");

    _nextMock.Verify(m => m.GenericResultNext(), Times.Never());
    _nextMock.VerifyNoOtherCalls();

    _genericResultTestCommandValidatorMock.Verify(m =>
      m.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()), Times.Once());

    _genericResultTestCommandValidatorMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task Should_ReturnSuccess_GivenEmptyValidators()
  {
    // Arrange
    _nextMock
      .Setup(m => m.Next())
      .ReturnsAsync(await Task.FromResult(true));

    // Act
    var isSuccess = await _testCommandValidationBehavior.Handle(_testCommandMock.Object, _nextMock.Object.Next,
      CancellationToken.None);

    // Assert
    isSuccess.Should().BeTrue();

    _nextMock.Verify(m => m.Next(), Times.Once());
    _nextMock.VerifyNoOtherCalls();
  }
}

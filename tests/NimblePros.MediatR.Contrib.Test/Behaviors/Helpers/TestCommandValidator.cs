using FluentValidation;

namespace NimblePros.MediatR.Contrib.Test.Behaviors.Helpers;

internal class TestCommandValidator : AbstractValidator<TestCommand>
{
  public TestCommandValidator()
  {
    RuleFor(c => c.Name).NotEmpty();
  }
}

using FluentValidation;

namespace NimblePros.MediatR.Contrib.Test.Behaviors.Helpers;

internal class GenericResultTestCommandValidator : AbstractValidator<GenericResultTestCommand>
{
  public GenericResultTestCommandValidator()
  {
    RuleFor(c => c.Name).NotEmpty();
  }
}

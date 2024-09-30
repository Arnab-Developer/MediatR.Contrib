using FluentValidation;

namespace NimblePros.MediatR.Contrib.Test.Behaviors.Helpers;

internal class ResultTestCommandValidator : AbstractValidator<ResultTestCommand>
{
  public ResultTestCommandValidator()
  {
    RuleFor(c => c.Name).NotEmpty();
  }
}

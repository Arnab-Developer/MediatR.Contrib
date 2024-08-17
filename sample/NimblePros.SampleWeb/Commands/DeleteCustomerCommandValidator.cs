using FluentValidation;

namespace NimblePros.SampleWeb.Commands;

public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
  public DeleteCustomerCommandValidator()
  {
    RuleFor(c => c.Id).GreaterThan(0);
  }
}

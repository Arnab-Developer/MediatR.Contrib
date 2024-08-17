using FluentValidation;

namespace NimblePros.SampleWeb.Commands;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
  public UpdateCustomerCommandValidator()
  {
    RuleFor(c => c.Id).GreaterThan(0);
    RuleFor(c => c.FirstName).NotEmpty();
    RuleFor(c => c.LastName).NotEmpty();
    RuleFor(c => c.Country).NotEmpty();
  }
}

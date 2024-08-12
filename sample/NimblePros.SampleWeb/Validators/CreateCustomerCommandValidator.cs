using FluentValidation;
using NimblePros.SampleWeb.Commands;

namespace NimblePros.SampleWeb.Validators;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
  public CreateCustomerCommandValidator()
  {
    RuleFor(c => c.FirstName).NotEmpty();
    RuleFor(c => c.LastName).NotEmpty();
    RuleFor(c => c.Country).NotEmpty();
  }
}

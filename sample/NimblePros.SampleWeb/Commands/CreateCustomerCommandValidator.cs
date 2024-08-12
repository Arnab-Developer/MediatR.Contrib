using FluentValidation;

namespace NimblePros.SampleWeb.Commands;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
  public CreateCustomerCommandValidator()
  {
    RuleFor(c => c.FirstName).NotEmpty();
    RuleFor(c => c.LastName).NotEmpty();
    RuleFor(c => c.Country).NotEmpty();
  }
}

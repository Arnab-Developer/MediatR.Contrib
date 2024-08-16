using FluentValidation;

namespace NimblePros.SampleWeb.Queries;

public class GetCustomerQueryValidator : AbstractValidator<GetCustomerQuery>
{
  public GetCustomerQueryValidator()
  {
    RuleFor(q => q.Id).GreaterThan(0);
  }
}

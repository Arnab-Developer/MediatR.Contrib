using NimblePros.MediatR.Contrib.Abstractions;
using NimblePros.SampleWeb.Endpoints.CreateCustomerEndpoint;
using NimblePros.SampleWeb.Models;

namespace NimblePros.SampleWeb.Commands;

public class CreateCustomerCommandHandler(CustomerContext context)
  : ICommandHandler<CreateCustomerCommand, CreateCustomerResponse>
{
  private readonly CustomerContext _context = context;

  public async Task<CreateCustomerResponse> Handle(
    CreateCustomerCommand request,
    CancellationToken cancellationToken)
  {
    var customer = new Customer()
    {
      FirstName = request.FirstName,
      LastName = request.LastName,
      Country = request.Country
    };

    await _context.Customers.AddAsync(customer, cancellationToken).ConfigureAwait(false);
    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    var response = new CreateCustomerResponse()
    {
      NewCustomerId = customer.Id,
      Message = $"New customer created with name '{customer.FullName}' from '{customer.Country}'"
    };

    return response;
  }
}

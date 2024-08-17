using Microsoft.EntityFrameworkCore;

namespace NimblePros.SampleWeb.Queries;

public class GetCustomerQueryHandler(CustomerContext context)
  : IQueryHandler<GetCustomerQuery, Customer>
{
  private readonly CustomerContext _context = context;

  public async Task<Customer> Handle(GetCustomerQuery request,
    CancellationToken cancellationToken)
  {
    var customer = await _context.Customers
      .FirstAsync(c => c.Id == request.Id, cancellationToken)
      .ConfigureAwait(false);

    return customer;
  }
}

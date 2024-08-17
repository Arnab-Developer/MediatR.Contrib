using Microsoft.EntityFrameworkCore;

namespace NimblePros.SampleWeb.Queries;

public class GetCustomersQueryHandler(CustomerContext context)
  : ICommandHandler<GetCustomersQuery, IEnumerable<Customer>>
{
  private readonly CustomerContext _context = context;

  public async Task<IEnumerable<Customer>> Handle(GetCustomersQuery request,
    CancellationToken cancellationToken)
  {
    var customers = await _context.Customers
      .OrderBy(c => c.Id)
      .ToListAsync(cancellationToken)
      .ConfigureAwait(false);

    return customers;
  }
}

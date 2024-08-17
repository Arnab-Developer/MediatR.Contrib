using Microsoft.EntityFrameworkCore;

namespace NimblePros.SampleWeb.Commands;

public class UpdateCustomerCommandHandler(CustomerContext context)
  : ICommandHandler<UpdateCustomerCommand, bool>
{
  private readonly CustomerContext _context = context;

  public async Task<bool> Handle(UpdateCustomerCommand request,
    CancellationToken cancellationToken)
  {
    var customer = await _context.Customers
      .FirstAsync(c => c.Id == request.Id, cancellationToken)
      .ConfigureAwait(false);

    customer.FirstName = request.FirstName;
    customer.LastName = request.LastName;
    customer.Country = request.Country;

    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    return true;
  }
}

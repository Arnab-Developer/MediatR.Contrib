using Microsoft.EntityFrameworkCore;

namespace NimblePros.SampleWeb.Commands;

public class DeleteCustomerCommandHandler(CustomerContext context)
  : ICommandHandler<DeleteCustomerCommand, bool>
{
  private readonly CustomerContext _context = context;

  public async Task<bool> Handle(DeleteCustomerCommand request, 
    CancellationToken cancellationToken)
  {
    var customer = await _context.Customers
      .FirstAsync(c => c.Id == request.Id, cancellationToken)
      .ConfigureAwait(false);

    _context.Customers.Remove(customer);
    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    return true;
  }
}

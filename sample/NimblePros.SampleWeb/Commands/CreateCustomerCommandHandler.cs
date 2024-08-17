namespace NimblePros.SampleWeb.Commands;

public class CreateCustomerCommandHandler(CustomerContext context)
  : ICommandHandler<CreateCustomerCommand, int>
{
  private readonly CustomerContext _context = context;

  public async Task<int> Handle(CreateCustomerCommand request,
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

    var newCustomerId = customer.Id;
    return newCustomerId;
  }
}

using NimblePros.MediatR.Contrib.Abstractions;
using NimblePros.SampleWeb.Endpoints.CreateCustomerEndpoint;

namespace NimblePros.SampleWeb.Commands;

public class CreateCustomerCommandHandler
  : ICommandHandler<CreateCustomerCommand, CreateCustomerResponse>
{
  public async Task<CreateCustomerResponse> Handle(
    CreateCustomerCommand request,
    CancellationToken cancellationToken)
  {
    var customerFullName = $"{request.FirstName} {request.LastName}";

    var response = new CreateCustomerResponse()
    {
      NewCustomerId = 1, // In real case this id will come from the db.
      Message = $"New customer created with name '{customerFullName}' from '{request.Country}'"
    };

    return await Task.FromResult(response).ConfigureAwait(false);
  }
}

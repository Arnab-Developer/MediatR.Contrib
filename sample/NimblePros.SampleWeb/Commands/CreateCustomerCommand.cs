using NimblePros.MediatR.Contrib.Abstractions;
using NimblePros.SampleWeb.Endpoints.CreateCustomerEndpoint;

namespace NimblePros.SampleWeb.Commands;

public class CreateCustomerCommand : ICommand<CreateCustomerResponse>
{
  public string FirstName { get; set; } = string.Empty;

  public string LastName { get; set; } = string.Empty;

  public string Country { get; set; } = string.Empty;
}

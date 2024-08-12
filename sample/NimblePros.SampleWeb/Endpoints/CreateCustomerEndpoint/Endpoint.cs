using FastEndpoints;
using MediatR;
using NimblePros.SampleWeb.Commands;

namespace NimblePros.SampleWeb.Endpoints.CreateCustomerEndpoint;

public class Endpoint(IMediator mediator)
  : Endpoint<CreateCustomerRequest, CreateCustomerResponse>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get("/customer");
    AllowAnonymous();
  }

  public override async Task HandleAsync(
    CreateCustomerRequest request,
    CancellationToken cancellationToken)
  {
    var createCustomerCommand = new CreateCustomerCommand()
    {
      FirstName = request.FirstName,
      LastName = request.LastName,
      Country = request.Country
    };

    var createCustomerResponse = await _mediator
      .Send(createCustomerCommand, cancellationToken)
      .ConfigureAwait(false);

    await SendAsync(createCustomerResponse, cancellation: cancellationToken)
      .ConfigureAwait(false);
  }
}

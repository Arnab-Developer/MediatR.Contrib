using FastEndpoints;
using FluentValidation;
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

    CreateCustomerResponse createCustomerResponse;

    try
    {
      createCustomerResponse = await _mediator
        .Send(createCustomerCommand, cancellationToken)
        .ConfigureAwait(false);
    }
    catch (ValidationException ex)
    {
      createCustomerResponse = new CreateCustomerResponse()
      {
        NewCustomerId = 0,
        Message = ex.Message
      };
    }

    await SendAsync(createCustomerResponse, cancellation: cancellationToken)
      .ConfigureAwait(false);
  }
}

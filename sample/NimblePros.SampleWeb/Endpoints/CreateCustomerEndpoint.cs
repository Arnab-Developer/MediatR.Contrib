using FastEndpoints;
using FluentValidation;
using MediatR;

namespace NimblePros.SampleWeb.Endpoints;

public class CreateCustomerEndpoint(IMediator mediator)
  : Endpoint<CreateCustomerRequest, CreateCustomerResponse>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get("/create-customer");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateCustomerRequest request,
    CancellationToken cancellationToken)
  {
    var createCustomerCommand = new CreateCustomerCommand()
    {
      FirstName = request.FirstName,
      LastName = request.LastName,
      Country = request.Country
    };

    CreateCustomerResponse createCustomerResponse;
    int statusCode;

    try
    {
      var newCustomerId = await _mediator.Send(createCustomerCommand, cancellationToken)
        .ConfigureAwait(false);

      createCustomerResponse = new CreateCustomerResponse()
      {
        NewCustomerId = newCustomerId,
        Message = $"New customer created with name '{request.FirstName} {request.LastName}' from '{request.Country}'"
      };

      statusCode = 200;
    }
    catch (ValidationException ex)
    {
      createCustomerResponse = new CreateCustomerResponse()
      {
        NewCustomerId = 0,
        Message = ex.Message
      };

      statusCode = 400;
    }

    await SendAsync(createCustomerResponse, statusCode, cancellationToken)
      .ConfigureAwait(false);
  }
}

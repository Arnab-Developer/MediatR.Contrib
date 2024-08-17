using FastEndpoints;
using FluentValidation;
using MediatR;

namespace NimblePros.SampleWeb.Endpoints;

public class GetCustomersEndpoint(IMediator mediator)
  : EndpointWithoutRequest<GetCustomersResponse>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get("/get-customers");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    var getCustomersQuery = new GetCustomersQuery() { };
    GetCustomersResponse getCustomersResponse;
    int statusCode;

    try
    {
      var customers = await _mediator.Send(getCustomersQuery, cancellationToken)
        .ConfigureAwait(false);

      getCustomersResponse = new GetCustomersResponse()
      {
        Names = customers.Select(c => c.FullName)
      };

      statusCode = 200;
    }
    catch (ValidationException)
    {
      getCustomersResponse = new GetCustomersResponse() { };
      statusCode = 400;
    }

    await SendAsync(getCustomersResponse, statusCode, cancellationToken)
      .ConfigureAwait(false);
  }
}

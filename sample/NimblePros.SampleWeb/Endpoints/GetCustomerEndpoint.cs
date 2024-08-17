using FastEndpoints;
using FluentValidation;
using MediatR;

namespace NimblePros.SampleWeb.Endpoints;

public class GetCustomerEndpoint(IMediator mediator)
  : Endpoint<GetCustomerRequest, GetCustomerResponse>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get("/get-customer");
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetCustomerRequest request,
    CancellationToken cancellationToken)
  {
    var getCustomerQuery = new GetCustomerQuery() { Id = request.Id };
    GetCustomerResponse getCustomerResponse;
    int statusCode;

    try
    {
      var customer = await _mediator.Send(getCustomerQuery, cancellationToken)
        .ConfigureAwait(false);

      getCustomerResponse = new GetCustomerResponse()
      {
        Id = customer.Id,
        Name = customer.FullName
      };

      statusCode = 200;
    }
    catch (ValidationException ex)
    {
      getCustomerResponse = new GetCustomerResponse()
      {
        Id = 0,
        Name = ex.Message
      };

      statusCode = 400;
    }

    await SendAsync(getCustomerResponse, statusCode, cancellationToken)
      .ConfigureAwait(false);
  }
}

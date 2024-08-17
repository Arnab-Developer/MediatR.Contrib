using FastEndpoints;
using FluentValidation;
using MediatR;

namespace NimblePros.SampleWeb.Endpoints;

public class UpdateCustomerEndpoint(IMediator mediator)
  : Endpoint<UpdateCustomerRequest, UpdateCustomerResponse>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get("/update-customer");
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateCustomerRequest request,
    CancellationToken cancellationToken)
  {
    var updateCustomerCommand = new UpdateCustomerCommand()
    {
      Id = request.Id,
      FirstName = request.FirstName,
      LastName = request.LastName,
      Country = request.Country,
    };

    UpdateCustomerResponse updateCustomerResponse;
    int statusCode;

    try
    {
      var isSuccess = await _mediator.Send(updateCustomerCommand, cancellationToken)
        .ConfigureAwait(false);

      updateCustomerResponse = new UpdateCustomerResponse()
      {
        Message = $"Customer updated with name '{request.FirstName} {request.LastName}' from '{request.Country}'"
      };

      statusCode = 200;
    }
    catch (ValidationException ex)
    {
      updateCustomerResponse = new UpdateCustomerResponse()
      {
        Message = ex.Message
      };

      statusCode = 400;
    }

    await SendAsync(updateCustomerResponse, statusCode, cancellationToken)
      .ConfigureAwait(false);
  }
}

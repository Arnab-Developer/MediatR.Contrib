using FastEndpoints;
using FluentValidation;
using MediatR;

namespace NimblePros.SampleWeb.Endpoints;

public class DeleteCustomerEndpoint(IMediator mediator)
  : Endpoint<DeleteCustomerRequest, DeleteCustomerResponse>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get("/delete-customer");
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteCustomerRequest request,
    CancellationToken cancellationToken)
  {
    var deleteCustomerCommand = new DeleteCustomerCommand()
    {
      Id = request.Id
    };

    DeleteCustomerResponse deleteCustomerResponse;
    int statusCode;

    try
    {
      var isSuccess = await _mediator.Send(deleteCustomerCommand, cancellationToken)
        .ConfigureAwait(false);

      deleteCustomerResponse = new DeleteCustomerResponse()
      {
        Message = $"Customer deleted"
      };

      statusCode = 200;
    }
    catch (ValidationException ex)
    {
      deleteCustomerResponse = new DeleteCustomerResponse()
      {
        Message = ex.Message
      };

      statusCode = 400;
    }

    await SendAsync(deleteCustomerResponse, statusCode, cancellationToken)
      .ConfigureAwait(false);
  }
}

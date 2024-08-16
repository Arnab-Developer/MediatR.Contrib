using NimblePros.MediatR.Contrib.Abstractions;
using NimblePros.SampleWeb.Models;

namespace NimblePros.SampleWeb.Queries;

public class GetCustomerQuery : IQuery<Customer>
{
  public int Id { get; set; }
}

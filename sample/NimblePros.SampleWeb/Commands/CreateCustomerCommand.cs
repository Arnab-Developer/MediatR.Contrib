namespace NimblePros.SampleWeb.Commands;

public class CreateCustomerCommand : ICommand<int>
{
  public string FirstName { get; set; } = string.Empty;

  public string LastName { get; set; } = string.Empty;

  public string Country { get; set; } = string.Empty;
}

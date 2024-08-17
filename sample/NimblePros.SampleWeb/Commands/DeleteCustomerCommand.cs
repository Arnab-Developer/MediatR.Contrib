namespace NimblePros.SampleWeb.Commands;

public class DeleteCustomerCommand : ICommand<bool>
{
  public int Id { get; set; }
}

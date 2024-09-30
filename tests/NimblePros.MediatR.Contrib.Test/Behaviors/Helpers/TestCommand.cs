namespace NimblePros.MediatR.Contrib.Test.Behaviors.Helpers;

public class TestCommand : ICommand<bool>
{
  public string Name { get; set; } = string.Empty;
}

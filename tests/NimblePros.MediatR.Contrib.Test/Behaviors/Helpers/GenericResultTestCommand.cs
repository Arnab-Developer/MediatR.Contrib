using Ardalis.Result;

namespace NimblePros.MediatR.Contrib.Test.Behaviors.Helpers;

internal class GenericResultTestCommand : ICommand<Result<Value>>
{
  public string Name { get; set; } = string.Empty;
}

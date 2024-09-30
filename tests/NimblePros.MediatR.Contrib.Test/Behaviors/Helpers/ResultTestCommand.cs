using Ardalis.Result;

namespace NimblePros.MediatR.Contrib.Test.Behaviors.Helpers;

internal class ResultTestCommand : ICommand<Result>
{
  public string Name { get; set; } = string.Empty;
}

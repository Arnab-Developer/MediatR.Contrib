using Ardalis.Result;

namespace NimblePros.MediatR.Contrib.Test.Behaviors.Helpers;

public interface INext
{
  public Task<bool> Next();

  public Task<Result> ResultNext();

  public Task<Result<Value>> GenericResultNext();
}

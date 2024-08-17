using Ardalis.Result;

namespace NimblePros.MediatR.Contrib.Test.Behaviors.Helpers;

internal static class Pipeline
{
  public static async Task<bool> Next() =>
    await Task.FromResult(true);

  public static async Task<Result> ResultNext() =>
    await Task.FromResult(Result.Success());

  public static async Task<Result<Value>> GenericResultNext() =>
    await Task.FromResult(Result.Success(new Value()));
}

using Dynamo.Applications.AdvanceSteel.Services;

namespace Dynamo.Applications.AdvanceSteel
{
  public class SteelAppResolver : AppResolver
  {
    public override T ResolveType<T>()
    {
      if (typeof(T) == typeof(IContextManager))
        return new OneTransactionPerAllContexts() as T;

      if (typeof(T) == typeof(ISingleContextManager))
        return new OneTransactionPerContext() as T;

      if (typeof(T) == typeof(IAppInteraction))
        return new SteelAppInteraction() as T;

      return null;
    }
  }
}

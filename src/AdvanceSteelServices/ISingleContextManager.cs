
namespace Dynamo.Applications.AdvanceSteel.Services
{
  public interface ISingleContextManager
  {
    void EnsureInContext();
    void LeaveContext();
  }
}

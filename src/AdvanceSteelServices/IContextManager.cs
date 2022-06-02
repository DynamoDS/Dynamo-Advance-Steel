
namespace Dynamo.Applications.AdvanceSteel.Services
{
  public interface IContextManager
  {
    void EnsureInContext(DocContext ctx);
    void LeaveContext(DocContext ctx);

    void ForceCloseTransaction();
  }
}

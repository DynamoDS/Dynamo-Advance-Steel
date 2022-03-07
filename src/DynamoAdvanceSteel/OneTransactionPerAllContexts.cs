using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.DocumentManagement;

namespace Dynamo.Applications.AdvanceSteel
{
  public class OneTransactionPerAllContexts : IContextManager
  {
    private static Autodesk.AdvanceSteel.CADAccess.Transaction SteelTransaction = null;
    private static bool DocumentLocked = false;
    private static bool SubscribedToRefreshCompleted = false;

    public void EnsureInContext(DocContext ctx)
    {
      StartTransaction();
    }

    public void LeaveContext(DocContext ctx)
    {

    }

    public void ForceCloseTransaction()
    {
      CloseTransaction();
    }

    private static void StartTransaction()
    {
      if (DocumentLocked == false)
      {
        DocumentLocked = DocumentManager.LockCurrentDocument();
      }

      if (SteelTransaction == null && DocumentLocked == true)
      {
        SteelTransaction = Autodesk.AdvanceSteel.CADAccess.TransactionManager.StartTransaction();
      }

      if (DocumentLocked == false || SteelTransaction == null)
      {
        throw new System.Exception("Failed to access Document");
      }
    }
    private static void CloseTransaction()
    {
      if (SteelTransaction != null)
      {
        SteelTransaction.Commit();
        SteelTransaction = null;
      }

      if (DocumentLocked == true)
      {
        DocumentLocked = DocumentManager.UnlockCurrentDocument();
        DocumentLocked = false;
      }
    }
  }
}

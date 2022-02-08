using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.DocumentManagement;

namespace Dynamo.Applications.AdvanceSteel
{
  public class OneTransactionPerContext : ISingleContextManager
  {
    private Autodesk.AdvanceSteel.CADAccess.Transaction SteelTransaction = null;
    private bool DocumentLocked = false;

    public void EnsureInContext()
    {
      if (SteelTransaction != null || DocumentLocked == true)
        throw new System.Exception("Nested context");

      DocumentLocked = DocumentManager.LockCurrentDocument();

      if (DocumentLocked == true)
      {
        SteelTransaction = Autodesk.AdvanceSteel.CADAccess.TransactionManager.StartTransaction();
      }

      if (DocumentLocked == false || SteelTransaction == null)
      {
        throw new System.Exception("Failed to access Document");
      }
    }

    public void LeaveContext()
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

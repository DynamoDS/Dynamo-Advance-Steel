using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.DocumentManagement;

namespace Dynamo.Applications.AdvanceSteel
{
  public class OneTransactionPerContext : IContextManager
  {
    private Autodesk.AdvanceSteel.CADAccess.Transaction steelTransaction = null;
    private bool bDocumentLocked = false;

    public void EnsureInContext(DocContext ctx)
    {
      if (steelTransaction != null || bDocumentLocked == true)
        throw new System.Exception("Nested context");

      bDocumentLocked = DocumentManager.LockCurrentDocument();

      if (bDocumentLocked == true)
      {
        steelTransaction = Autodesk.AdvanceSteel.CADAccess.TransactionManager.StartTransaction();
      }

      if (bDocumentLocked == false || steelTransaction == null)
      {
        throw new System.Exception("Failed to access Document");
      }
    }

    public void LeaveContext(DocContext ctx)
    {
      if (steelTransaction != null)
      {
        steelTransaction.Commit();
        steelTransaction = null;
      }

      if (bDocumentLocked == true)
      {
        bDocumentLocked = DocumentManager.UnlockCurrentDocument();
        bDocumentLocked = false;
      }
    }
  }
}

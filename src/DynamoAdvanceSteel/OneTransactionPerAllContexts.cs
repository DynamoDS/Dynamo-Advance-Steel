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
    private static Autodesk.AdvanceSteel.CADAccess.Transaction steelTransaction = null;
    private static bool bDocumentLocked = false;
    private static bool bSubscribedToRefreshCompleted = false;

    public void EnsureInContext(DocContext ctx)
    {
      StartTransaction();
      SubscribeToRefreshCompleted();
    }

    public void LeaveContext(DocContext ctx)
    {

    }
    private static void StartTransaction()
    {
      if (bDocumentLocked == false)
      {
        bDocumentLocked = DocumentManager.LockCurrentDocument();
      }

      if (steelTransaction == null && bDocumentLocked == true)
      {
        steelTransaction = Autodesk.AdvanceSteel.CADAccess.TransactionManager.StartTransaction();
      }

      if (bDocumentLocked == false || steelTransaction == null)
      {
        throw new System.Exception("Failed to access Document");
      }
    }
    private static void CloseTransaction()
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
    private static void RefreshCompleted(Graph.Workspaces.HomeWorkspaceModel obj)
    {
      CloseTransaction();
      UnsubscribeFromRefreshCompleted();
    }
    private static void SubscribeToRefreshCompleted()
    {
      if (bSubscribedToRefreshCompleted == false)
      {
        var dynModel = Dynamo.Applications.AdvanceSteel.Model.DynamoModel;
        dynModel.RefreshCompleted += RefreshCompleted;
        bSubscribedToRefreshCompleted = true;
      }
    }
    private static void UnsubscribeFromRefreshCompleted()
    {
      if (bSubscribedToRefreshCompleted == true)
      {
        var dynModel = Dynamo.Applications.AdvanceSteel.Model.DynamoModel;
        dynModel.RefreshCompleted -= RefreshCompleted;
        bSubscribedToRefreshCompleted = false;
      }
    }
  }
}

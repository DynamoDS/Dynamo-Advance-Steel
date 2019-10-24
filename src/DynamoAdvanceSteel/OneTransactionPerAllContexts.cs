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
      SubscribeToRefreshCompleted();
    }

    public void LeaveContext(DocContext ctx)
    {

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
    private static void RefreshCompleted(Graph.Workspaces.HomeWorkspaceModel obj)
    {
      CloseTransaction();
      UnsubscribeFromRefreshCompleted();
    }
    private static void SubscribeToRefreshCompleted()
    {
      if (SubscribedToRefreshCompleted == false)
      {
        var dynModel = Dynamo.Applications.AdvanceSteel.Model.DynamoModel;
        dynModel.RefreshCompleted += RefreshCompleted;
        SubscribedToRefreshCompleted = true;
      }
    }
    private static void UnsubscribeFromRefreshCompleted()
    {
      if (SubscribedToRefreshCompleted == true)
      {
        var dynModel = Dynamo.Applications.AdvanceSteel.Model.DynamoModel;
        dynModel.RefreshCompleted -= RefreshCompleted;
        SubscribedToRefreshCompleted = false;
      }
    }
  }
}

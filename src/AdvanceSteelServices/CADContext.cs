using Autodesk.AdvanceSteel.DocumentManagement;
using System;

namespace AdvanceSteel.Services.ObjectAccess
{
  /// <summary>
  /// Used to manage the documents locking and transactions while accessing Advance Steel objects
  /// </summary>
  public class CADContext : IDisposable
  {
		Autodesk.AdvanceSteel.CADAccess.Transaction tr = null;
		private bool bTransactionStarted = false;
    private bool bDocumentLocked = false;

		public CADContext()
		{
			bDocumentLocked = DocumentManager.LockCurrentDocument();

			if (bDocumentLocked == true) {
				tr = Autodesk.AdvanceSteel.CADAccess.TransactionManager.StartTransaction();
        if(tr != null)
				  bTransactionStarted = true;
			}

			if (bDocumentLocked == false || bTransactionStarted == false)
      {
        UnInitialize();
        throw new System.Exception("Failed to access AutoCAD Document");
      }
    }

    private void UnInitialize()
    {
      if (bDocumentLocked == true)
      {
        if (bTransactionStarted == true)
				{
					tr.Commit();
				}

        bDocumentLocked = DocumentManager.UnlockCurrentDocument();
      }

      bDocumentLocked = bTransactionStarted = false;
    }

    public void Dispose()
    {
      UnInitialize();
    }
  }
}
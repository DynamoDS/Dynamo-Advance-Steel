using Autodesk.AdvanceSteel.DocumentManagement;
using System;

namespace AdvanceSteel.Services.ObjectAccess
{
  /// <summary>
  /// Used to manage the documents locking and transactions while accessing Advance Steel objects
  /// </summary>
  public class CADContext : IDisposable
  {
    private bool bTransactionStarted = false;
    private bool bDocumentLocked = false;

    public CADContext()
    {
      bDocumentLocked = DocumentManager.lockCurrentDocument();

      if (bDocumentLocked == true)
        bTransactionStarted = Autodesk.AdvanceSteel.CADAccess.TransactionManager.startTransaction();

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
          bTransactionStarted = Autodesk.AdvanceSteel.CADAccess.TransactionManager.endTransaction();

        bDocumentLocked = DocumentManager.unlockCurrentDocument();
      }

      bDocumentLocked = bTransactionStarted = false;
    }

    public void Dispose()
    {
      UnInitialize();
    }
  }
}
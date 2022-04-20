using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using System;
using Dynamo.Applications.AdvanceSteel.Services;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.CADAccess;

namespace AdvanceSteel.Nodes
{
  /// <summary>
  /// This is the equivalent of an Advance Steel object in Dynamo
  /// </summary>
  [IsVisibleInDynamoLibrary(false)]
  public abstract class SteelDbObject : SteelDynObject, IGraphicItem
  {
    private string ObjectHandle;
    private static readonly object access_obj = new object();
    protected bool IsOwnedByDynamo = true;

    protected void SafeInit(Action init)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var elementManager = LifecycleManager.GetInstance();

          string handle = GetObjectASHandleFromTrace();
          bool elementExist = !string.IsNullOrEmpty(handle);

          int count = 0;
          if (elementExist)
          {
            count = elementManager.GetRegisteredCount(handle);
          }

          try
          {
            init();
          }
          catch (Exception e)
          {
            //If the element is newly created and bound but the creation is aborted because
            //of an exception, it need to be unregistered and deleted
            if (!elementExist && ObjectHandle != null)
            { 
              var filerObject = Utils.GetObject(ObjectHandle);

              if (filerObject != null)
                filerObject.DelFromDb();

              elementManager.UnRegisterAssociation(ObjectHandle, this);
              ObjectHandle = null;
              throw e;
            }
            else if (elementExist)
            {
              //If the internal element has already been bound, and if the registered count has increased,
              //it need to be unregistered.
              if (elementManager.GetRegisteredCount(handle) == (count + 1))
              {
                elementManager.UnRegisterAssociation(handle, this);
                ObjectHandle = null;
              }

              //It means that the updating operation failed, an attemption of making a new element is made.
              ElementBinder.SetRawDataForTrace(null);
              SafeInit(init);
            }
            else
            {
              throw e;
            }
          }//catch

        }// TransactionContext
      }

    }

    private string GetObjectASHandleFromTrace()
    {
      FilerObject filerObject = ElementBinder.GetObjectASFromTrace<FilerObject>();

      if (filerObject == null)
        return null;

      return filerObject.Handle;
    }

    protected void SetHandle(FilerObject pFilerObject)
    {
      this.Handle = pFilerObject.Handle;
    }

    /// <summary>
    /// Property that holds the handle of the object
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public string Handle
    {
      get
      {
        return ObjectHandle;
      }
      private set
      {
        ObjectHandle = value;

        var elementManager = LifecycleManager.GetInstance();
        elementManager.RegisterAsssociation(ObjectHandle, this);
      }
    }

    [IsVisibleInDynamoLibrary(false)]
    public override void Dispose()
    {
      lock (access_obj)
      {
        // Do not cleanup elements if we are shutting down Dynamo.
        if (DisposeLogic.IsShuttingDown || DisposeLogic.IsClosingHomeworkspace)
          return;

        var elementManager = LifecycleManager.GetInstance();

        int remainingBindings = elementManager.UnRegisterAssociation(Handle, this);

        // Do not delete owned elements
        if (remainingBindings == 0 && IsOwnedByDynamo == true)
        {
          if (Handle != null)
          {
            //lock the document and start a transaction
            using (var ctx = new DocContext())
            {
              var filerObject = Utils.GetObject(Handle);

              if (filerObject != null)
                filerObject.DelFromDb();

              ObjectHandle = string.Empty;
            }
          }
        }
        else
        {
          //This element has gone
          ObjectHandle = string.Empty;
        }
      }
    }

    [IsVisibleInDynamoLibrary(false)]
    public void Tessellate(IRenderPackage package, TessellationParameters parameters)
    {
    }
  }
}
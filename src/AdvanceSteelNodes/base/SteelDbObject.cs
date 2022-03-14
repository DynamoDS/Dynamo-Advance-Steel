using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using System;
using Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes
{
  /// <summary>
  /// This is the equivalent of an Advance Steel object in Dynamo
  /// </summary>
  [IsVisibleInDynamoLibrary(false)]
  public abstract class SteelDbObject : SteelDynObject, IGraphicItem
  {
    protected string ObjectHandle;
    protected static readonly object access_obj = new object();
    internal bool IsOwnedByDynamo = true;

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
      set
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
        if (DisposeLogic.IsShuttingDown)
          return;

        var elementManager = LifecycleManager.GetInstance();
        bool entityBinding = elementManager.IsEntityBinding(Handle);
        int remainingBindings = elementManager.UnRegisterAssociation(Handle, this);

        // Do not delete owned elements neither not linked. If Homeworkspace is closing it is not needed to delete element
        if (!DisposeLogic.IsClosingHomeworkspace && remainingBindings == 0 && IsOwnedByDynamo && entityBinding)
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
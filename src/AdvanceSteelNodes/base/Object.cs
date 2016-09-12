using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using System;

namespace AdvanceSteel.Nodes
{
  /// <summary>
  /// This is the equivalent of an Advance Steel object in Dynamo
  /// </summary>
  [IsVisibleInDynamoLibrary(false)]
  public abstract class Object : IDisposable, IGraphicItem, IFormattable
  {
    private string ObjectHandle;
    internal readonly object myLock = new object();

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

        var elementManager = AdvanceSteel.Services.LifecycleManager<string>.GetInstance();
        elementManager.RegisterAsssociation(ObjectHandle, this);
      }
    }

    [IsVisibleInDynamoLibrary(false)]
    public virtual void Dispose()
    {
      //use lock just to be safe
      //AutoCAD does not support multithreaded access
      lock (myLock)
      {
        // Do not cleanup elements if we are shutting down Dynamo.
        if (AdvanceSteel.Services.DisposeLogic.IsShuttingDown || AdvanceSteel.Services.DisposeLogic.IsClosingHomeworkspace)
          return;

        //this function is not implemented for the moment
        bool didAdvanceSteelDelete = AdvanceSteel.Services.LifecycleManager<string>.GetInstance().IsAdvanceSteelDeleted(Handle);

        var elementManager = AdvanceSteel.Services.LifecycleManager<string>.GetInstance();
        int remainingBindings = elementManager.UnRegisterAssociation(Handle, this);

        // Do not delete owned elements
        if (remainingBindings == 0 && !didAdvanceSteelDelete)
        {
          //lock the document and start a transaction
          using (var _CADAccess = new AdvanceSteel.Services.ObjectAccess.CADContext())
          {
            if (Handle != null)
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

    [IsVisibleInDynamoLibrary(false)]
    public override string ToString()
    {
      return GetType().Name;
    }

    public virtual string ToString(string format, IFormatProvider formatProvider)
    {
      return ToString();
    }
  }
}
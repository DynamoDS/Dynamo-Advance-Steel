using Autodesk.AdvanceSteel.DocumentManagement;
using System;
using System.Reflection;

namespace Dynamo.Applications.AdvanceSteel.Services
{
  /// <summary>
  /// Used to manage the transactions while accessing Advance Steel objects - Single Context
  /// </summary>
  public class DocSingleContext : IDisposable
  {
    private bool readOnly = false;

    public DocSingleContext(bool readOnly = false)
    {
      this.readOnly = readOnly;
      Manager.EnsureInContext();
    }

    public void Dispose()
    {
      Manager.LeaveContext();
    }
    public static ISingleContextManager Manager
    {
      get
      {
        if (manager == null)
        {
          manager = AppResolver.Resolve<ISingleContextManager>();
        }

        return manager;
      }
    }

    public bool ReadOnly
    {
      get
      {
        return readOnly;
      }
      set
      {
        readOnly = value;
      }
    }

    private static ISingleContextManager manager;
  }
}
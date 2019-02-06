using Autodesk.AdvanceSteel.DocumentManagement;
using System;
using System.Reflection;

namespace Dynamo.Applications.AdvanceSteel.Services
{
  /// <summary>
  /// Used to manage the transactions while accessing Advance Steel objects
  /// </summary>
  public class DocContext : IDisposable
  {
    private bool readOnly = false;

    public DocContext(bool readOnly = false)
    {
      this.readOnly = readOnly;
      Manager.EnsureInContext(this);
    }

    public void Dispose()
    {
      Manager.LeaveContext(this);
    }
    public static IContextManager Manager
    {
      get
      {
        if (manager == null)
        {
          manager = AppResolver.Instance.Resolve<IContextManager>();
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

    private static IContextManager manager;
  }
}
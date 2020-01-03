using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamo.Applications.AdvanceSteel.Services
{
  public abstract class AppResolver
  {
    public abstract T ResolveType<T>() where T : class;
    public static T Resolve<T>() where T : class
    {
      return Instance.ResolveType<T>();
    }

    internal static AppResolver Instance
    {
      get
      {
        if (inst == null)
        {
          string appName = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName.ToLower();
          switch (appName)
          {
            case "revit.exe":
              {
                inst = Activator.CreateInstance("DynamoSteelRevit", "Dynamo.Applications.AdvanceSteel.RevitAppResolver").Unwrap() as AppResolver;
                break;
              }
            case "acad.exe":
              {
                inst = Activator.CreateInstance("DynamoAdvanceSteel", "Dynamo.Applications.AdvanceSteel.SteelAppResolver").Unwrap() as AppResolver;
                break;
              }
            case "vstest.executionengine.exe":
            case "testhost.exe":
              {
                inst = Activator.CreateInstance("DynamoSteelTests", "DynamoSteelTests.TestsAppResolver").Unwrap() as AppResolver;
                break;
              }
            default:
              {
                break;
              }
          }
        }

        return inst;
      }
      set
      {
        inst = value;
      }
    }

    private static AppResolver inst;
  }
}

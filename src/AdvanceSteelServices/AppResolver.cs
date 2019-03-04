using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamo.Applications.AdvanceSteel.Services
{
  public abstract class AppResolver
  {
    public abstract T Resolve<T>() where T : class;

    public static AppResolver Instance
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
                inst = (AppResolver)Activator.CreateInstance("DynamoSteelRevit", "Dynamo.Applications.AdvanceSteel.RevitAppResolver").Unwrap();
                break;
              }
            case "acad.exe":
              {
                inst = (AppResolver)Activator.CreateInstance("DynamoAdvanceSteel", "Dynamo.Applications.AdvanceSteel.SteelAppResolver").Unwrap();
                break;
              }
            case "vstest.executionengine.exe":
            case "testhost.exe":
              {
                inst = (AppResolver)Activator.CreateInstance("DynamoSteelTests", "DynamoSteelTests.TestsAppResolver").Unwrap();
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
      internal set
      {
        inst = value;
      }
    }

    private static AppResolver inst;
  }
}

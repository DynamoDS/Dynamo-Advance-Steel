using Autodesk.AutoCAD.ApplicationServices.Core;
using Dynamo.Scheduler;
using System;

namespace Dynamo.Applications.AdvanceSteel
{
  /// <summary>
  /// class used to execute dynamo tasks in AutoCAD application context
  /// </summary>
  public class SchedulerThread : ISchedulerThread
  {
    private IScheduler scheduler;

    public void Initialize(IScheduler owningScheduler)
    {
      scheduler = owningScheduler;
      Application.Idle += Application_Idle;
    }

    private void Application_Idle(object sender, EventArgs e)
    {
      Application.DocumentManager.ExecuteInApplicationContext(ExecuteInApplicationContextCallback, null);
    }

    public void ExecuteInApplicationContextCallback(object userData)
    {
      while (scheduler.ProcessNextTask(false))
      {
      }
    }

    public void Shutdown()
    {
      Application.Idle -= Application_Idle;
    }
  }
}
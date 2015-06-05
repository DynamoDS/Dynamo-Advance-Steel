using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Core.Threading;
using Dynamo.Interfaces;
using Autodesk.AutoCAD.ApplicationServices.Core;

namespace Dynamo.Applications
{
    /// <summary>
    /// class used to execute dynamo tasks in AutoCAD application context
    /// </summary>
    public class SchedulerThread:ISchedulerThread
    {
        private IScheduler scheduler;
        public void Initialize(IScheduler owningScheduler)
        {
            scheduler = owningScheduler;
            Application.Idle += Application_Idle;
        }

        void Application_Idle(object sender, EventArgs e)
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


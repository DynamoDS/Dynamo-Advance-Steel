using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.CADLink.Database;
using Autodesk.AdvanceSteel.ConstructionHelper;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.DocumentManagement;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.AdvanceSteel.Profiles;
using Autodesk.AdvanceSteel.Runtime;


using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Threading;

using Dynamo.Controls;
using Dynamo.Core;
using Dynamo.Core.Threading;
using Dynamo.Models;
using Dynamo.Services;
using Dynamo.Utilities;
using Dynamo.ViewModels;

using DynamoUtilities;

using MessageBox = System.Windows.Forms.MessageBox;
using Dynamo.Applications.Models;
using AdvanceSteel.Nodes;

[assembly: CommandClassAttribute(typeof(Dynamo.Applications.CommandClass))]
namespace Dynamo.Applications
{
    /// <summary>
    /// class that contains the definition for the command that is exposed in AutoCAD
    /// </summary>
    public class CommandClass
    {
        private static DynamoViewModel dynamoViewModel;
        private static AdvanceSteelModel advanceSteelModel;

        [CommandMethodAttribute("TEST_GROUP", "Create", "RunDynamo", CommandFlags.Modal | CommandFlags.UsePickSet | CommandFlags.Redraw)]
        public void Create()
        {
            //disable document switch while dynamo is open
            Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.DocumentActivationEnabled = false;

            InitializeCore();

            advanceSteelModel = InitializeCoreModel();            
            dynamoViewModel = InitializeCoreViewModel(advanceSteelModel);

            //show dynamo window
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessWindow(InitializeCoreView());
        }
        private static AdvanceSteelModel InitializeCoreModel()
        {
            var prefs = PreferenceSettings.Load();
            var corePath =
                Path.GetFullPath(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\");

            return AdvanceSteelModel.Start(
                new AdvanceSteelModel.StartConfiguration()
                {
                    Preferences = prefs,
                    DynamoCorePath = corePath,
                    SchedulerThread = new SchedulerThread()
                });
        }
        private static DynamoViewModel InitializeCoreViewModel(AdvanceSteelModel advanceSteelModel)
        {

            var viewModel = DynamoViewModel.Start(
                new DynamoViewModel.StartConfiguration()
                {
                    DynamoModel = advanceSteelModel
                });
            return viewModel;
        }
        private static DynamoView InitializeCoreView()
        {
            IntPtr mwHandle = Autodesk.AdvanceSteel.CADAccess.CADUtilities.GetCADWindowHandle();
            var dynamoView = new DynamoView(dynamoViewModel);
            
            //autocad will manage the interop helper
            //new WindowInteropHelper(dynamoView).Owner = mwHandle;

            return dynamoView;
        }
        private static bool initializedCore;
        private static void InitializeCore()
        {
            if (initializedCore) return;

            string interactivityPath = Path.Combine(
               DynamoPathManager.Instance.MainExecPath,
               "System.Windows.Interactivity.dll");

            if (File.Exists(interactivityPath))
                Assembly.LoadFrom(interactivityPath);

            initializedCore = true;
        }
    }
}
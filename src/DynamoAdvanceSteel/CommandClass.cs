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
using Dynamo.UpdateManager;
using Microsoft.Win32;

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

          try
          {
            string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
          }
          catch (Exception ex)
          {
            MessageBox.Show(ex.ToString());
          }

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
            string corePath = DynamoAdvanceSteelApplication.DynamoCorePath;
            return AdvanceSteelModel.Start(
                new Dynamo.Models.DynamoModel.DefaultStartConfiguration()
                {
                    GeometryFactoryPath = GetGeometryFactoryPath(corePath),
                    //Preferences = prefs,
                    DynamoCorePath = corePath,
                    SchedulerThread = new SchedulerThread(),
                    PathResolver = new AdvanceSteelPathResolver()
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

            string interactivityPath = "";// Path.Combine(
                //DynamoPathManager.Instance.MainExecPath,
                //"System.Windows.Interactivity.dll");

            if (File.Exists(interactivityPath))
                Assembly.LoadFrom(interactivityPath);

            initializedCore = true;
        }
        /// <summary>
        /// DynamoShapeManager.dll is a companion assembly of Dynamo core components,
        /// we do not want a static reference to it (since the Revit add-on can be 
        /// installed anywhere that's outside of Dynamo), we do not want a duplicated 
        /// reference to it. Here we use reflection to obtain GetGeometryFactoryPath
        /// method, and call it to get the geometry factory assembly path.
        /// </summary>
        /// <param name="corePath">The path where DynamoShapeManager.dll can be 
        /// located.</param>
        /// <returns>Returns the full path to geometry factory assembly.</returns>
        /// 
        public static string GetGeometryFactoryPath(string corePath)
        {
          var dynamoAsmPath = Path.Combine(corePath, "DynamoShapeManager.dll");
          var assembly = Assembly.LoadFrom(dynamoAsmPath);
          if (assembly == null)
            throw new FileNotFoundException("File not found", dynamoAsmPath);

          var utilities = assembly.GetType("DynamoShapeManager.Utilities");
          var getGeometryFactoryPath = utilities.GetMethod("GetGeometryFactoryPath");

          return (getGeometryFactoryPath.Invoke(null,
              new object[] { corePath, 221 }) as string);
        }
    }
}
using Autodesk.AdvanceSteel.Runtime;
using Dynamo.Applications.Models;
using Dynamo.Controls;
using Dynamo.ViewModels;
using Dynamo.Wpf.Interfaces;
using DynamoShapeManager;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MessageBox = System.Windows.Forms.MessageBox;

[assembly: CommandClassAttribute(typeof(Dynamo.Applications.CommandClass))]

namespace Dynamo.Applications
{
	/// <summary>
	/// Class that contains the definition for the command that is exposed in AutoCAD
	/// </summary>
	public class CommandClass
	{
		private static DynamoViewModel dynamoViewModel;
		private static AdvanceSteelModel advanceSteelModel;
		private static string GeometryFactoryPath = "";

		[CommandMethodAttribute("TEST_GROUP", "Create", "RunDynamo", CommandFlags.Modal | CommandFlags.UsePickSet | CommandFlags.Redraw)]
		public void Create()
		{
			try
			{
				//disable document switch while dynamo is open
				//Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.DocumentActivationEnabled = false;

				InitializeCore();

				advanceSteelModel = InitializeCoreModel();
				// get Dynamo Advance Steel Version
				var asDynamoVersion = Assembly.GetExecutingAssembly().GetName().Version;
				advanceSteelModel.HostName = "Dynamo AS";
				advanceSteelModel.HostVersion = asDynamoVersion.ToString();

				dynamoViewModel = InitializeCoreViewModel(advanceSteelModel);

				//show dynamo window
				Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessWindow(InitializeCoreView());

				//disable Ribbon button
				DynamoASUtils.modifyRibbon(DynamoASUtils.tabUIDDynamoAS, DynamoASUtils.panelUIDDynamoAS, DynamoASUtils.buttonUIDDynamoAS, false);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private static AdvanceSteelModel InitializeCoreModel()
		{
			string corePath = DynamoAdvanceSteelApplication.DynamoCorePath;
			var userDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dynamo", "Dynamo Advance Steel");
			var commonDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Dynamo", "Dynamo Advance Steel");

			return AdvanceSteelModel.Start(
					new Dynamo.Models.DynamoModel.DefaultStartConfiguration()
					{
						GeometryFactoryPath = GeometryFactoryPath,
						DynamoCorePath = corePath,
						SchedulerThread = new SchedulerThread(),
						PathResolver = new AdvanceSteelPathResolver(userDataFolder, commonDataFolder)
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
			DynamoView dynamoView = new DynamoView(dynamoViewModel);
            dynamoView.Loaded += (o, e) => UpdateLibraryLayoutSpec();


            return dynamoView;
        }
        /// <summary>
        /// Updates the Libarary Layout spec to include layout for Steel nodes. 
        /// The Steel layout spec is embeded as resource "LayoutSpecs.json".
        /// </summary>
        private static void UpdateLibraryLayoutSpec()
        {
            //Get the library view customization service to update spec
            var customization = advanceSteelModel.ExtensionManager.Service<ILibraryViewCustomization>();
            if (customization == null) return;

            //Register the icon resource
            /*customization.RegisterResourceStream("/icons/Category.AdvanceSteel.svg", 
                GetResourceStream("Dynamo.Applications.Resources.Category.AdvanceSteel.svg"));*/

            //Read the steelSpec from the resource stream
            LayoutSpecification steelSpecs;
            using (Stream stream = GetResourceStream("Dynamo.Applications.Resources.LayoutSpecs.json"))
            {
                steelSpecs = LayoutSpecification.FromJSONStream(stream);
            }

            //The steelSpec should have only one section, add all its child elements to the customization
            var elements = steelSpecs.sections.First().childElements;
            customization.AddElements(elements); //add all the elements to default section
        }

        /// <summary>
        /// Reads the embeded resource stream by given name
        /// </summary>
        /// <param name="resource">Fully qualified name of the embeded resource.</param>
        /// <returns>The resource Stream if successful else null</returns>
        private static Stream GetResourceStream(string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(resource);
            return stream;
        }

		private static bool initializedCore;

		private static void InitializeCore()
		{
			if (initializedCore) return;

			string path = Environment.GetEnvironmentVariable("PATH");
			Environment.SetEnvironmentVariable("PATH", path + ";" + DynamoAdvanceSteelApplication.DynamoCorePath);

			var preloader = new Preloader(DynamoAdvanceSteelApplication.DynamoCorePath, DynamoAdvanceSteelApplication.ACADCorePath, LibraryVersion.Version224);
			preloader.Preload();
			GeometryFactoryPath = preloader.GeometryFactoryPath;

			initializedCore = true;
		}
	}
}
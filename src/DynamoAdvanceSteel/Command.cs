using Autodesk.AdvanceSteel.Runtime;
using Dynamo.Controls;
using Dynamo.ViewModels;
using Dynamo.Wpf.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;

[assembly: CommandClassAttribute(typeof(Dynamo.Applications.AdvanceSteel.Command))]

namespace Dynamo.Applications.AdvanceSteel
{
  public class Command
  {
    private static string GeometryFactoryPath = "";

    private const string GroupDynamo = "DYNAMO";
    private const string RunDynamoCommand = "RunDynamo";

    [CommandMethodAttribute(GroupDynamo, RunDynamoCommand, RunDynamoCommand, CommandFlags.Modal | CommandFlags.UsePickSet | CommandFlags.Redraw)]
    public void RunDynamo()
    {
      if (ModelController.DynamoView != null)
      {
        RibbonUtils.SetEnabledDynamoButton(false);

        ModelController.DynamoView.Focus();
        CenterWindowOnScreen(ModelController.DynamoView);
        return;
      }

      try
      {
        RibbonUtils.SetEnabledDynamoButton(false);

        InitializeCore();

        ModelController.DynamoModel = InitializeCoreModel();

        ModelController.DynamoModel.Logger.Log("SYSTEM", string.Format("Environment Path:{0}", Environment.GetEnvironmentVariable("PATH")));

        ModelController.ViewModel = InitializeCoreViewModel(ModelController.DynamoModel);

        ModelController.DynamoView = InitializeCoreView(ModelController.ViewModel);

        Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessWindow(ModelController.DynamoView);
      }
      catch (Exception ex)
      {
        ModelController.CleanControls();
        RibbonUtils.SetEnabledDynamoButton(true);
        MessageBox.Show(ex.ToString());
      }
    }

    /// <summary>
    /// To center dynamo window at the second time launch
    /// </summary>
    /// <param name="window"></param>
    private void CenterWindowOnScreen(Window window)
    {
      double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
      double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
      double windowWidth = window.Width;
      double windowHeight = window.Height;
      window.Left = (screenWidth / 2) - (windowWidth / 2);
      window.Top = (screenHeight / 2) - (windowHeight / 2);
    }

    private static DynamoSteelModel InitializeCoreModel()
    {
      string folder1 = "Dynamo";
      string folder2 = "Dynamo Advance Steel";
      string folder3 = "2023";

      var userDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), folder1, folder2, folder3);
      var commonDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), folder1, folder2, folder3);

      var startConfiguration = new Dynamo.Models.DynamoModel.DefaultStartConfiguration()
      {
        GeometryFactoryPath = GeometryFactoryPath,
        DynamoCorePath = DynamoSteelApp.DynamoCorePath,
        SchedulerThread = new SchedulerThread(),
        PathResolver = new PathResolver(userDataFolder, commonDataFolder),
        AuthProvider = new SteelAuthProvider(),
        ProcessMode = Scheduler.TaskProcessMode.Asynchronous
      };

      return DynamoSteelModel.Start(startConfiguration);
    }

    private static DynamoViewModel InitializeCoreViewModel(DynamoSteelModel advanceSteelModel)
    {
      var viewModel = DynamoSteelViewModel.Start(
        new DynamoViewModel.StartConfiguration()
      {
        DynamoModel = advanceSteelModel
      });

      return viewModel;
    }

    private static DynamoView InitializeCoreView(DynamoViewModel advanceSteelViewModel)
    {
      DynamoView dynamoView = new DynamoView(advanceSteelViewModel);
      dynamoView.Loaded += OnDynamoViewLoaded;
      dynamoView.Closed += OnDynamoViewClosed;

      return dynamoView;
    }
    private static void OnDynamoViewLoaded(object sender, EventArgs e)
    {
      UpdateLibraryLayoutSpec();
    }

    private static void OnDynamoViewClosed(object sender, EventArgs e)
    {
      var view = (DynamoView)sender;
      view.Loaded -= OnDynamoViewLoaded;
      view.Closed -= OnDynamoViewClosed;

      RibbonUtils.SetEnabledDynamoButton(true);
    }

    /// <summary>
    /// Updates the Libarary Layout spec to include layout for Steel nodes. 
    /// The Steel layout spec is embeded as resource "LayoutSpecs.json".
    /// </summary>
    private static void UpdateLibraryLayoutSpec()
    {
      var customization = ModelController.DynamoModel.ExtensionManager.Service<ILibraryViewCustomization>();
      if (customization == null) return;

      if (DynamoSteelApp.ShutdownHandler == null)
      {
        //Make sure to notify customization for application closing, so that 
        //the CEF can be shutdown for clean Advance Steel exit
        DynamoSteelApp.ShutdownHandler = () => customization.OnAppShutdown();
      }

      //Register the icon resource
      /*customization.RegisterResourceStream("/icons/Category.AdvanceSteel.svg", 
          GetResourceStream("Dynamo.Applications.Resources.Category.AdvanceSteel.svg"));*/

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

    /// <summary>
    /// Returns the version of ASM which is installed with AutoCAD at the requested path.
    /// This version number can be used to load the appropriate libG version.
    /// </summary>
    /// <param name="asmLocation">path where asm dlls are located, this is usually the product(AutoCAD) install path</param>
    /// <returns></returns>
    internal static Version findCurrentASMVersion(string asmLocation)
    {
      var lookup = new DynamoInstallDetective.InstalledProductLookUp("AutoCAD", "ASMAHL*.dll");
      var product = lookup.GetProductFromInstallPath(asmLocation);
      var libGversion = new Version(product.VersionInfo.Item1, product.VersionInfo.Item2, product.VersionInfo.Item3);
      return libGversion;
    }
    internal static Version PreloadAsm()
    {
      var acadPath = DynamoSteelApp.ACADCorePath;
      Version libGVersion = findCurrentASMVersion(acadPath);
      var preloaderLocation = DynamoShapeManager.Utilities.GetLibGPreloaderLocation(libGVersion, DynamoSteelApp.DynamoCorePath);

      // The LibG version maybe different in Dynamo and AutoCAD, using the one which is in Dynamo.
      Version preLoadLibGVersion = PreloadLibGVersion(preloaderLocation);

      // We do not preload anymore, because Advance Steel seems to prompt AutoCAD to already load the ASM modules that are actually needed (all the geometry stuff).
      // Not preloading here fixes an issue where AutoCAD does not have a particular ASM library file that is not actually needed for geometry, yet because of the missing file preloading would fail
      //DynamoShapeManager.Utilities.PreloadAsmFromPath(preloaderLocation, acadPath);
      return preLoadLibGVersion;
    }
    internal static Version PreloadLibGVersion(string preloaderLocation)
    {
      preloaderLocation = new DirectoryInfo(preloaderLocation).Name;
      var regExp = new Regex(@"^libg_(\d\d\d)_(\d)_(\d)$", RegexOptions.IgnoreCase);

      var match = regExp.Match(preloaderLocation);
      if (match.Groups.Count == 4)
      {
        return new Version(
            Convert.ToInt32(match.Groups[1].Value),
            Convert.ToInt32(match.Groups[2].Value),
            Convert.ToInt32(match.Groups[3].Value));
      }

      return new Version();
    }

    private static bool initializedCore = false;
    private static void InitializeCore()
    {
      if (initializedCore)
        return;

      string path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
      Environment.SetEnvironmentVariable("PATH", path + ";" + DynamoSteelApp.DynamoCorePath, EnvironmentVariableTarget.Process);

      //var acadLocale = CultureInfo.CurrentUICulture.ToString();
      //Environment.SetEnvironmentVariable("LANGUAGE", acadLocale.Replace("-", "_"));

      var loadedLibGVersion = PreloadAsm();
      GeometryFactoryPath = DynamoShapeManager.Utilities.GetGeometryFactoryPath2(DynamoSteelApp.DynamoCorePath, loadedLibGVersion);

      initializedCore = true;
    }
  }
}
using Autodesk.AdvanceSteel.Runtime;
using Dynamo.Controls;
using Dynamo.ViewModels;
using Dynamo.Wpf.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using MessageBox = System.Windows.Forms.MessageBox;

[assembly: CommandClassAttribute(typeof(Dynamo.Applications.AdvanceSteel.Command))]

namespace Dynamo.Applications.AdvanceSteel
{
  public static class Model
  {
    public static DynamoViewModel ViewModel;
    public static DynamoSteelModel DynamoModel;
  }

  [SupportedOSPlatform("windows")]
  public class Command
  {
    private static string GeometryFactoryPath = "";

    [CommandMethodAttribute("DYNAMO", "RunDynamo", "RunDynamo", CommandFlags.Modal | CommandFlags.UsePickSet | CommandFlags.Redraw)]
    public void RunDynamo()
    {
      try
      {
        InitializeCore();

        Model.DynamoModel = InitializeCoreModel();

        Models.DynamoModel.HostAnalyticsInfo = new Models.HostAnalyticsInfo() { HostName = "Dynamo AS", };
        Model.DynamoModel.HostVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        Model.ViewModel = InitializeCoreViewModel(Model.DynamoModel);

        Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessWindow(InitializeCoreView());

        RibbonUtils.SetEnabled(RibbonUtils.DynamoASTabUID, RibbonUtils.DynamoASPanelUID, RibbonUtils.DynamoASButtonUID, false);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }

    private static DynamoSteelModel InitializeCoreModel()
    {
      var userDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dynamo", "Dynamo Advance Steel", "2026");
      var commonDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Dynamo", "Dynamo Advance Steel", "2026");

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
      var config = new DynamoViewModel.StartConfiguration()
      {
        DynamoModel = advanceSteelModel
      };

      return DynamoViewModel.Start(config);
    }

    private static DynamoView InitializeCoreView()
    {
      DynamoView dynamoView = new DynamoView(Model.ViewModel);
      dynamoView.Closed += OnDynamoViewClosed;
      dynamoView.Loaded += (o, e) => UpdateLibraryLayoutSpec();

      return dynamoView;
    }
    private static void OnDynamoViewClosed(object sender, EventArgs e)
    {
      var view = (DynamoView)sender;
      view.Closed -= OnDynamoViewClosed;
    }

    /// <summary>
    /// Updates the Libarary Layout spec to include layout for Steel nodes. 
    /// The Steel layout spec is embeded as resource "LayoutSpecs.json".
    /// </summary>
    private static void UpdateLibraryLayoutSpec()
    {
      var customization = Model.DynamoModel.ExtensionManager.Service<ILibraryViewCustomization>();
      if (customization == null) return;

      //Make sure to notify customization for application closing
      DynamoSteelApp.ShutdownHandler = () => customization.OnAppShutdown();

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
      var location = Path.GetDirectoryName(typeof(Command).Assembly.Location);
      string paths = $"{location};{DynamoSteelApp.DynamoCorePath}";
      Autodesk.AdvanceSteel.ASAssemblyLocator.AddSearchPaths(paths);

      var asmLocation = DynamoSteelApp.ASCInstallPath;
      Version libGVersion = findCurrentASMVersion(asmLocation);
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

      string path = Environment.GetEnvironmentVariable("PATH");
      Environment.SetEnvironmentVariable("PATH", path + ";" + DynamoSteelApp.DynamoCorePath);

      //var acadLocale = CultureInfo.CurrentUICulture.ToString();
      //Environment.SetEnvironmentVariable("LANGUAGE", acadLocale.Replace("-", "_"));

      var loadedLibGVersion = PreloadAsm();
      GeometryFactoryPath = DynamoShapeManager.Utilities.GetGeometryFactoryPath2(DynamoSteelApp.DynamoCorePath, loadedLibGVersion);

      initializedCore = true;
    }
  }
}
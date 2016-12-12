using Dynamo.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Dynamo.Applications
{
  internal class AdvanceSteelPathResolver : IPathResolver
  {
    private readonly List<string> preloadLibraryPaths;
    private readonly List<string> additionalNodeDirectories;
    private readonly List<string> additionalResolutionPaths;
    private readonly string userDataRootFolder;
    private readonly string commonDataRootFolder;

    internal AdvanceSteelPathResolver(string userDataFolder, string commonDataFolder)
    {
      string addinDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

      // so we have to walk up one level.
      var currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
      var currentAssemblyDir = Path.GetDirectoryName(currentAssemblyPath);

      var nodesDirectory = addinDir;
      var nodesDll = Path.Combine(nodesDirectory, "AdvanceSteelNodes.dll");

      // Just making sure we are looking at the right level of nesting.
      if (!Directory.Exists(nodesDirectory))
        throw new DirectoryNotFoundException(nodesDirectory);

      // Add Revit-specific library paths for preloading.
      preloadLibraryPaths = new List<string>
            {
                "VMDataBridge.dll",
                "ProtoGeometry.dll",
                "DSCoreNodes.dll",
                "DSOffice.dll",
                "DSIronPython.dll",
                "FunctionObject.ds",
                "Optimize.ds",
                "DynamoConversions.dll",
                "DynamoUnits.dll",
                "Tessellation.dll",
                "Analysis.dll",
                "Display.dll",

                nodesDll
            };

      // Add an additional node processing folder
      additionalNodeDirectories = new List<string> { nodesDirectory };

      // Add the Revit_20xx folder for assembly resolution
      additionalResolutionPaths = new List<string> { currentAssemblyDir };
      userDataRootFolder = userDataFolder;
      commonDataRootFolder = commonDataFolder;
    }

    public IEnumerable<string> AdditionalNodeDirectories
    {
      get { return additionalNodeDirectories; }
    }

    public IEnumerable<string> AdditionalResolutionPaths
    {
      get { return additionalResolutionPaths; }
    }

    public IEnumerable<string> PreloadedLibraryPaths
    {
      get { return preloadLibraryPaths; }
    }

    public string UserDataRootFolder
    {
      get { return userDataRootFolder; }
    }

    public string CommonDataRootFolder
    {
      get { return commonDataRootFolder; }
    }
  }
}
using Dynamo.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Dynamo.Applications.AdvanceSteel
{
  internal class PathResolver : IPathResolver
  {
    private readonly List<string> preloadLibraryPaths;
    private readonly List<string> additionalNodeDirectories;
    private readonly List<string> additionalResolutionPaths;
    private readonly string userDataRootFolder;
    private readonly string commonDataRootFolder;

    internal PathResolver(string userDataFolder, string commonDataFolder)
    {
      userDataRootFolder = userDataFolder;
      commonDataRootFolder = commonDataFolder;

      string steelNodesDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


      additionalNodeDirectories = new List<string> { steelNodesDirectory };
      additionalResolutionPaths = new List<string> { steelNodesDirectory };

      var steelNodesDll = Path.Combine(steelNodesDirectory, "AdvanceSteelNodes.dll");

      preloadLibraryPaths = new List<string>
            {
                "VMDataBridge.dll",
                "ProtoGeometry.dll",
                "DesignScriptBuiltin.dll",
                "DSCoreNodes.dll",
                "DSOffice.dll",
                "DSIronPython.dll",
                "FunctionObject.ds",
                "BuiltIn.ds",
                "DynamoConversions.dll",
                "DynamoUnits.dll",
                "Tessellation.dll",
                "Analysis.dll",
                "GeometryColor.dll",
                steelNodesDll
            };
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
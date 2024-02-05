using Dynamo.Configuration;
using Dynamo.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
              //"DSIronPython.dll",   > version 2.6 - it isn't needed to use this because DSIronPython is loaded by extension(without IronPython installed)
                "DSCPython.dll",   // > version 2.7
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

    public IEnumerable<string> GetDynamoUserDataLocations()
    {
      var appDatafolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

      var paths = new List<string>();
      //Pre 1.0 Dynamo Studio user data was stored at %appdata%\Dynamo\
      var dynamoFolder = Path.Combine(appDatafolder, "Dynamo");
      if (Directory.Exists(dynamoFolder))
      {
        paths.AddRange(Directory.EnumerateDirectories(dynamoFolder));
      }

      //From 1.0 onwards Dynamo Studio user data is stored at %appdata%\Dynamo\Dynamo Advance Steel
      var advanceSteelFolder = Path.Combine(dynamoFolder, "Dynamo Advance Steel");
      if (Directory.Exists(advanceSteelFolder))
      {
        paths.AddRange(Directory.EnumerateDirectories(advanceSteelFolder));
      }

      return paths;
    }
  }
}
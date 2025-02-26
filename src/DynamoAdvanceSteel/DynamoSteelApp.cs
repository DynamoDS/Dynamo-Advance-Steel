using Autodesk.AdvanceSteel.Runtime;
using Autodesk.AdvanceSteel;
using System;
using System.IO;
using System.Reflection;
using Autodesk.AdvanceSteel.ASSettings;
using System.Runtime.Versioning;

[assembly: ExtensionApplicationAttribute(typeof(Dynamo.Applications.AdvanceSteel.DynamoSteelApp))]

namespace Dynamo.Applications.AdvanceSteel
{
  public sealed class DynamoSteelApp : IExtensionApplication
  {
    public static string DynamoCorePath = ProductLocator.GetDynamoCorePath();
#pragma warning disable CA1416 // Validate platform compatibility
    public static string ASCInstallPath = ProductLocator.GetASCInstallPath();
#pragma warning restore CA1416 // Validate platform compatibility
    public static Action ShutdownHandler = null;

    void IExtensionApplication.Initialize()
    {
      SubscribeAssemblyResolvingEvent();
    }

    void IExtensionApplication.Terminate()
    {
      ShutdownHandler?.Invoke();
      UnsubscribeAssemblyResolvingEvent();
    }

    private void SubscribeAssemblyResolvingEvent()
    {
      AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
    }

    private void UnsubscribeAssemblyResolvingEvent()
    {
      AppDomain.CurrentDomain.AssemblyResolve -= ResolveAssembly;
    }

    public static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
    {
      var assemblyPath = string.Empty;
      var assemblyName = new AssemblyName(args.Name).Name + ".dll";

      try
      {
        var corePath = DynamoCorePath;

        assemblyPath = Path.Combine(corePath, assemblyName);

        return (File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null);
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("The location of the assembly, {0} could not be resolved for loading.", assemblyPath), ex);
      }
    }
  }

  internal class ProductLocator
  {
    public static string GetDynamoCorePath()
    {
      string currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      return Path.Combine(currentDir, "Core");
    }

    [SupportedOSPlatform("windows")]
    public static string GetASCInstallPath()
    {
      return ASSettingsUtils.GetASCInstallPath();
    }
  }
}
using Autodesk.AdvanceSteel.Runtime;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

[assembly: ExtensionApplicationAttribute(typeof(Dynamo.Applications.AdvanceSteel.DynamoSteelApp))]

namespace Dynamo.Applications.AdvanceSteel
{
  public sealed class DynamoSteelApp : IExtensionApplication
  {
    public static string DynamoCorePath = ProductLocator.GetDynamoCorePath();
    public static string ACADCorePath = ProductLocator.GetACADCorePath();
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
    private static readonly string DynamoProductName = "Dynamo Core 2.";

    public static string GetDynamoCorePath()
    {
      var corePaths = GetInstallsFor(DynamoProductName);
      if (corePaths.ToArray().Length > 0)
        return corePaths.ToArray()[0];

      return string.Empty;
    }

    public static string GetACADCorePath()
    {
      string acadExePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
      return System.IO.Path.GetDirectoryName(acadExePath);
    }

    private static IEnumerable<string> GetInstallsFor(string productName)
    {
      const string regKey64 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\";
      var regKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
      regKey = regKey.OpenSubKey(regKey64);

      //Get "InstallLocation" value as string for all the subkey that starts with "Dynamo Core 1."
      return regKey.GetSubKeyNames().Where(s => s.StartsWith(productName)).Select(
              (s) => regKey.OpenSubKey(s).GetValue("InstallLocation") as string);
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AdvanceSteel.Runtime;

//using Dynamo.Applications.Properties;
using Dynamo.Utilities;

using DynamoUtilities;


using MessageBox = System.Windows.Forms.MessageBox;
using System.IO;
using System.Reflection;
using Dynamo.Applications.Models;
using Microsoft.Win32;

[assembly: ExtensionApplicationAttribute(typeof(Dynamo.Applications.DynamoAdvanceSteelApplication))]
namespace Dynamo.Applications
{
	public sealed class DynamoAdvanceSteelApplication : IExtensionApplication
	{
    public static string DynamoCorePath = DynamoLocator.GetCorePath();

		void IExtensionApplication.Initialize()
		{
      SubscribeAssemblyResolvingEvent();
		}
		void IExtensionApplication.Terminate()
		{
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
        var corePath = DynamoAdvanceSteelApplication.DynamoCorePath;

        assemblyPath = Path.Combine(corePath, assemblyName);

        return (File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null);
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("The location of the assembly, {0} could not be resolved for loading.", assemblyPath), ex);
      }
    }
	}
  internal class DynamoLocator
  {
    public static string GetCorePath()
    {
      var corePaths = GetDynamoInstalls();
      if (corePaths.ToArray().Length > 0)
        return corePaths.ToArray()[0];

      return string.Empty;
    }
    static IEnumerable<string> GetDynamoInstalls()
    {
      const string regKey64 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\";
      //Open HKLM for 64bit registry
      var regKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
      //Open Windows/CurrentVersion/Uninstall registry key
      regKey = regKey.OpenSubKey(regKey64);

      //Get "InstallLocation" value as string for all the subkey that starts with "Dynamo"
      return regKey.GetSubKeyNames().Where(s => s.StartsWith("Autodesk Dynamo Studio 2016")).Select(
          (s) => regKey.OpenSubKey(s).GetValue("InstallLocation") as string);
    }
  }
}

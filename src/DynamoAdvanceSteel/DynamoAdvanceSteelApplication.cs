using Autodesk.AdvanceSteel.Runtime;

//using Dynamo.Applications.Properties;
using Dynamo.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

[assembly: ExtensionApplicationAttribute(typeof(Dynamo.Applications.DynamoAdvanceSteelApplication))]

namespace Dynamo.Applications
{
	public sealed class DynamoAdvanceSteelApplication : IExtensionApplication
	{
		public static string DynamoCorePath = ProductLocator.GetDynamoCorePath();
		public static string ACADCorePath = ProductLocator.GetACADCorePath();

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
			//var corePaths = GetInstallsFor(AcadProductName);
			//if (corePaths.ToArray().Length > 0)
			//  return corePaths.ToArray()[0];

			//return string.Empty;
		}

		private static IEnumerable<string> GetInstallsFor(string productName)
		{
			const string regKey64 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\";
			//Open HKLM for 64bit registry
			var regKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			//Open Windows/CurrentVersion/Uninstall registry key
			regKey = regKey.OpenSubKey(regKey64);

			//Get "InstallLocation" value as string for all the subkey that starts with "Dynamo Core 1."
			return regKey.GetSubKeyNames().Where(s => s.StartsWith(productName)).Select(
					(s) => regKey.OpenSubKey(s).GetValue("InstallLocation") as string);
		}
	}
}
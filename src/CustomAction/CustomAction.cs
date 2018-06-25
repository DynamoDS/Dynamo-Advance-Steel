using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using System.Linq;
using System.Text.RegularExpressions;


namespace CustomAction
{
	public class CustomActions
	{
		private static readonly string DynamoProductName = "Dynamo Core 2.";
		[CustomAction]
		public static ActionResult GetDynamoCorePath(Session session)
		{
			var corePaths = GetInstallsFor(DynamoProductName);
			if (corePaths.ToArray().Length > 0)
			{
				string path = (corePaths.ToArray()[0]).TrimEnd('\\');
				try
				{
					path = Directory.GetParent(Directory.GetParent(path).FullName).FullName;
				}
				catch (Exception exception)
				{
					session.Log("ERROR in accessing DYNAMOLOCATION path {0}", exception.ToString());
				}
				try
				{
					session.Log("Begin SetProperty DYNAMOLOCATION action");
					session["DYNAMOLOCATION"] = path;
				}
				catch (Exception exception)
				{
					session.Log("ERROR in custom action SetProperty DYNAMOLOCATION {0}", exception.ToString());
				}
			}
			return ActionResult.Success;
		}
		[CustomAction]
		public static ActionResult GetDynamoCoreVersion(Session session)
		{
			var coreVersion = GetVersionFor(DynamoProductName);
			if (coreVersion.ToArray().Length > 0)
			{
				string version = coreVersion.ToArray()[0];

				Match m = Regex.Match(version, @"^(\d+\.\d+)");
				if (m.Success)
				{
					try
					{
						session.Log("Begin SetProperty DYNAMO_VERSION action");
						session["DYNAMO_VERSION"] = m.Value;
					}
					catch (Exception exception)
					{
						session.Log("ERROR in custom action SetProperty DYNAMO_VERSION {0}", exception.ToString());
					}
				}
			}
			return ActionResult.Success;
		}

		private static IEnumerable<string> GetInstallsFor(string productName)
		{
			const string regKey64 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\";
			var regKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			regKey = regKey.OpenSubKey(regKey64);

			return regKey.GetSubKeyNames().Where(s => s.StartsWith(productName)).Select(
					(s) => regKey.OpenSubKey(s).GetValue("InstallLocation") as string);
		}
		private static IEnumerable<string> GetVersionFor(string productName)
		{
			const string regKey64 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\";
			var regKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			regKey = regKey.OpenSubKey(regKey64);

			return regKey.GetSubKeyNames().Where(s => s.StartsWith(productName)).Select(
					(s) => regKey.OpenSubKey(s).GetValue("Version") as string);
		}
	}
}

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

[assembly: ExtensionApplicationAttribute(typeof(Dynamo.Applications.DynamoAdvanceSteel))]
namespace Dynamo.Applications
{
	public sealed class DynamoAdvanceSteel : IExtensionApplication
	{
		void IExtensionApplication.Initialize()
		{
            try
            {
                //DynamoUtilities.dll is not in the same folder
                //we must load it first because it will manage the loading process of dynamo assemblies  
                
                string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Assembly.LoadFrom(currentPath + @"\..\DynamoUtilities.dll");

                SetupDynamoPaths();

                //register Dynamo assembly resover
                SubscribeAssemblyResolvingEvent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
		}

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            throw new NotImplementedException();
        }

		void IExtensionApplication.Terminate()
		{
            UnsubscribeAssemblyResolvingEvent();
		}
        private static void SetupDynamoPaths()
        {
            string assDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            DynamoPathManager.Instance.AddResolutionPath(assDir);
            DynamoPathManager.Instance.InitializeCore(Path.GetFullPath(assDir + @"\.."));

            DynamoPathManager.Instance.AddPreloadLibrary(Path.Combine(assDir, "AsNodes.dll"));

            //add an additional node processing folder
            //DynamoPathManager.Instance.Nodes.Add(Path.Combine(assDir, "nodes"));
            
            DynamoPathManager.Instance.SetLibGPath("219");

            //rigth now we force Dynamo to load Asm Libs from Revit
            //in the future the libs will be hosted in AutoCAD
            DynamoPathManager.PreloadAsmLibraries(DynamoPathManager.Instance);
        }

        private void SubscribeAssemblyResolvingEvent()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyHelper.ResolveAssembly;
        }

        private void UnsubscribeAssemblyResolvingEvent()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= AssemblyHelper.ResolveAssembly;
        }
	}
}

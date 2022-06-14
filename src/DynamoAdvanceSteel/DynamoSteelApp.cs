using AdvanceSteel.Nodes;
using Autodesk.AdvanceSteel.Runtime;
using Autodesk.AutoCAD.EditorInput;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using acadApp = Autodesk.AutoCAD.ApplicationServices.Application;

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

      //Load dictionary properties
      InicializeSteelObjectPropertySets();
    }

    private void InicializeSteelObjectPropertySets()
    {
      try
      {
        UtilsProperties.LoadASTypeDictionary();
      }
      catch (Exception ex)
      {
        throw new Exception("Dynamo Load Error - Type Dictionary not loaded");
      }
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

    public static string GetACADCorePath()
    {
      string acadExePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
      return System.IO.Path.GetDirectoryName(acadExePath);
    }
  }
}
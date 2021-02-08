using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace ContentBuilder
{
  internal class NodeParser
  {
    private static string DynCorePath;
    private static string SteelCorePath;

    public static IEnumerable<Member> GetNodesFromAssemblies(IEnumerable<string> assembliesPaths)
    {
      IEnumerable<Member> ret = new List<Member>();
      foreach (string path in assembliesPaths)
        ret = ret.Concat(GetNodesFromAssembly(path));

      return ret.OrderBy(member => member.name);
    }

    public static IEnumerable<Member> GetNodesFromAssembly(string assemblyPath)
    {
      AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

      try
      {
        List<Member> ret = new List<Member>();

        Type[] types = Assembly.LoadFrom(assemblyPath).GetTypes();
        foreach (Type type in types)
        {
          if (!type.IsPublic || !IsVisibleInDynamo(type))
          {
            continue;
          }

          if (IsUINode(type))
          {
            Member uiNodeInfo = new Member();
            SetDataFromAttributes(uiNodeInfo, type);

            ret.Add(uiNodeInfo);
          }
          else // look for properties and static methods
          {
            // get all instance properties
            IEnumerable<MemberInfo> propertiesInfo = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // get all public static methods
            IEnumerable<MemberInfo> membersInfo = type.GetMembers(BindingFlags.Public | BindingFlags.Static);

            // merge props with methods
            IEnumerable<MemberInfo> allMembers = membersInfo.Concat(propertiesInfo);

            // get only the ones that are visible in dynamo
            allMembers = allMembers.Where(memberInfo => IsVisibleInDynamo(memberInfo));

            foreach (MemberInfo memberInfo in allMembers)
            {
              Member member = new Member() { name = type.Name + "." + memberInfo.Name };
              SetDataFromAttributes(member, memberInfo);

              ret.Add(member);
            }
          }
        }

        return ret.OrderBy(member => member.name);
      }
      finally
      {
        AppDomain.CurrentDomain.AssemblyResolve -= ResolveAssembly;
      }
    }
    private static void SetDataFromAttributes(Member toOverride, MemberInfo from)
    {
      foreach (Attribute attribute in from.GetCustomAttributes(false))
      {
        if (attribute is NodeNameAttribute)
        {
          toOverride.name = (attribute as NodeNameAttribute).Name;
        }
        else if (attribute is NodeDescriptionAttribute)
        {
          toOverride.summary = toOverride.summary ?? new Summary();
          toOverride.summary.description = (attribute as NodeDescriptionAttribute).ElementDescription;
        }
        else if (attribute is OutPortNamesAttribute)
        {
          string[] descs = (attribute as OutPortNamesAttribute).PortNames.ToArray();
          toOverride.returns = toOverride.returns ?? descs.Select(x => new Return()).ToList();

          // override the names
          for (int i = 0; i < descs.Length; i++)
          {
            toOverride.returns[i].name = descs[i];
          }
        }
        else if (attribute is OutPortDescriptionsAttribute)
        {
          string[] descs = (attribute as OutPortDescriptionsAttribute).PortDescriptions.ToArray();
          toOverride.returns = toOverride.returns ?? descs.Select(x => new Return()).ToList();

          // override descriptions
          for (int i = 0; i < descs.Length; i++)
          {
            toOverride.returns[i].description = descs[i];
          }
        }
      }

      // do not allow null values for the next properties
      toOverride.summary = toOverride.summary ?? new Summary();
      toOverride.parameters = toOverride.parameters ?? new List<Param>();
      toOverride.returns = toOverride.returns ?? new List<Return>();
    }

    private static bool IsVisibleInDynamo(MemberInfo memberInfo)
    {
      if (memberInfo is Type)
      {
        Type type = memberInfo as Type;
        if (
          IsSubclassOf(type, typeof(CoreNodeModelsWpf.Nodes.SelectionBaseNodeViewCustomization<,>))
          || type.Name.Contains("AstDropDownBase")
          || type.Name.Contains("SteelSelection")
          )
          return false;
      }

      foreach (Attribute attribute in memberInfo.GetCustomAttributes(false))
      {
        if (attribute is ObsoleteAttribute || attribute is SupressImportIntoVMAttribute)
          return false;

        if (attribute is IsVisibleInDynamoLibraryAttribute)
          return (attribute as IsVisibleInDynamoLibraryAttribute).Visible;
      }

      return true;
    }

    private static bool IsUINode(Type type)
    {
      return IsSubclassOf(type, typeof(CoreNodeModels.SelectionBase<,>)) || type.IsSubclassOf(typeof(CoreNodeModels.DSDropDownBase));
    }

    private static void ReadPathsFromPropsFile()
    {
      if (!string.IsNullOrEmpty(SteelCorePath) && !string.IsNullOrEmpty(DynCorePath))
        return;

      System.Reflection.Assembly assem = (new NodeParser()).GetType().Assembly;
      string configPath = Path.Combine(Path.GetDirectoryName(assem.Location), @"..\..\..\..\..\src\config\user_local.props");

      XmlDocument doc = new XmlDocument();
      doc.Load(configPath);

      foreach (XmlNode node in doc.DocumentElement.ChildNodes)
      {
        foreach (XmlNode locNode in node)
        {
          if (locNode.Name == "ADVANCESTEELAPI")
          {
            SteelCorePath = locNode.InnerText;

          }
          else if (locNode.Name == "DYNAMOAPI")
          {
            DynCorePath = locNode.InnerText;
          }
        }
      }

      string path = Environment.GetEnvironmentVariable("PATH");
      Environment.SetEnvironmentVariable("PATH", path + ";" + SteelCorePath + ";" + DynCorePath);

    }
    private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
    {
      ReadPathsFromPropsFile();

      string assemblyName = new AssemblyName(args.Name).Name + ".dll";
      string assemblyPath = Path.Combine(DynCorePath, assemblyName);
      Assembly ret = (File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null);

      if (null == ret)
      {
        assemblyPath = Path.Combine(DynCorePath, "nodes", assemblyName);
        ret = (File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null);
      }

      if (null == ret)
      {
        assemblyPath = Path.Combine(SteelCorePath, assemblyName);
        ret = (File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null);
      }

      if (null == ret)
      {
        ret = Assembly.Load(new AssemblyName(args.Name).FullName);
      }
      return ret;
    }

    private static bool IsSubclassOf(Type toCheck, Type generic)
    {
      while (toCheck != null && toCheck != typeof(object))
      {
        var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
        if (generic == cur)
        {
          return true;
        }
        toCheck = toCheck.BaseType;
      }
      return false;
    }

  }
}

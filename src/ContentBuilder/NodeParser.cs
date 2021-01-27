using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ContentBuilder
{
  internal class NodeParser
  {
    private static string DynCorePath;
    private static string SteelCorePath;

    public static IEnumerable<string> GetNodesFromAssembly(string assemblyPath)
    {
      AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ResolveReflectionOnlyAssembly;

      try
      {
        List<string> ret = new List<string>();

        Type[] types = Assembly.ReflectionOnlyLoadFrom(assemblyPath).GetTypes();
        foreach (Type type in types)
        {
          if (!type.IsPublic || !IsVisibleInDynamo(type))
          {
            continue;
          }

          // get all instance properties
          IEnumerable<MemberInfo> propertiesInfo = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

          // get all public static methods
          IEnumerable<MemberInfo> membersInfo = type.GetMembers(BindingFlags.Public | BindingFlags.Static);

          // merge props with methods
          IEnumerable<MemberInfo> allMembers = membersInfo.Concat(propertiesInfo);

          // get only the ones that are visible in dynamo
          allMembers = allMembers.Where(memberInfo => IsVisibleInDynamo(memberInfo));

          IEnumerable<string> nodes = allMembers.Select(memberInfo => type.Name + "." + memberInfo.Name);
          ret.AddRange(nodes);
        }

        ret.Sort();

        return ret;
      }
      finally
      {
        AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= ResolveReflectionOnlyAssembly;
      }
    }
    private static bool IsVisibleInDynamo(MemberInfo memberInfo)
    {
      foreach (CustomAttributeData customAttribute in memberInfo.CustomAttributes)
      {
        Type attributeType = customAttribute.AttributeType;

        if (attributeType.FullName == typeof(ObsoleteAttribute).FullName || attributeType.FullName == typeof(SupressImportIntoVMAttribute).FullName)
          return false;

        if (attributeType.FullName == typeof(IsVisibleInDynamoLibraryAttribute).FullName && customAttribute.ConstructorArguments[0].Value.Equals(false))
          return false;
      }

      return true;
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
    }
    private static Assembly ResolveReflectionOnlyAssembly(object sender, ResolveEventArgs args)
    {
      ReadPathsFromPropsFile();

      string assemblyName = new AssemblyName(args.Name).Name + ".dll";
      string assemblyPath = Path.Combine(DynCorePath, assemblyName);

      Assembly ret = (File.Exists(assemblyPath) ? Assembly.ReflectionOnlyLoadFrom(assemblyPath) : null);
      if (null == ret)
      {
        assemblyPath = Path.Combine(SteelCorePath, assemblyName);
        ret = (File.Exists(assemblyPath) ? Assembly.ReflectionOnlyLoadFrom(assemblyPath) : null);
      }

      return ret;
    }

  }
}

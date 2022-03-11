using Autodesk.AdvanceSteel.DotNetRoots.DatabaseAccess;
using Autodesk.DesignScript.Runtime;
using Dynamo.Applications.AdvanceSteel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceSteel.Nodes.Util
{
  [IsVisibleInDynamoLibrary(false)]
  public static class DataBaseUtils
  {
    public static List<(string, string)> GetSubTypes()
    {
      AstorProfiles astorProfiles = AstorProfiles.Instance;
      System.Data.DataTable table = astorProfiles.getProfileSubtypesTable();

      var columnDescription = table.Columns["Description"];
      var columnSubtypeName = table.Columns["SubtypeName"];

      return table.Select().Select(x => (x[columnSubtypeName].ToString(), x[columnDescription].ToString())).ToList();
    }

    public static List<(string, string)> GetTablesBySubTypeName(string subTypeNameFilter)
    {
      if (string.IsNullOrEmpty(subTypeNameFilter))
      {
        return new List<(string, string)>();
      }

      AstorProfiles astorProfiles = AstorProfiles.Instance;
      System.Data.DataTable table = astorProfiles.getProfileMasterTable();

      var columnDescription = table.Columns["RunName"];
      var columnSubTypeName = table.Columns["TypeNameText"];

      var rows = table.Select(string.Format("SubTypeName='{0}'", subTypeNameFilter));

      if (!rows.Any())
      {
        throw new Exception(String.Format(ResourceStrings.Nodes_ProfileSubTypeNotFound, subTypeNameFilter));
      }

      return rows.Select(x => (x[columnSubTypeName].ToString(), x[columnDescription].ToString())).ToList();
    }

    /// <summary>
    /// Get profile sections
    /// </summary>
    /// <param name="typeNameText">sectionType</param>
    /// <returns></returns>
    public static List<(string, string)> GetProfileSectionsByTypeNameText(string typeNameText)
    {
      if (string.IsNullOrEmpty(typeNameText))
      {
        return new List<(string, string)>();
      }

      AstorProfiles astorProfiles = AstorProfiles.Instance;
      System.Data.DataTable table = astorProfiles.getProfileMasterTable();

      var rowSectionType = table.Select(string.Format("TypeNameText='{0}'", typeNameText)).FirstOrDefault();

      if (rowSectionType == null)
      {
        throw new Exception(String.Format(ResourceStrings.Nodes_ProfileSectionTypeNotFound, typeNameText));
      }

      var tableName = rowSectionType["TableName"].ToString();

      var tableProfiles = astorProfiles.getSectionsTable(tableName);

      if (tableProfiles == null)
      {
        throw new Exception(String.Format(ResourceStrings.Nodes_TableNotFound, tableName));
      }

      var columnStandardName = tableProfiles.Columns["StandardName"];
      var columnSectionName = tableProfiles.Columns["SectionName"];

      return tableProfiles.Select().Select(x => (x[columnSectionName].ToString(), x[columnStandardName].ToString())).ToList();
    }

    public static void GetProfileSectionDetails(string sectionType, out string sectionTypeDescription, out string subTypeName, out string subTypeNameDescription)
    {
      sectionTypeDescription = null;
      subTypeName = null;
      subTypeNameDescription = null;

      AstorProfiles astorProfiles = AstorProfiles.Instance;
      System.Data.DataTable tableProfileMaster = astorProfiles.getProfileMasterTable();

      var rowSectionType = tableProfileMaster.Select(string.Format("TypeNameText='{0}'", sectionType)).FirstOrDefault();

      if (rowSectionType == null)
      {
        throw new Exception(String.Format(ResourceStrings.Nodes_ProfileSectionTypeNotFound, sectionType));
      }

      sectionTypeDescription = rowSectionType["RunName"].ToString();

      subTypeName = rowSectionType["SubTypeName"].ToString();

      System.Data.DataTable tableSubTypes = astorProfiles.getProfileSubtypesTable();

      var rowTypeName = tableSubTypes.Select(string.Format("SubtypeName='{0}'", subTypeName)).FirstOrDefault();

      if (rowTypeName == null)
      {
        throw new Exception(String.Format(ResourceStrings.Nodes_ProfileSubTypeNotFound, subTypeName));
      }

      subTypeNameDescription = rowTypeName["Description"].ToString();
    }

    /// <summary>
    /// Check if profile section exists
    /// </summary>
    /// <param name="sectionType"></param>
    /// <param name="sectionName"></param>
    public static void CheckProfileSection(string sectionType, string sectionName)
    {
      AstorProfiles astorProfiles = AstorProfiles.Instance;
      System.Data.DataTable table = astorProfiles.getProfileMasterTable();

      var rowSectionType = table.Select(string.Format("TypeNameText='{0}'", sectionType)).FirstOrDefault();

      if (rowSectionType == null)
      {
        throw new Exception(String.Format(ResourceStrings.Nodes_ProfileSectionTypeNotFound, sectionType));
      }

      var tableName = rowSectionType["TableName"].ToString();

      var tableProfiles = astorProfiles.getSectionsTable(tableName);

      if (tableProfiles == null)
      {
        throw new Exception(String.Format(ResourceStrings.Nodes_TableNotFound, tableName));
      }

      if (!tableProfiles.Select(string.Format("SectionName='{0}'", sectionName)).Any())
      {
        throw new Exception(String.Format(ResourceStrings.Nodes_ProfileSectionNameNotFound, sectionName));
      }
    }
  }
}

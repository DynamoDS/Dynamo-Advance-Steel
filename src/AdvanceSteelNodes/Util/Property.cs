using AdvanceSteel.Nodes.Plates;
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.DesignScript.Runtime;
using DynGeometry = Autodesk.DesignScript.Geometry;
using SteelGeometry = Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.DotNetRoots.DatabaseAccess;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.ConstructionTypes;
using System.Collections.Generic;
using Autodesk.AdvanceSteel.Geometry;
using System.Linq;
using System;


namespace AdvanceSteel.Nodes.Util
{
  /// <summary>
  /// Collection of methods to access steel data
  /// </summary>
  public class Property
  {
    internal Property()
    {
    }

    /// <summary>
    /// Get Single Advance Steel Property from Advance Steel Object
    /// </summary>
    /// <param name="steelObject"> Selected Advance Steel Object</param>
    /// <param name="propertyName"> Advance Steel Property name</param>
    /// <returns></returns>
    public static ASProperty GetPropertyByElement(SteelDbObject steelObject, string propertyName)
    {
      ASProperty ret = null;
      using (var ctx = new SteelServices.DocContext())
      {
        ASProperty extractionProperty = Utils.GetProperty(propertyName, ePropertyDataOperator.Get);
        if (extractionProperty != null)
        {
          if (extractionProperty.EvaluateValueFromSteelDBObject(steelObject))
          {
            ret = extractionProperty;
          }
        }
        else
        {
          throw new System.Exception("InValid Property");
        }
      }
      return ret;
    }

    /// <summary>
    /// Get a list of Advance Steel Properties from Advance Steel Object
    /// </summary>
    /// <param name="steelObject"> Selected Advance Steel Object</param>
    /// <param name="propertyNames"> List of Property names</param>
    /// <returns></returns>
    public static List<ASProperty> GetPropertiesByElement(SteelDbObject steelObject,
                                                List<string> propertyNames)
    {
      List<ASProperty> ret = new List<ASProperty>() { };
      using (var ctx = new SteelServices.DocContext())
      {
        for (int i = 0; i < propertyNames.Count; i++)
        {
          string propertyName = propertyNames[i];
          ASProperty extractionProperty = Utils.GetProperty(propertyName, ePropertyDataOperator.Get);
          if (extractionProperty != null)
          {
            if (extractionProperty.EvaluateValueFromSteelDBObject(steelObject))
            {
              ret.Add(extractionProperty);
            }
          }
          else
          {
            throw new System.Exception("InValid Property");
          }
        }
      }
      return ret;
    }

    /// <summary>
    /// Get all Advance Steel Properties from Advance Steel Object
    /// </summary>
    /// <param name="steelObject"> Advance Steel Object</param>
    /// <returns></returns>
    public static List<ASProperty> GetElementProperties(SteelDbObject steelObject)
    {
      List<ASProperty> ret = new List<ASProperty>() { };
      using (var ctx = new SteelServices.DocContext())
      {
        Dictionary<string, ASProperty> allProperties = Utils.GetAllProperties(ePropertyDataOperator.Get);
        FilerObject fObj = Utils.GetObject(steelObject.Handle);

        foreach (KeyValuePair<string, ASProperty> prop in allProperties)
        {
          if (prop.Value.ElementTypeList.Contains(fObj.Type()))
          {
            ASProperty extractionProperty = prop.Value;
            if (extractionProperty != null)
            {
              if (extractionProperty.EvaluateValueFromSteelDBObject(steelObject))
              {
                ret.Add(extractionProperty);
              }
            }
          }
        }
      }
      return ret;
    }

    /// <summary>
    /// Get the property name
    /// </summary>
    /// <param name="property"> property object to extract information from</param>
    /// <returns></returns>
    public static string GetPropertyName(ASProperty property)
    {
      string ret = string.Empty;

      if (property != null)
      {
        ret = property.Name;
      }
      else
        throw new System.Exception("No Property object found");

      return ret;
    }

    /// <summary>
    /// Check if the property is readonly
    /// </summary>
    /// <param name="property"> property object to extract information from</param>
    /// <returns></returns>
    public static bool IsReadOnly(ASProperty property)
    {
      if (property != null)
      {
        return property.DataOperator == ePropertyDataOperator.Get;
      }
      else
        throw new System.Exception("No Property object found");
    }

    /// <summary>
    /// Get the property value
    /// </summary>
    /// <param name="property"> property object to extract information from</param>
    /// <returns></returns>
    public static object GetPropertyValue(ASProperty property)
    {
      object ret = null;

      if (property != null)
      {
        ret = property.Value;
      }
      else
        throw new System.Exception("No Property object found");

      return ret;
    }

    /// <summary>
    /// Build AS Property - Writeable or Readable
    /// </summary>
    /// <param name="propertyName"> Input Property name from Property Node for particular Object Type</param>
    /// <param name="propertyValue"> Input Property value for Property Type</param>
    /// <returns></returns>
    public static ASProperty ByNameAndValue(string propertyName, object propertyValue)
    {
      ASProperty selectedProperty = Utils.GetProperty(propertyName, ePropertyDataOperator.Set_Get);
      if (selectedProperty != null)
      {
        selectedProperty.Value = propertyValue;
        if (!selectedProperty.hasValidValue())
        {
          throw new System.Exception("Property Value not Valid");
        }
      }
      else
        throw new System.Exception("No Property object found");
      return selectedProperty;
    }

    /// <summary>
    /// Modify Oject Parameters based on a property
    /// </summary>
    /// <param name="objectToModifiy"> Input Object to Modifiy</param>
    /// <param name="parameter"> Input modifcation Value</param>
    /// <returns></returns>
    public static AtomicElement ModifyObjectParameter(AtomicElement objectToModifiy,
                                                      ASProperty parameter)
    {
      if (parameter != null)
      {
        if (objectToModifiy != null)
        {
          parameter.UpdateASObject(objectToModifiy);
        }
      }
      else
        throw new System.Exception("No Advance Steel Object Found");
      return objectToModifiy;
    }

    /// <summary>
    /// Modify Oject Parameters based on list of properties
    /// </summary>
    /// <param name="objectToModifiy"> Input Object to Modifiy</param>
    /// <param name="parameters"> List of Properties to modify </param>
    /// <returns></returns>
    public static AtomicElement ModifyObjectParameters(AtomicElement objectToModifiy,
                                                       List<ASProperty> parameters)
    {
      if (parameters != null)
      {
        if (objectToModifiy != null)
        {
          Utils.SetParameters(objectToModifiy, parameters);
        }
        else
        {
          throw new System.Exception("No Parameter found to be modified");
        }
      }
      else
        throw new System.Exception("No Advance Steel Object Found");
      return objectToModifiy;
    }

    /// <summary>
    /// Set Assembly Location 
    /// </summary>
    /// <param name="connectionType">Input assembly location</param>
    /// <param name="screwBolts">Input aux </param>
    /// <returns></returns>
    public static Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern SetBoltAssemblyLocation(Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern screwBolts, int connectionType)
    {
      if (screwBolts.IsKindOf(FilerObject.eObjectType.kCircleScrewBoltPattern) ||
          screwBolts.IsKindOf(FilerObject.eObjectType.kFinitRectScrewBoltPattern) ||
          screwBolts.IsKindOf(FilerObject.eObjectType.kAnchorPattern))
      {
        screwBolts.AssemblyLocation = (AtomicElement.eAssemblyLocation)connectionType;
      }
      return screwBolts;
    }

    /// <summary>
    /// Set Anchor Bolt Orientation
    /// </summary>
    /// <param name="orientation">Input Anchor Bolt Orientation location</param>
    /// <param name="anchorBolts">Input aux </param>
    /// <returns></returns>
    public static Autodesk.AdvanceSteel.Modelling.AnchorPattern SetAnchorBoltOrientation(Autodesk.AdvanceSteel.Modelling.AnchorPattern anchorBolts, int orientation)
    {
      if (anchorBolts.IsKindOf(FilerObject.eObjectType.kAnchorPattern))
      {
        anchorBolts.OrientationType = (Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType)orientation;
      }
      return anchorBolts;
    }
  }
}

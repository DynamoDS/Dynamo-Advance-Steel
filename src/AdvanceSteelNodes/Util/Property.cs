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
	/// Store Bolts properties in a Node to pass to Bolt Node
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
    /// <param name="propertyType"> Advance Steel Property</param>
    /// <returns></returns>
    public static ASProperty GetPropertyByElement(SteelDbObject steelObject, string propertyType)
    {
      ASProperty ret = null;
      using (var ctx = new SteelServices.DocContext())
      {
        ASProperty extractionProperty = Utils.GetProperty(propertyType, ePropertyDataOperator.Get);
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
    /// Generate a List of Advance Steel Object Properties based on Property Types
    /// </summary>
    /// <param name="steelObject"> Selected Advance Steel Object</param>
    /// <param name="propertyTypes"> List of Property type per Object Type</param>
    /// <returns></returns>
    public static List<ASProperty> GetPropertiesByElement(SteelDbObject steelObject, 
                                                List<string> propertyTypes)
    {
      List<ASProperty> ret = new List<ASProperty>() { };
      using (var ctx = new SteelServices.DocContext())
      {
        for (int i = 0; i < propertyTypes.Count; i++)
        {
          string propertyType = propertyTypes[i];
          ASProperty extractionProperty = Utils.GetProperty(propertyType, ePropertyDataOperator.Get);
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
    /// Generate a List of Advance Steel Object Property Data based on Get Type Properties
    /// </summary>
    /// <param name="steelObject"> Selected Advance Steel Object</param>
    /// <param name="asStringForReference"> Displays the data as String with Property Name as Prefix</param>
    /// <returns></returns>
    public static List<ASProperty> GetElementProperties(SteelDbObject steelObject)
    {
      List<ASProperty> ret = new List<ASProperty>() { };
      using (var ctx = new SteelServices.DocContext())
      {
        Dictionary<string, ASProperty> allProperties = Utils.GetAllProperties(ePropertyDataOperator.Get);
        FilerObject fObj = Utils.GetObject(steelObject.Handle);

        foreach ( KeyValuePair<string, ASProperty> prop in allProperties)
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
    /// Build AS Property - Writeable or Readable
    /// </summary>
    /// <param name="propertyType"> Input Property from Property Node for particular Object Type</param>
    /// <param name="propertyValue"> Input Property value for Property Type</param>
    /// <returns></returns>
    public static ASProperty ByNameAndValue(string propertyType, object propertyValue)
    {
      ASProperty selectedProperty = Utils.GetProperty(propertyType, ePropertyDataOperator.Set_Get);
      if (selectedProperty != null)
      {
        selectedProperty.PropValue = propertyValue;
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

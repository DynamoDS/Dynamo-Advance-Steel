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


namespace AdvanceSteel.Nodes.Selection
{
  /// <summary>
  /// Select Advance Steel Objects
  /// </summary>
  public static class SelectionFilter
  {

    /// <summary>
    /// Filter a collection of steel objects by a list of object types
    /// </summary>
    /// <param name="steelObjects">List of steel objects</param>
    /// <param name="objectTypeFilters"> List of accepted Steel Object Types</param>
    /// <returns></returns>
    public static List<SteelDbObject> FilterSelectionByType(List<SteelDbObject> steelObjects,
                                                  List<int> objectTypeFilters)
    {
      List<SteelDbObject> retListOfFilteredSteelObjects = new List<SteelDbObject>();
      ClassTypeFilter filter = createFilterObject(objectTypeFilters);

      if (objectTypeFilters.Count > 0)
      {
        for (int i = 0; i < steelObjects.Count; i++)
        {
          FilerObject objX = Utils.GetObject(steelObjects[i].Handle);
          if (objX != null)
          {
            if (filter.Filter(objX.Type()) != FilerObject.eObjectType.kUnknown)
            {
              retListOfFilteredSteelObjects.Add(steelObjects[i]);
            }
          }
          else
            throw new System.Exception("No Object return Null during Filtering");
        }
      }
      else
        throw new System.Exception("No Object Filter List Provided");
      return retListOfFilteredSteelObjects;
    }

    private static ClassTypeFilter createFilterObject(List<int> objectFilters)
    {
      ClassTypeFilter filter = new ClassTypeFilter();
      filter.RejectAllFirst();
      for (int i = 0; i < objectFilters.Count; i++)
      {
        filter.AppendAcceptedClass((FilerObject.eObjectType)objectFilters[i]);
      }
      return filter;
    }
  }
}

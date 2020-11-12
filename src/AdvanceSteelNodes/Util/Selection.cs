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
	/// Store Bolts properties in a Node to pass to Bolt Node
	/// </summary>
	public class ObjectSelection
	{
		internal ObjectSelection()
		{
		}

    /// <summary>
    /// Filter a Selection Set based on Object Types
    /// </summary>
    /// <param name="currentSelectionSet"> Current Selection set</param>
    /// <param name="objectTypeFilters"> List of AS Object Types</param>
    /// <returns></returns>
    public static List<SteelDbObject> FilterSelectionByType(List<SteelDbObject> currentSelectionSet,
                                                  List<int> objectTypeFilters)
    {
      List<SteelDbObject> retListOfFilteredSteelObjects = new List<SteelDbObject>();
      ClassTypeFilter filter = createFilterObject(objectTypeFilters);

      if (objectTypeFilters.Count > 0)
      {
        for (int i = 0; i < currentSelectionSet.Count; i++)
        {
          FilerObject objX = Utils.GetObject(currentSelectionSet[i].Handle);
          if (objX != null)
          {
            if (filter.Filter(objX.Type()) != FilerObject.eObjectType.kUnknown)
            {
              retListOfFilteredSteelObjects.Add(currentSelectionSet[i]);
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

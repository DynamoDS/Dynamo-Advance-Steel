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
    /// <returns name="steelObjects"> gets a filtered list of steel objects that match the list of steel object types</returns>
    public static List<SteelDbObject> FilterSelectionByType(List<SteelDbObject> steelObjects, List<string> objectTypeFilters)
    {
      List<SteelDbObject> retListOfFilteredSteelObjects = new List<SteelDbObject>();
      List<Type> typeFilters = Utils.GetASObjectTypeFilters(objectTypeFilters);

      using (var ctx = new SteelServices.DocContext())
      {
        if (objectTypeFilters.Count == 0)
        {
          throw new System.Exception("No Object Filter List Provided");
        }

        for (int i = 0; i < steelObjects.Count; i++)
        {
          FilerObject objX = Utils.GetObject(steelObjects[i].Handle);
          if (objX == null)
          {
            throw new System.Exception("No Object return Null during Filtering");
          }

          var typeAS = objX.GetType();

          if (typeFilters.Any(x => typeAS.IsSubclassOf(x) || typeAS.IsEquivalentTo(x)))
          {
            retListOfFilteredSteelObjects.Add(steelObjects[i]);
          }

        }
      }

      return retListOfFilteredSteelObjects;
    }
  }
}

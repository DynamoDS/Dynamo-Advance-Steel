using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.Modelling;
using AdvanceSteel.Nodes.Plates;
using DynGeometry = Autodesk.DesignScript.Geometry;
using SteelGeometry = Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Arrangement;
using Autodesk.AdvanceSteel.Contours;
using System;
using Autodesk.AdvanceSteel.Connection;

namespace AdvanceSteel.Nodes.Features
{
  /// <summary>
  /// Advance Steel Rectangular Hole Patterns
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class Joint : GraphicObject
  {


    private Joint(UserAutoConstructionObject userAutoConstructionObject)
    {
      SafeInit(() => SetHandle(userAutoConstructionObject));
    }

    internal static Joint FromExisting(UserAutoConstructionObject userAutoConstructionObject)
    {
      return new Joint(userAutoConstructionObject)
      {
        IsOwnedByDynamo = false
      };
    }
  }
}
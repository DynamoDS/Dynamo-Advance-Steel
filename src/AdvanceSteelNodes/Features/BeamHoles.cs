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
using ASConnectionHoleBeam = Autodesk.AdvanceSteel.Modelling.ConnectionHoleBeam;
using Autodesk.AdvanceSteel.Connection;

namespace AdvanceSteel.Nodes.Features
{
  /// <summary>
  /// Advance Steel Rectangular Hole Patterns
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class BeamHoles : GraphicObject
  {


    private BeamHoles(ASConnectionHoleBeam holes)
    {
      SafeInit(() => SetHandle(holes));
    }

    internal static BeamHoles FromExisting(ASConnectionHoleBeam holes)
    {
      return new BeamHoles(holes)
      {
        IsOwnedByDynamo = false
      };
    }
  }
}
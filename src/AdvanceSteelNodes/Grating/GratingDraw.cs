using AdvanceSteel.Nodes.Plates;
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.DesignScript.Runtime;
using DynGeometry = Autodesk.DesignScript.Geometry;
using SteelGeometry = Autodesk.AdvanceSteel.Geometry;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using System.Collections.Generic;
using Autodesk.AdvanceSteel.Geometry;
using System.Linq;
using System;

namespace AdvanceSteel.Nodes.Gratings
{
  [IsVisibleInDynamoLibrary(false)]
  public static class GratingDraw
  {
    public static List<DynGeometry.Point> GetPointsToDraw(Autodesk.AdvanceSteel.Modelling.Grating grating)
    {
      var coordSystem = grating.CS;
      coordSystem.GetCoordSystem(out var origPt, out var vX, out var vY, out var vZ);

      var temp1 = vY * grating.Width / 2.0;
      var temp2 = vX * grating.Length / 2.0;

      var pt1 = new SteelGeometry.Point3d(grating.CenterPoint);
      pt1.Add(temp1 + temp2);

      var pt2 = new SteelGeometry.Point3d(grating.CenterPoint);
      pt2.Add(temp1 - temp2);

      var pt3 = new SteelGeometry.Point3d(grating.CenterPoint);
      pt3.Add(-temp1 - temp2);

      var pt4 = new SteelGeometry.Point3d(grating.CenterPoint);
      pt4.Add(-temp1 + temp2);


      List<DynGeometry.Point> polyPoints = new List<DynGeometry.Point>
      {
        Utils.ToDynPoint(pt1, true),
        Utils.ToDynPoint(pt2, true),
        Utils.ToDynPoint(pt3, true),
        Utils.ToDynPoint(pt4, true)
      };

      return polyPoints;
    }
  }
}

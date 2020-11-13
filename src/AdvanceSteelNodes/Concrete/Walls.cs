using Autodesk.AdvanceSteel.CADAccess;
using SteelGeometry = Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;

using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using System.Collections.Generic;
using Autodesk.AdvanceSteel.CADLink.Database;
using Autodesk.AdvanceSteel.Geometry;

namespace AdvanceSteel.Nodes.Concrete
{
  /// <summary>
  /// Advance Steel plate
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class Walls : GraphicObject
  {
    internal Walls()
    {
    }

    internal Walls(SteelGeometry.Point3d ptCenter, double dLength, double dHeight, double thickness, SteelGeometry.Vector3d vNormal, double side)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          SteelGeometry.Plane plane = new SteelGeometry.Plane(ptCenter, vNormal);
          Autodesk.AdvanceSteel.Modelling.Wall wallObject = null;
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            wallObject = new Autodesk.AdvanceSteel.Modelling.Wall(plane, ptCenter, dLength, dHeight);
            wallObject.Thickness = thickness;
            wallObject.Portioning = side;
            wallObject.WriteToDb();
          }
          else
          {
            wallObject = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Wall;
            if (wallObject != null && wallObject.IsKindOf(FilerObject.eObjectType.kWall))
            {
              wallObject.DefinitionPlane = plane;
              wallObject.Thickness = thickness;
              wallObject.SetLength(dLength, true);
              wallObject.SetWidth(dHeight, true);
              wallObject.Portioning = side;
            }
            else
            {
              throw new System.Exception("Not a Wall");
            }
          }

          Handle = wallObject.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(wallObject);
        }
      }
    }

    internal Walls(SteelGeometry.Matrix3d matrix, double dLength, double dHeight, double thickness, double side)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          SteelGeometry.Point3d baseOrigin = new SteelGeometry.Point3d();
          SteelGeometry.Vector3d xAxis = new SteelGeometry.Vector3d();
          SteelGeometry.Vector3d yAxis = new SteelGeometry.Vector3d();
          SteelGeometry.Vector3d zAxis = new SteelGeometry.Vector3d();
          matrix.GetCoordSystem(out baseOrigin, out xAxis, out yAxis, out zAxis);

          SteelGeometry.Vector3d lengthVec = xAxis * dLength; 
          SteelGeometry.Vector3d heightVec = zAxis * dHeight;

          SteelGeometry.Point3d brPnt = new SteelGeometry.Point3d(baseOrigin).Add(lengthVec);
          SteelGeometry.Point3d trPnt = new SteelGeometry.Point3d(brPnt).Add(heightVec);
          SteelGeometry.Point3d tlPnt = new SteelGeometry.Point3d(baseOrigin).Add(heightVec);

          SteelGeometry.Point3d centerWallPnt = baseOrigin.GetMidPointBetween(trPnt);

          SteelGeometry.Point3d[] wallPoints = { baseOrigin, brPnt, trPnt, tlPnt };
          double[] cornerRadii = (double[])System.Collections.ArrayList.Repeat(0.0, wallPoints.Length).ToArray(typeof(double));

          SteelGeometry.Plane plane = new SteelGeometry.Plane(centerWallPnt, yAxis);

          Autodesk.AdvanceSteel.Modelling.Wall wallObject = null;
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            wallObject = new Autodesk.AdvanceSteel.Modelling.Wall(plane, wallPoints);
            wallObject.Thickness = thickness;
            Polyline3d outerPoly = new Polyline3d(wallPoints, cornerRadii, true, yAxis, false, 0, true, true);
            IEnumerable<ObjectId> deletedFeaturesIds = null;
            IEnumerable<ObjectId> newFeaturesIds = null;
            wallObject.SetOuterContour(outerPoly, out deletedFeaturesIds, out newFeaturesIds);
            wallObject.Portioning = side;
            wallObject.WriteToDb();
          }
          else
          {
            wallObject = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Wall;
            if (wallObject != null && wallObject.IsKindOf(FilerObject.eObjectType.kWall))
            {
              //TODO - Missing SetPolygon
              wallObject.DefinitionPlane = plane;
              wallObject.Thickness = thickness;
              Polyline3d outerPoly = new Polyline3d(wallPoints, cornerRadii, true, yAxis, false, 0, true, true);
              IEnumerable<ObjectId> deletedFeaturesIds = null;
              IEnumerable<ObjectId> newFeaturesIds = null;
              wallObject.SetOuterContour(outerPoly, out deletedFeaturesIds, out newFeaturesIds);
              wallObject.Portioning = side;
            }
            else
            {
              throw new System.Exception("Not a Wall");
            }
          }

          Handle = wallObject.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(wallObject);
        }
      }
    }

    internal Walls(Autodesk.DesignScript.Geometry.Polygon poly, double thickness, double side)
    {
      if (poly.IsPlanar == false)
        throw new System.Exception("Polygon is not planar");

      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Point3d[] astPoints = Utils.ToAstPoints(poly.Points, true);
          double[] cornerRadii = (double[])System.Collections.ArrayList.Repeat(0.0, poly.Points.Length).ToArray(typeof(double));

          var astPoly = new Autodesk.AdvanceSteel.Geometry.Polyline3d(astPoints, null, poly.IsClosed, true);
          var polyPlane = new Plane(astPoints[0], astPoly.Normal);

          Autodesk.AdvanceSteel.Modelling.Wall wallObject = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            wallObject = new Autodesk.AdvanceSteel.Modelling.Wall(polyPlane, astPoints);
            wallObject.Thickness = thickness;
            Polyline3d outerPoly = new Polyline3d(astPoints, cornerRadii, true, astPoly.Normal, false, 0, true, true);
            IEnumerable<ObjectId> deletedFeaturesIds = null;
            IEnumerable<ObjectId> newFeaturesIds = null;
            wallObject.SetOuterContour(outerPoly, out deletedFeaturesIds, out newFeaturesIds);
            wallObject.Portioning = side;
            wallObject.WriteToDb();
          }
          else
          {
            wallObject = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Wall;

            if (wallObject != null && wallObject.IsKindOf(FilerObject.eObjectType.kWall))
            {
              wallObject.DefinitionPlane = polyPlane;
              wallObject.Thickness = thickness;
              Polyline3d outerPoly = new Polyline3d(astPoints, cornerRadii, true, astPoly.Normal, false, 0, true, true);
              IEnumerable<ObjectId> deletedFeaturesIds = null;
              IEnumerable<ObjectId> newFeaturesIds = null;
              wallObject.SetOuterContour(outerPoly, out deletedFeaturesIds, out newFeaturesIds);
              wallObject.Portioning = side;
            }
            else
              throw new System.Exception("Not a Slab");
          }

          Handle = wallObject.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(wallObject);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel Rectangular Wall by defining the extants of the face of the wall by Coordinate System
    /// </summary>
    /// <param name="coordinateSystem"></param>
    /// <param name="length"></param>
    /// <param name="height"></param>
    /// <param name="thickness"></param>
    /// <param name="side">0, 1, 0.5 - Nearside, Farside or Center</param>
    /// <returns></returns>
    public static Walls FaceByLengthHeightByCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem, double length, double height, double thickness, double side)
    {
      return new Walls(Utils.ToAstPoint(coordinateSystem.Origin, true), Utils.ToInternalUnits(length, true), Utils.ToInternalUnits(height, true),
                                 Utils.ToInternalUnits(thickness, true), Utils.ToAstVector3d(coordinateSystem.ZAxis, true), side);
    }

    /// <summary>
    /// Create an Advance Steel Rectangular Wall by defining the extants of the face of the wall by Dynamo Plane
    /// </summary>
    /// <param name="plane"></param>
    /// <param name="length"></param>
    /// <param name="height"></param>
    /// <param name="thickness"></param>
    /// <param name="side">0, 1, 0.5 - Nearside, Farside or Center</param>
    /// <returns></returns>
    public static Walls FaceByLengthHeightByPlane(Autodesk.DesignScript.Geometry.Plane plane, double length, double height, double thickness, double side)
    {
      return new Walls(Utils.ToAstPoint(plane.Origin, true), Utils.ToInternalUnits(length, true), Utils.ToInternalUnits(height, true),
                                 Utils.ToInternalUnits(thickness, true), Utils.ToAstVector3d(plane.Normal, true), side);
    }

    /// <summary>
    /// Create an Advance Steel Wall by Polygon
    /// </summary>
    /// <param name="poly"></param>
    /// <param name="thickness"></param>
    /// <param name="side">0, 1, 0.5 - Nearside, Farside or Center</param>
    /// <returns></returns>
    public static Walls FaceByPolygon(Autodesk.DesignScript.Geometry.Polygon poly, double thickness, double side)
    {
      return new Walls(poly, Utils.ToInternalUnits(thickness, true), side);
    }

    /// <summary>
    /// Creates aa Advance Steel Wall at CS by Length and by Height - Z axis is assumed Wall creation
    /// </summary>
    /// <param name="coordinateSystem"></param>
    /// <param name="length"></param>
    /// <param name="height"></param>
    /// <param name="thickness"></param>
    /// <param name="side">0, 1, 0.5 - Nearside, Farside or Center</param>
    /// <returns></returns>
    public static Walls AtBaseByLengthHeightByCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem, double length, double height, double thickness, double side)
    {
      return new Walls(Utils.ToAstMatrix3d(coordinateSystem, true), Utils.ToInternalUnits(length, true), Utils.ToInternalUnits(height, true),
                                 Utils.ToInternalUnits(thickness, true), side);
    }



    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var walls = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.Wall;

          SteelGeometry.Polyline3d astPoly = null;
          walls.GetBaseContourPolygon(0.0, out astPoly);

          var dynPoints = Utils.ToDynPoints(astPoly.Vertices, true);
          var poly = Autodesk.DesignScript.Geometry.Polygon.ByPoints(dynPoints, astPoly.IsClosed);

          foreach (var pt in dynPoints) { pt.Dispose(); }

          return poly;
        }
      }
    }
  }
}
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.CADLink.Database;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Concrete
{
  /// <summary>
  /// Advance Steel plate
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class Slabs : GraphicObject
  {
    internal Slabs()
    {
    }

    internal Slabs(Autodesk.DesignScript.Geometry.Polygon poly, double thickness, double side)
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

          Autodesk.AdvanceSteel.Modelling.Slab floorSlab = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            floorSlab = new Autodesk.AdvanceSteel.Modelling.Slab(polyPlane, astPoints);
            floorSlab.Thickness = thickness;
            Polyline3d outerPoly = new Polyline3d(astPoints, cornerRadii, true, astPoly.Normal, false, 0, true, true);
            IEnumerable<ObjectId> deletedFeaturesIds = null;
            IEnumerable<ObjectId> newFeaturesIds = null;
            floorSlab.SetOuterContour(outerPoly, out deletedFeaturesIds, out newFeaturesIds);
            floorSlab.Portioning = side;
            floorSlab.WriteToDb();
          }
          else
          {
            floorSlab = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Slab;

            if (floorSlab != null && floorSlab.IsKindOf(FilerObject.eObjectType.kSlab))
            {
              floorSlab.DefinitionPlane = polyPlane;
              floorSlab.Thickness = thickness;
              Polyline3d outerPoly = new Polyline3d(astPoints, cornerRadii, true, astPoly.Normal, false, 0, true, true);
              IEnumerable<ObjectId> deletedFeaturesIds = null;
              IEnumerable<ObjectId> newFeaturesIds = null;
              floorSlab.SetOuterContour(outerPoly, out deletedFeaturesIds, out newFeaturesIds);
              floorSlab.Portioning = side;
            }
            else
              throw new System.Exception("Not a Slab");
          }

          Handle = floorSlab.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(floorSlab);
        }
      }
    }

    internal Slabs(Point3d ptCenter, double dWidth, double dLength, double thickness, Vector3d vNormal, double side)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(ptCenter, vNormal);
          Autodesk.AdvanceSteel.Modelling.Slab floorSlab = null;
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            floorSlab = new Autodesk.AdvanceSteel.Modelling.Slab(plane, ptCenter, dWidth, dLength);
            floorSlab.SetLength(dWidth, true);
            floorSlab.SetWidth(dLength, true);
            floorSlab.Thickness = thickness;
            floorSlab.Portioning = side;
            floorSlab.WriteToDb();
          }
          else
          {
            floorSlab = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Slab;
            if (floorSlab != null && floorSlab.IsKindOf(FilerObject.eObjectType.kSlab))
            {
              floorSlab.DefinitionPlane = plane;
              floorSlab.Thickness = thickness;
              floorSlab.SetLength(dWidth, true);
              floorSlab.SetWidth(dLength, true);
              floorSlab.Portioning = side;
            }
            else
            {
              throw new System.Exception("Not a Slab");
            }
          }

          Handle = floorSlab.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(floorSlab);
        }
      }
    }

    internal Slabs(Point3d ptCenter, double dRadius, double thickness, Vector3d vNormal, double side)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(ptCenter, vNormal);
          Autodesk.AdvanceSteel.Modelling.Slab floorSlab = null;
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            floorSlab = new Autodesk.AdvanceSteel.Modelling.Slab(plane, ptCenter, dRadius, 0);
            floorSlab.setRadius(dRadius, true); //Not Working
            floorSlab.Thickness = thickness;
            floorSlab.Portioning = side;
            floorSlab.WriteToDb();
          }
          else
          {
            floorSlab = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Slab;
            if (floorSlab != null && floorSlab.IsKindOf(FilerObject.eObjectType.kSlab))
            {
              floorSlab.DefinitionPlane = plane;
              floorSlab.setRadius(dRadius, true); //Not Working
              floorSlab.Thickness = thickness;
              floorSlab.Portioning = side;
            }
            else
            {
              throw new System.Exception("Not a Slab");
            }
          }

          Handle = floorSlab.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(floorSlab);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel Slab By Polygon and Thickness
    /// </summary>
    /// <param name="poly">Input Dynamo Polygon</param>
    /// <param name="thickness">Slab Thickness in Current Model Length Unit Settings</param>
    /// <param name="side">0, 1, 0.5 - Nearside, Farside or Center</param>
    /// <returns></returns>
    public static Slabs ByPolygon(Autodesk.DesignScript.Geometry.Polygon poly, double thickness, double side)
    {
      return new Slabs(poly, thickness, side);
    }

    /// <summary>
    /// Create an Advance Steel Rectangular Slab By Thickness
    /// </summary>
    /// <param name="coordinateSystem">Input Dynamo Coordinate System</param>
    /// <param name="width">Slab Width in Current Model Advance Steel Length Unit Settings</param>
    /// <param name="length">Slab Length in Current Model Advance Steel Length Unit Settings</param>
    /// <param name="thickness">Slab Thickness in Current Model Length Unit Settings</param>
    /// <param name="side">0, 1, 0.5 - Nearside, Farside or Center</param>
    /// <returns></returns>
    public static Slabs ByRectangularByCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem, double width, double length, double thickness, double side)
    {
      return new Slabs(Utils.ToAstPoint(coordinateSystem.Origin, true), Utils.ToInternalUnits(width, true), Utils.ToInternalUnits(length, true),
                                 Utils.ToInternalUnits(thickness, true), Utils.ToAstVector3d(coordinateSystem.ZAxis, true), side);
    }

    /// <summary>
    /// Create an Advance Steel Radial Slab By Thickness
    /// </summary>
    /// <param name="coordinateSystem"></param>
    /// <param name="radius">Slab Radius in Current Model Length Unit Settings</param>
    /// <param name="thickness">Slab Thickness in Current Model Length Unit Settings</param>
    /// <param name="side">0, 1, 0.5 - Nearside, Farside or Center</param>
    /// <returns></returns>
    private static Slabs ByCircularByCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem, double radius, double thickness, double side)
    {
      return new Slabs(Utils.ToAstPoint(coordinateSystem.Origin, true), Utils.ToInternalUnits(radius, true), Utils.ToInternalUnits(thickness, true), Utils.ToAstVector3d(coordinateSystem.ZAxis, true), side);
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var slab = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.Slab;

          Polyline3d astPoly = null;
          slab.GetBaseContourPolygon(0.0, out astPoly);

          var dynPoints = Utils.ToDynPoints(astPoly.Vertices, true);
          var poly = Autodesk.DesignScript.Geometry.Polygon.ByPoints(dynPoints, astPoly.IsClosed);

          foreach (var pt in dynPoints) { pt.Dispose(); }

          return poly;
        }
      }
    }
  }
}
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;

using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Concrete
{
  /// <summary>
  /// Advance Steel Isloated Footing
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class Footings : GraphicObject
  {
    internal Footings(Point3d ptCenter, Vector3d vNormal, double depth, double radius)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(ptCenter, vNormal);

          Autodesk.AdvanceSteel.Modelling.FootingIsolated padFooting = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            padFooting = new Autodesk.AdvanceSteel.Modelling.FootingIsolated(plane, ptCenter, radius);
            padFooting.Thickness = depth;
            padFooting.WriteToDb();
          }
          else
          {
            padFooting = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.FootingIsolated;

            if (padFooting != null && padFooting.IsKindOf(FilerObject.eObjectType.kFootingIsolated))
            {
              padFooting.DefinitionPlane = plane;
              padFooting.Thickness = depth;
              padFooting.setRadius(radius, true);
            }
            else
              throw new System.Exception("Not an Isolated Footing");
          }

          Handle = padFooting.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(padFooting);
        }
      }
    }

    internal Footings(Point3d ptCenter, Vector3d vNormal, double depth, double width, double length)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(ptCenter, vNormal);

          Autodesk.AdvanceSteel.Modelling.FootingIsolated padFooting = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            padFooting = new Autodesk.AdvanceSteel.Modelling.FootingIsolated(plane, ptCenter, width, length);
            padFooting.SetLength(length, false);
            padFooting.SetWidth(width, false);
            padFooting.Thickness = depth;
            padFooting.WriteToDb();
          }
          else
          {
            padFooting = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.FootingIsolated;

            if (padFooting != null && padFooting.IsKindOf(FilerObject.eObjectType.kFootingIsolated))
            {
              padFooting.DefinitionPlane = plane;
              padFooting.Thickness = depth;
              padFooting.SetLength(length, false);
              padFooting.SetWidth(width, false);
            }
            else
              throw new System.Exception("Not an Isolated Footing");
          }

          Handle = padFooting.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(padFooting);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel Isolated Footing - Circular
    /// </summary>
    /// <param name="coordinateSystem">Footing Insert Coordinate System</param>
    /// <param name="footingDepth">Depth of Footing</param>
    /// <param name="footingRadius">Footing Radius</param>
    /// <returns></returns>
    public static Footings ByRadiusOnCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem, double footingDepth, double footingRadius)
    {
      return new Footings(Utils.ToAstPoint(coordinateSystem.Origin, true), Utils.ToAstVector3d(coordinateSystem.ZAxis, true), footingDepth, footingRadius);
    }

    /// <summary>
    /// Create an Advance Steel Isolated Footing - Rectangular
    /// </summary>
    /// <param name="coordinateSystem"></param>
    /// <param name="footingDepth"></param>
    /// <param name="footingWidth"></param>
    /// <param name="footingLength"></param>
    /// <returns></returns>
    public static Footings ByLengthWidthOnCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem, double footingDepth, double footingWidth, double footingLength)
    {
      return new Footings(Utils.ToAstPoint(coordinateSystem.Origin, true), Utils.ToAstVector3d(coordinateSystem.ZAxis, true), footingDepth, footingWidth,footingLength);
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var padFooting = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.FootingIsolated;

          Polyline3d astPoly = null;
          padFooting.GetBaseContourPolygon(0.0, out astPoly);

          var dynPoints = Utils.ToDynPoints(astPoly.Vertices, true);
          var poly = Autodesk.DesignScript.Geometry.Polygon.ByPoints(dynPoints, astPoly.IsClosed);

          foreach (var pt in dynPoints) { pt.Dispose(); }

          return poly;
        }
      }
    }
  }
}
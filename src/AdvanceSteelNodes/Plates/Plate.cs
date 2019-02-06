using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;

using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Plates
{
  /// <summary>
  /// Advance Steel plate
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class Plate : GraphicObject
  {
    internal Plate(Autodesk.DesignScript.Geometry.Polygon poly)
    {
      if (poly.IsPlanar == false)
        throw new System.Exception("Polygon is not planar");

      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Point3d[] astPoints = Utils.ToAstPoints(poly.Points, true);
          var astPoly = new Autodesk.AdvanceSteel.Geometry.Polyline3d(astPoints, null, poly.IsClosed, true);
          var polyPlane = new Plane(astPoints[0], astPoly.Normal);

          Autodesk.AdvanceSteel.Modelling.Plate plate = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            plate = new Autodesk.AdvanceSteel.Modelling.Plate(polyPlane, astPoints);

            plate.WriteToDb();
          }
          else
          {
            plate = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Plate;

            if (plate != null && plate.IsKindOf(FilerObject.eObjectType.kPlate))
            {
              plate.DefinitionPlane = polyPlane;
              plate.SetPolygonContour(astPoints);
            }
            else
              throw new System.Exception("Not a Plate");
          }

          Handle = plate.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(plate);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel plate
    /// </summary>
    /// <param name="poly">Input polygon</param>
    /// <returns></returns>
    public static Plate ByPolygon(Autodesk.DesignScript.Geometry.Polygon poly)
    {
      return new Plate(poly);
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var plate = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.Plate;

          Polyline3d astPoly = null;
          plate.GetBaseContourPolygon(0.0, out astPoly);

          var dynPoints = Utils.ToDynPoints(astPoly.Vertices, true);
          var poly = Autodesk.DesignScript.Geometry.Polygon.ByPoints(dynPoints, astPoly.IsClosed);

          foreach (var pt in dynPoints) { pt.Dispose(); }

          return poly;
        }
      }
    }
  }
}
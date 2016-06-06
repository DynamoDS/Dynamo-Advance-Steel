using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;

namespace AdvanceSteel.Nodes.Plates
{
  /// <summary>DynamoServices
  /// An AdvanceSteel Plate
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class Plate : GraphicObject
  {
    internal Plate(Autodesk.DesignScript.Geometry.Polygon poly)
    {
      if (poly.IsPlanar == false)
        throw new System.Exception("Polygon is not planar");

      //use lock just to be safe
      //AutoCAD does not support multithreaded access
      lock (myLock)
      {
        //lock the document and start transaction
        using (var _CADAccess = new AdvanceSteel.Services.ObjectAccess.CADContext())
        {
          string handle = AdvanceSteel.Services.ElementBinder.GetHandleFromTrace();

          Point3d[] astPoints = Utils.ToAstPoints(poly.Points, true);
          var astPoly = new Autodesk.AdvanceSteel.Geometry.Polyline3d(astPoints, null, poly.IsClosed, true);
          var polyPlane = new Plane(astPoints[0], astPoly.Normal);

          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            var myPlate = new Autodesk.AdvanceSteel.Modelling.Plate(polyPlane, astPoints);

            myPlate.WriteToDb();
            handle = myPlate.Handle;
          }

          Autodesk.AdvanceSteel.Modelling.Plate plate = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Plate;

          if (plate != null && plate.IsKindOf(FilerObject.eObjectType.kPlate))
          {
            plate.DefinitionPlane = polyPlane;
            plate.SetPolygonContour(astPoints);
          }
          else
            throw new System.Exception("Not a Plate");

          this.Handle = handle;

          AdvanceSteel.Services.ElementBinder.CleanupAndSetElementForTrace(plate);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel plate
    /// </summary>
    /// <param name="poly"></param>
    /// <returns></returns>
    public static Plate ByPolygon(Autodesk.DesignScript.Geometry.Polygon poly)
    {
      return new Plate(poly);
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve Curve
    {
      get
      {
        //use lock just to be safe
        //AutoCAD does not support multithreaded access
        lock (myLock)
        {
          using (var _CADAccess = new AdvanceSteel.Services.ObjectAccess.CADContext())
          {
            var plate = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.Plate;

            Polyline3d astPoly = null;
            plate.GetBaseContourPolygon(0.0, out astPoly);

            var poly = Autodesk.DesignScript.Geometry.Polygon.ByPoints(Utils.ToDynPoints(astPoly.Vertices, true), astPoly.IsClosed);
            return poly;
          }
        }
      }
    }
  }
}
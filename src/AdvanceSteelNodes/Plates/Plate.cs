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
    internal Plate(Autodesk.DesignScript.Geometry.Polygon poly, double thickness, double side)
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
            if (thickness > 0)
            {
              plate.Thickness = thickness;
            }
            plate.Portioning = side;
            plate.WriteToDb();
          }
          else
          {
            plate = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Plate;

            if (plate != null && plate.IsKindOf(FilerObject.eObjectType.kPlate))
            {
              plate.DefinitionPlane = polyPlane;
              if (thickness > 0)
              {
                plate.Thickness = thickness;
              }
              plate.Portioning = side;
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

    internal Plate(Point3d planePoint, Vector3d normal, double thickness, double length, double width, double side, int corner)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();
          var polyPlane = new Plane(planePoint, normal);

          Autodesk.AdvanceSteel.Modelling.Plate plate = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            plate = new Autodesk.AdvanceSteel.Modelling.Plate(polyPlane, planePoint, length, width);
            plate.SetLengthAndWidth(length, width, 1);
            if (thickness > 0)
            {
              plate.Thickness = thickness;
            }
            Vector2d offset;
            switch (corner)
            {
              case 0:  //Top Left
                offset = new Vector2d(-1, 1);
                break;
              case 1: //Top Right
                offset = new Vector2d(1, 1);
                break;
              case 2: //Bottom Right
                offset = new Vector2d(1, -1);
                break;
              case 3: //Bottom left
                offset = new Vector2d(-1, -1);
                break;
              default: //Anything else ignore
                offset = new Vector2d(0,0);
                break;
            }
            plate.Offset = offset;
            plate.Portioning = side;
            plate.WriteToDb();
          }
          else
          {
            plate = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Plate;
            if (plate != null && plate.IsKindOf(FilerObject.eObjectType.kPlate))
            {
              plate.DefinitionPlane = polyPlane;
              if (thickness > 0)
              {
                plate.Thickness = thickness;
              }
              plate.SetLengthAndWidth(length, width, 1);
              Vector2d offset;
              switch (corner)
              {
                case 0:  //Top Left
                  offset = new Vector2d(-1, 1);
                  break;
                case 1: //Top Right
                  offset = new Vector2d(1, 1);
                  break;
                case 2: //Bottom Right
                  offset = new Vector2d(1, -1);
                  break;
                case 3: //Bottom left
                  offset = new Vector2d(-1, -1);
                  break;
                default: //Anything else ignore
                  offset = new Vector2d(0, 0);
                  break;
              }
              plate.Offset = offset;
              plate.Portioning = side;
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
    /// Create an Advance Steel plate (Position default to 0, with system thickness as default)
    /// </summary>
    /// <param name="poly">Input Dynamo Closed Polygon</param>
    /// <returns></returns>
    public static Plate ByPolygon(Autodesk.DesignScript.Geometry.Polygon poly)
    {
      return new Plate(poly, 0, 0);
    }

    /// <summary>
    /// Create an Advance Steel Plate by polygon, including setting the side. Thickness is as system Default
    /// </summary>
    /// <param name="poly">Input Dynamo Closed Polygon</param>
    /// <param name="side">Input Plate Side - 0, 0.5, 1</param>
    /// <returns></returns>
    public static Plate ByPolygon(Autodesk.DesignScript.Geometry.Polygon poly, double side)
    {
      return new Plate(poly, 0, side);
    }

    /// <summary>
    /// Create an Advance Steel Plate by polygon, including setting the side and thickness
    /// </summary>
    /// <param name="poly">Input Dynamo Closed Polygon</param>
    /// <param name="thickness">Input Plate Thickness - 0 will use system defaults</param>
    /// <param name="side">Input Plate Side - 0, 0.5, 1</param>
    /// <returns></returns>
    public static Plate ByPolygon(Autodesk.DesignScript.Geometry.Polygon poly, double thickness, double side)
    {
      return new Plate(poly, thickness, side);
    }

    /// <summary>
    /// Create a rectangular Advance Steel Plate at CS by length and width, including setting the side and thickness
    /// </summary>
    /// <param name="coordinateSystem">Input Dynamo CoordinateSytem</param>
    /// <param name="length">Input Plate Length</param>
    /// <param name="width">Input Plate Width</param>
    /// <param name="thickness">Input Plate Thickness - 0 will use system defaults</param>
    /// <param name="side">Input Plate Side - 0, 0.5, 1</param>
    /// <param name="corner">0 - TL, 1 - TR, 2 - BR, 3 - BL, else center</param>
    /// <returns></returns>
    public static Plate ByRectanglarByCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem, double length, double width, double thickness, double side, int corner)
    {
      return new Plate(Utils.ToAstPoint(coordinateSystem.Origin, true), Utils.ToAstVector3d(coordinateSystem.ZAxis, true), Utils.ToInternalUnits(thickness, true), Utils.ToInternalUnits(length, true), Utils.ToInternalUnits(width, true), side, corner);
    }

    /// <summary>
    /// Create Advance Steel plate by Length, Width and Normal - Include thickness, side and corner
    /// </summary>
    /// <param name="origin">Input Plate Orgin Point</param>
    /// <param name="normal">Input Plate Normal</param>
    /// <param name="length">Input Plate Length</param>
    /// <param name="width">Input Plate Width</param>
    /// <param name="thickness">Input Plate Thickness - 0 will use system defaults</param>
    /// <param name="side">Input Plate Side - 0, 0.5, 1</param>
    /// <param name="corner">0 - TL, 1 - TR, 2 - BR, 3 - BL, else center</param>
    /// <returns></returns>
    public static Plate ByRectanglarByPointandNormal(Autodesk.DesignScript.Geometry.Point origin, Autodesk.DesignScript.Geometry.Vector normal, double length, double width, double thickness, double side, int corner)
    {
      return new Plate(Utils.ToAstPoint(origin, true), Utils.ToAstVector3d(normal, true), Utils.ToInternalUnits(thickness, true), Utils.ToInternalUnits(length, true), Utils.ToInternalUnits(width, true), side, corner);
    }

    /// <summary>
    /// Create a rectangular Advance Steel Plate at the location of Dynamo Line with Width, including setting the side and thickness
    /// </summary>
    /// <param name="line">Input Dynamo Line</param>
    /// <param name="normal">Input Plate Normal</param>
    /// <param name="width">Input Plate Width</param>
    /// <param name="thickness">Input Plate Thickness</param>
    /// <param name="side">Input Plate Side - 0, 0.5, 1</param>
    /// <returns></returns>
    public static Plate ByLengthEdge(Autodesk.DesignScript.Geometry.Line line, Autodesk.DesignScript.Geometry.Vector normal, double width, double thickness, double side)
    {
      if (line.Length == 0)
        throw new System.Exception("Line length Cant be Zero");
      return new Plate(Utils.ToAstPoint(line.StartPoint, true), Utils.ToAstVector3d(normal, true), Utils.ToInternalUnits(thickness, true), Utils.ToInternalUnits(line.Length, true), Utils.ToInternalUnits(width, true), side, 1);
    }

    /// <summary>
    /// Create a rectangular Advance Steel Plate at 2 points for length with Width, including setting the side and thickness
    /// </summary>
    /// <param name="startPoint">Input Start Point of Length</param>
    /// <param name="EndPoint">Input End Point of Length</param>
    /// <param name="normal">Input Plate Normal</param>
    /// <param name="width">Input Plate Width</param>
    /// <param name="thickness">Input Plate Thickness</param>
    /// <param name="side">Input Plate Side - 0, 0.5, 1</param>
    /// <returns></returns>
    public static Plate ByLengthEdge(Autodesk.DesignScript.Geometry.Point startPoint, Autodesk.DesignScript.Geometry.Point EndPoint, Autodesk.DesignScript.Geometry.Vector normal, double width, double thickness, double side)
    {
      double length = Utils.ToAstPoint(startPoint, true).DistanceTo(Utils.ToAstPoint(EndPoint, true));
      if (length == 0)
        throw new System.Exception("Distance between 2 points Cant be Zero");
      return new Plate(Utils.ToAstPoint(startPoint, true), Utils.ToAstVector3d(normal, true), Utils.ToInternalUnits(thickness, true), Utils.ToInternalUnits(length, true), Utils.ToInternalUnits(width, true), side, 1);
    }

    /// <summary>
    /// Create a rectangular Advance Steel Plate by using 2 points at opposite corners for extents, including setting the side and thickness
    /// </summary>
    /// <param name="cs">Input Dynamo Corrdinate System</param>
    /// <param name="cornerPoint1">Input Point of Lower Left of plate</param>
    /// <param name="cornerPoint2">Input Point of Upper right of plate</param>
    /// <param name="thickness">Input Plate Thickness</param>
    /// <param name="side">Input Plate Side - 0, 0.5, 1</param>
    /// <returns></returns>
    public static Plate ByTwoDiagonalPointsByCS(Autodesk.DesignScript.Geometry.CoordinateSystem cs, Autodesk.DesignScript.Geometry.Point cornerPoint1, Autodesk.DesignScript.Geometry.Point cornerPoint2, double thickness, double side)
    {
      Point3d cpt1 = Utils.ToAstPoint(cornerPoint1, true);
      Point3d cpt2 = Utils.ToAstPoint(cornerPoint2, true);
      Plane ply1 = new Plane(cpt1, Utils.ToAstVector3d(cs.YAxis, true));
      Plane plx2 = new Plane(cpt2, Utils.ToAstVector3d(cs.XAxis, true));
      double width = cpt1.OrthoProject(plx2).DistanceTo(cpt1);
      if (width == 0)
        throw new System.Exception("Width Cant be Zero");
      double length = cpt2.OrthoProject(ply1).DistanceTo(cpt2);
      if (length == 0)
        throw new System.Exception("Length Cant be Zero");
      return new Plate(Utils.ToAstPoint(cs.Origin, true), Utils.ToAstVector3d(cs.ZAxis, true), Utils.ToInternalUnits(thickness, true), Utils.ToInternalUnits(length, true), Utils.ToInternalUnits(width, true), side, 0);
    }

    /// <summary>
    /// Create an Advance Steel Plate by 3 Points - Orgin, X Direction & Y Direction
    /// </summary>
    /// <param name="orginPoint">Input the Origin of the Rectangular Plate</param>
    /// <param name="xDirectionPoint">Input Point in the X Direction - distance from orgin will determine the width</param>
    /// <param name="yDirectionPoint">Input Point in approximate Y Direction - True Y Direction will get recalculated</param>
    /// <param name="thickness">Input Plate Thickness</param>
    /// <param name="side">Input Plate Side - 0, 0.5, 1</param>
    /// <returns></returns>
    public static Plate ByThreePoints(Autodesk.DesignScript.Geometry.Point orginPoint, Autodesk.DesignScript.Geometry.Point xDirectionPoint, Autodesk.DesignScript.Geometry.Point yDirectionPoint, double thickness, double side)
    {
      Point3d cpOrigin = Utils.ToAstPoint(orginPoint, true);
      Point3d xDPoint = Utils.ToAstPoint(xDirectionPoint, true);
      Point3d yDPoint = Utils.ToAstPoint(yDirectionPoint, true);

      Vector3d xAxis = xDPoint.Subtract(cpOrigin);
      Vector3d yAxis = yDPoint.Subtract(cpOrigin);
      
      Vector3d zAxis = xAxis.CrossProduct(yAxis);
      yAxis = xAxis.CrossProduct(zAxis);
      Plane yPlane = new Plane(yDPoint, yAxis);
      Point3d finalYPoint = cpOrigin.OrthoProject(yPlane);

      double width = cpOrigin.DistanceTo(xDPoint);
      if (width == 0)
        throw new System.Exception("Width Cant be Zero");
      double length = cpOrigin.DistanceTo(finalYPoint);
      if (length == 0)
        throw new System.Exception("Length Cant be Zero");

      return new Plate(cpOrigin, zAxis, Utils.ToInternalUnits(thickness, true), Utils.ToInternalUnits(width, true), Utils.ToInternalUnits(length , true), side, 0);
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
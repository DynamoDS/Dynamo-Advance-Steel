using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using ASPlate = Autodesk.AdvanceSteel.Modelling.Plate;

namespace AdvanceSteel.Nodes.Plates
{
  /// <summary>
  /// Advance Steel Plate
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class Plate : GraphicObject
  {
    private Plate(Point3d[] astPoints, Vector3d normal, List<Property> plateProperties)
    {
      SafeInit(() => InitPlate(astPoints, normal, plateProperties));
    }

    private Plate(Point3d planePoint, Vector3d normal, double length, double width, int corner, List<Property> plateProperties)
    {
      SafeInit(() => InitPlate(planePoint, normal, length, width, corner, plateProperties));
    }

    private Plate(ASPlate plate)
    {
      SafeInit(() => SetHandle(plate));
    }

    internal static Plate FromExisting(ASPlate plate)
    {
      return new Plate(plate)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitPlate(Point3d[] astPoints, Vector3d normal, List<Property> plateProperties)
    {
      //if (poly.IsPlanar == false)
      //  throw new System.Exception("Polygon is not planar");

      List<Property> defaultData = plateProperties.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> postWriteDBData = plateProperties.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();

      //Point3d[] astPoints = Utils.ToAstPoints(poly.Points, true);
      var astPoly = new Autodesk.AdvanceSteel.Geometry.Polyline3d(astPoints, null, true, true);

      if (astPoly.IsPlanar == false)
        throw new System.Exception("Polygon is not planar");

      var polyPlane = new Plane(astPoints[0], normal); // astPoly.Normal);

      ASPlate plate = SteelServices.ElementBinder.GetObjectASFromTrace<ASPlate>();
      if (plate == null)
      {
        plate = new ASPlate(polyPlane, astPoints);
        if (defaultData != null)
        {
          Utils.SetParameters(plate, defaultData);
        }

        plate.WriteToDb();
      }
      else
      {
        if (!plate.IsKindOf(FilerObject.eObjectType.kPlate))
          throw new System.Exception("Not a Plate");

        plate.DefinitionPlane = polyPlane;
        if (defaultData != null)
        {
          Utils.SetParameters(plate, defaultData);
        }

        plate.SetPolygonContour(astPoints);
      }

      SetHandle(plate);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(plate, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(plate);
    }

    private void InitPlate(Point3d planePoint, Vector3d normal, double length, double width, int corner, List<Property> plateProperties)
    {
      List<Property> defaultData = plateProperties.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> postWriteDBData = plateProperties.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();

      var polyPlane = new Plane(planePoint, normal);

      ASPlate plate = SteelServices.ElementBinder.GetObjectASFromTrace<ASPlate>();
      if (plate == null)
      {
        plate = new ASPlate(polyPlane, planePoint, length, width);
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

        if (defaultData != null)
        {
          Utils.SetParameters(plate, defaultData);
        }

        plate.WriteToDb();
      }
      else
      {
        if (!plate.IsKindOf(FilerObject.eObjectType.kPlate))
          throw new System.Exception("Not a Plate");

        plate.DefinitionPlane = polyPlane;
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

        if (defaultData != null)
        {
          Utils.SetParameters(plate, defaultData);
        }

        plate.Offset = offset;
      }

      SetHandle(plate);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(plate, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(plate);
    }

    /// <summary>
    /// Create an Advance Steel plate (Position default to 0, with system thickness as default)
    /// </summary>
    /// <param name="poly">Input Dynamo Closed Polygon</param>
    /// <param name="additionalPlateParameters"> Optional Input Plate Build Properties </param>
    /// <returns name="plate"> plate</returns>
    public static Plate ByPolygon(Autodesk.DesignScript.Geometry.Polygon poly,
                                  [DefaultArgument("null")] List<Property> additionalPlateParameters)
    {
      additionalPlateParameters = PreSetDefaults(additionalPlateParameters);
      Point3d[] astPoints = Utils.ToAstPoints(poly.Points, true);
      return new Plate(astPoints, Utils.ToAstVector3d(poly.Normal, true), additionalPlateParameters);
    }

    /// <summary>
    /// Create a rectangular Advance Steel Plate at CS by length and width, including setting the side and thickness
    /// </summary>
    /// <param name="coordinateSystem">Input Dynamo CoordinateSytem</param>
    /// <param name="length">Input Plate Length</param>
    /// <param name="width">Input Plate Width</param>
    /// <param name="corner">0 - TL, 1 - TR, 2 - BR, 3 - BL, else center</param>
    /// <param name="additionalPlateParameters"> Optional Input Plate Build Properties </param>
    /// <returns name="plate"> plate</returns>
    public static Plate ByRectanglarByCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                          double length,
                                          double width,
                                          [DefaultArgument("-1")] int corner,
                                          [DefaultArgument("null")] List<Property> additionalPlateParameters)
    {
      if (length == 0)
        throw new System.Exception("Length Cant be Zero");
      if (width == 0)
        throw new System.Exception("Width Cant be Zero");
      additionalPlateParameters = PreSetDefaults(additionalPlateParameters);
      return new Plate(Utils.ToAstPoint(coordinateSystem.Origin, true), Utils.ToAstVector3d(coordinateSystem.ZAxis, true), Utils.ToInternalDistanceUnits(length, true), Utils.ToInternalDistanceUnits(width, true), corner, additionalPlateParameters);
    }

    /// <summary>
    /// Create Advance Steel plate by Length, Width and Normal - Include thickness, side and corner
    /// </summary>
    /// <param name="origin">Input Plate Orgin Point</param>
    /// <param name="normal">Input Plate Normal</param>
    /// <param name="length">Input Plate Length</param>
    /// <param name="width">Input Plate Width</param>
    /// <param name="corner">0 - TL, 1 - TR, 2 - BR, 3 - BL, else center</param>
    /// <param name="additionalPlateParameters"> Optional Input Plate Build Properties </param>
    /// <returns name="plate"> plate</returns>
    public static Plate ByRectanglarByPointandNormal(Autodesk.DesignScript.Geometry.Point origin,
                                                      Autodesk.DesignScript.Geometry.Vector normal,
                                                      double length,
                                                      double width,
                                                      [DefaultArgument("-1")] int corner,
                                                      [DefaultArgument("null")] List<Property> additionalPlateParameters)
    {
      if (length == 0)
        throw new System.Exception("Length Cant be Zero");
      if (width == 0)
        throw new System.Exception("Width Cant be Zero");
      additionalPlateParameters = PreSetDefaults(additionalPlateParameters);
      return new Plate(Utils.ToAstPoint(origin, true), Utils.ToAstVector3d(normal, true), Utils.ToInternalDistanceUnits(length, true), Utils.ToInternalDistanceUnits(width, true), corner, additionalPlateParameters);
    }

    /// <summary>
    /// Create a rectangular Advance Steel Plate at the location of Dynamo Line with Width, including setting the side and thickness
    /// </summary>
    /// <param name="line">Input Dynamo Line</param>
    /// <param name="normal">Input Plate Normal</param>
    /// <param name="width">Input Plate Width</param>
    /// <param name="additionalPlateParameters"> Optional Input Plate Build Properties </param>
    /// <returns name="plate"> plate</returns>
    public static Plate ByLengthEdge(Autodesk.DesignScript.Geometry.Line line,
                                      Autodesk.DesignScript.Geometry.Vector normal,
                                      double width,
                                      [DefaultArgument("null")] List<Property> additionalPlateParameters)
    {
      if (line.Length == 0)
        throw new System.Exception("Line length Cant be Zero");
      if (width == 0)
        throw new System.Exception("Width Cant be Zero");
      additionalPlateParameters = PreSetDefaults(additionalPlateParameters);
      Point3d[] astPoints = PrepForByEdgeLength(line.StartPoint, line.EndPoint, normal, width);
      return new Plate(astPoints, Utils.ToAstVector3d(normal, true), additionalPlateParameters);
    }

    /// <summary>
    /// Create a rectangular Advance Steel Plate at 2 points for length with Width, including setting the side and thickness
    /// </summary>
    /// <param name="startPoint">Input Start Point of Length</param>
    /// <param name="endPoint">Input End Point of Length</param>
    /// <param name="normal">Input Plate Normal</param>
    /// <param name="width">Input Plate Width</param>
    /// <param name="additionalPlateParameters"> Optional Input Plate Build Properties </param>
    /// <returns name="plate"> plate</returns>
    public static Plate ByLengthEdge(Autodesk.DesignScript.Geometry.Point startPoint,
                                      Autodesk.DesignScript.Geometry.Point endPoint,
                                      Autodesk.DesignScript.Geometry.Vector normal,
                                      double width,
                                      [DefaultArgument("null")] List<Property> additionalPlateParameters)
    {
      double length = Utils.ToAstPoint(startPoint, true).DistanceTo(Utils.ToAstPoint(endPoint, true));
      if (length == 0)
        throw new System.Exception("Distance between 2 points Cant be Zero");
      if (width == 0)
        throw new System.Exception("Width Cant be Zero");
      additionalPlateParameters = PreSetDefaults(additionalPlateParameters);
      Point3d[] astPoints = PrepForByEdgeLength(startPoint, endPoint, normal, width);
      return new Plate(astPoints, Utils.ToAstVector3d(normal, true), additionalPlateParameters);
    }


    /// <summary>
    /// Create a rectangular Advance Steel Plate by using 2 points at opposite corners for extents, including setting the side and thickness
    /// </summary>
    /// <param name="cs">Input Dynamo Corrdinate System</param>
    /// <param name="cornerPoint1">Input Point of Lower Left of plate</param>
    /// <param name="cornerPoint2">Input Point of Upper right of plate</param>
    /// <param name="additionalPlateParameters"> Optional Input Plate Build Properties </param>
    /// <returns name="plate"> plate</returns>
    public static Plate ByTwoDiagonalPointsByCS(Autodesk.DesignScript.Geometry.CoordinateSystem cs,
                                                Autodesk.DesignScript.Geometry.Point cornerPoint1,
                                                Autodesk.DesignScript.Geometry.Point cornerPoint2,
                                                [DefaultArgument("null")] List<Property> additionalPlateParameters)
    {
      Point3d cpt1 = Utils.ToAstPoint(cornerPoint1, true);
      Point3d cpt2 = Utils.ToAstPoint(cornerPoint2, true);
      Plane ply1 = new Plane(cpt1, Utils.ToAstVector3d(cs.YAxis, true));
      Plane plx2 = new Plane(cpt2, Utils.ToAstVector3d(cs.XAxis, true));
      Point3d cpt1a = cpt1.OrthoProject(plx2);
      Point3d cpt2a = cpt2.OrthoProject(ply1);
      double width = cpt1a.DistanceTo(cpt1);
      if (width == 0)
        throw new System.Exception("Width Cant be Zero");
      double length = cpt2a.DistanceTo(cpt2);
      if (length == 0)
        throw new System.Exception("Length Cant be Zero");
      additionalPlateParameters = PreSetDefaults(additionalPlateParameters);
      Point3d[] astPoints = new Point3d[] { cpt1, cpt1a, cpt2, cpt2a };
      return new Plate(astPoints, Utils.ToAstVector3d(cs.ZAxis, true), additionalPlateParameters);
    }

    /// <summary>
    /// Create an Advance Steel Plate by 3 Points - Orgin, X Direction and Y Direction (Approx)
    /// </summary>
    /// <param name="orginPoint">Input the Origin of the Rectangular Plate</param>
    /// <param name="xDirectionPoint">Input Point in the X Direction - distance from orgin will determine the width</param>
    /// <param name="yDirectionPoint">Input Point in approximate Y Direction - True Y Direction will get recalculated</param>
    /// <param name="additionalPlateParameters"> Optional Input Plate Build Properties </param>
    /// <returns name="plate"> plate</returns>
    public static Plate ByThreePoints(Autodesk.DesignScript.Geometry.Point orginPoint,
                                      Autodesk.DesignScript.Geometry.Point xDirectionPoint,
                                      Autodesk.DesignScript.Geometry.Point yDirectionPoint,
                                      [DefaultArgument("null")] List<Property> additionalPlateParameters)
    {
      Point3d cpOrigin = Utils.ToAstPoint(orginPoint, true);
      Point3d xDPoint = Utils.ToAstPoint(xDirectionPoint, true);
      Point3d yDPoint = Utils.ToAstPoint(yDirectionPoint, true);

      Vector3d xAxis = xDPoint.Subtract(cpOrigin);
      Vector3d yAxis = yDPoint.Subtract(cpOrigin);

      Vector3d zAxis = xAxis.CrossProduct(yAxis);
      yAxis = xAxis.CrossProduct(zAxis);
      Plane yPlane = new Plane(yDPoint, yAxis);
      Plane xPlane = new Plane(xDPoint, xAxis);
      Point3d finalYPoint = cpOrigin.OrthoProject(yPlane);

      double width = cpOrigin.DistanceTo(xDPoint);
      if (width == 0)
        throw new System.Exception("Width Cant be Zero");
      double length = cpOrigin.DistanceTo(finalYPoint);
      if (length == 0)
        throw new System.Exception("Length Cant be Zero");
      Point3d pt3 = finalYPoint.OrthoProject(xPlane);
      additionalPlateParameters = PreSetDefaults(additionalPlateParameters);
      Point3d[] astPoints = new Point3d[] { cpOrigin, xDPoint, pt3, finalYPoint };
      return new Plate(astPoints, zAxis, additionalPlateParameters);
    }

    /// <summary>
    /// Get Plate Physical Length and Width
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <returns name="Length"> plate length and width values</returns>
    [MultiReturn(new[] { "Length", "Width" })]
    public static Dictionary<string, double> GetPhysicalLengthAndWidth(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      Dictionary<string, double> ret = new Dictionary<string, double>();
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
            {
              Autodesk.AdvanceSteel.Modelling.PlateBase selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.PlateBase;
              double length = 0;
              double width = 0;
              selectedObj.GetPhysLengthAndWidth(out length, out width);
              ret.Add("Length", Utils.FromInternalDistanceUnits(length, true));
              ret.Add("Width", Utils.FromInternalDistanceUnits(width, true));
            }
            else
              throw new System.Exception("Not a Plate Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object or Point is null");
      }
      return ret;
    }

    /// <summary>
    /// Get Plate Circumference
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <returns name="plateCircumference"> plate circumference value</returns>
    public static double GetCircumference(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      double ret = 0;
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
            {
              Autodesk.AdvanceSteel.Modelling.PlateBase selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.PlateBase;
              ret = (double)selectedObj.GetCircumference();
            }
            else
              throw new System.Exception("Not a Plate Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object or Point is null");
      }
      return Utils.FromInternalDistanceUnits(ret, true);
    }

    /// <summary>
    /// Is Plate Rectangular
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <returns name="IsRectangular"> reads if the plate is rectangular - true or false</returns>
    public static bool IsRectangular(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      bool ret = false;
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
            {
              Autodesk.AdvanceSteel.Modelling.PlateBase selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.PlateBase;
              ret = (bool)selectedObj.IsRectangular();
            }
            else
              throw new System.Exception("Not a Plate Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object or Point is null");
      }
      return ret;
    }

    private static List<Property> PreSetDefaults(List<Property> listPlateData)
    {
      if (listPlateData == null)
      {
        listPlateData = new List<Property>() { };
      }
      return listPlateData;
    }

    private static Point3d[] PrepForByEdgeLength(Autodesk.DesignScript.Geometry.Point startPoint,
                                                                              Autodesk.DesignScript.Geometry.Point endPoint,
                                                                              Autodesk.DesignScript.Geometry.Vector normal,
                                                                              double width)
    {
      Point3d originLine = Utils.ToAstPoint(startPoint, true);
      Point3d secPoint = Utils.ToAstPoint(endPoint, true);
      Vector3d linXvec = secPoint.Subtract(originLine);
      Vector3d linNormVec = Utils.ToAstVector3d(normal, true);
      Vector3d linYvec = linXvec.CrossProduct(linNormVec);
      Point3d midPoint = Utils.GetMidPointBetween(originLine, secPoint);
      double internWidth = (Utils.ToInternalDistanceUnits(width, true));
      Point3d finalOrigin = new Point3d(midPoint).Add(linYvec.Normalize() * (internWidth / 2));
      Point3d pt4 = new Point3d(originLine).Add(linYvec.Normalize() * internWidth);
      Point3d pt3 = new Point3d(secPoint).Add(linYvec.Normalize() * internWidth);
      return new Point3d[] { originLine, secPoint, pt3, pt4 };
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      var plate = Utils.GetObject(Handle) as ASPlate;

      Polyline3d astPoly = null;
      plate.GetBaseContourPolygon(0.0, out astPoly);

      var dynPoints = Utils.ToDynPoints(astPoly.Vertices, true);
      var poly = Autodesk.DesignScript.Geometry.Polygon.ByPoints(dynPoints, astPoly.IsClosed);
      foreach (var pt in dynPoints) { pt.Dispose(); }

      return poly;
    }

  }
}
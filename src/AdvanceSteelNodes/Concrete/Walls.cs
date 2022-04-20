using Autodesk.AdvanceSteel.CADAccess;
using SteelGeometry = Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using System.Collections.Generic;
using Autodesk.AdvanceSteel.CADLink.Database;
using Autodesk.AdvanceSteel.Geometry;
using System.Linq;
using ASWall = Autodesk.AdvanceSteel.Modelling.Wall;

namespace AdvanceSteel.Nodes.Concrete
{
  /// <summary>
  /// Advance Steel Walls
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class Walls : GraphicObject
  {
    private Walls(SteelGeometry.Point3d ptCenter,
                    double dLength, double dHeight, double thickness,
                    SteelGeometry.Vector3d vNormal,
                    List<Property> concreteProperties)
    {
      SafeInit(() => InitWalls(ptCenter, dLength, dHeight, thickness, vNormal, concreteProperties));
    }

    private Walls(SteelGeometry.Matrix3d matrix,
                    double dLength, double dHeight, double thickness,
                    List<Property> concreteProperties)
    {
      SafeInit(() => InitWalls(matrix, dLength, dHeight, thickness, concreteProperties));
    }

    private Walls(Autodesk.DesignScript.Geometry.Polygon poly,
                    double thickness,
                    List<Property> concreteProperties)
    {
      SafeInit(() => InitWalls(poly, thickness, concreteProperties));
    }

    private Walls(ASWall wall)
    {
      SafeInit(() => SetHandle(wall));
    }

    internal static Walls FromExisting(ASWall wall)
    {
      return new Walls(wall)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitWalls(SteelGeometry.Point3d ptCenter,
                    double dLength, double dHeight, double thickness,
                    SteelGeometry.Vector3d vNormal,
                    List<Property> concreteProperties)
    {
      List<Property> defaultData = concreteProperties.Where(x => x.Level == ".").ToList<Property>();
      List<Property> postWriteDBData = concreteProperties.Where(x => x.Level == "Z_PostWriteDB").ToList<Property>();

      SteelGeometry.Plane plane = new SteelGeometry.Plane(ptCenter, vNormal);

      ASWall wallObject = SteelServices.ElementBinder.GetObjectASFromTrace<ASWall>();
      if (wallObject == null)
      {
        wallObject = new ASWall(plane, ptCenter, dLength, dHeight);
        wallObject.Thickness = thickness;
        if (defaultData != null)
        {
          Utils.SetParameters(wallObject, defaultData);
        }

        wallObject.WriteToDb();
      }
      else
      {
        if (wallObject != null && wallObject.IsKindOf(FilerObject.eObjectType.kWall))
        {
          wallObject.DefinitionPlane = plane;
          wallObject.Thickness = thickness;
          wallObject.SetLength(dLength, true);
          wallObject.SetWidth(dHeight, true);

          if (defaultData != null)
          {
            Utils.SetParameters(wallObject, defaultData);
          }
        }
        else
        {
          throw new System.Exception("Not a Wall");
        }
      }

      SetHandle(wallObject);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(wallObject, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(wallObject);
    }

    private void InitWalls(SteelGeometry.Matrix3d matrix,
                    double dLength, double dHeight, double thickness,
                    List<Property> concreteProperties)
    {
      List<Property> defaultData = concreteProperties.Where(x => x.Level == ".").ToList<Property>();
      List<Property> postWriteDBData = concreteProperties.Where(x => x.Level == "Z_PostWriteDB").ToList<Property>();

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

      ASWall wallObject = SteelServices.ElementBinder.GetObjectASFromTrace<ASWall>();
      if (wallObject == null)
      {
        wallObject = new Autodesk.AdvanceSteel.Modelling.Wall(plane, wallPoints);
        wallObject.Thickness = thickness;
        Polyline3d outerPoly = new Polyline3d(wallPoints, cornerRadii, true, yAxis, false, 0, true, true);
        IEnumerable<ObjectId> deletedFeaturesIds = null;
        IEnumerable<ObjectId> newFeaturesIds = null;
        wallObject.SetOuterContour(outerPoly, out deletedFeaturesIds, out newFeaturesIds);

        if (defaultData != null)
        {
          Utils.SetParameters(wallObject, defaultData);
        }

        wallObject.WriteToDb();
      }
      else
      {
        if (wallObject != null && wallObject.IsKindOf(FilerObject.eObjectType.kWall))
        {
          //TODO - Missing SetPolygon
          wallObject.DefinitionPlane = plane;
          wallObject.Thickness = thickness;
          Polyline3d outerPoly = new Polyline3d(wallPoints, cornerRadii, true, yAxis, false, 0, true, true);
          IEnumerable<ObjectId> deletedFeaturesIds = null;
          IEnumerable<ObjectId> newFeaturesIds = null;

          if (defaultData != null)
          {
            Utils.SetParameters(wallObject, defaultData);
          }

          wallObject.SetOuterContour(outerPoly, out deletedFeaturesIds, out newFeaturesIds);
        }
        else
        {
          throw new System.Exception("Not a Wall");
        }
      }

      SetHandle(wallObject);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(wallObject, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(wallObject);
    }

    private void InitWalls(Autodesk.DesignScript.Geometry.Polygon poly,
                    double thickness,
                    List<Property> concreteProperties)
    {
      if (poly.IsPlanar == false)
        throw new System.Exception("Polygon is not planar");

      List<Property> defaultData = concreteProperties.Where(x => x.Level == ".").ToList<Property>();
      List<Property> postWriteDBData = concreteProperties.Where(x => x.Level == "Z_PostWriteDB").ToList<Property>();

      Point3d[] astPoints = Utils.ToAstPoints(poly.Points, true);
      double[] cornerRadii = (double[])System.Collections.ArrayList.Repeat(0.0, poly.Points.Length).ToArray(typeof(double));

      var astPoly = new Autodesk.AdvanceSteel.Geometry.Polyline3d(astPoints, null, poly.IsClosed, true);
      var polyPlane = new Plane(astPoints[0], astPoly.Normal);

      ASWall wallObject = SteelServices.ElementBinder.GetObjectASFromTrace<ASWall>();
      if (wallObject == null)
      {
        wallObject = new ASWall(polyPlane, astPoints);
        wallObject.Thickness = thickness;
        Polyline3d outerPoly = new Polyline3d(astPoints, cornerRadii, true, astPoly.Normal, false, 0, true, true);
        IEnumerable<ObjectId> deletedFeaturesIds = null;
        IEnumerable<ObjectId> newFeaturesIds = null;
        wallObject.SetOuterContour(outerPoly, out deletedFeaturesIds, out newFeaturesIds);

        if (defaultData != null)
        {
          Utils.SetParameters(wallObject, defaultData);
        }

        wallObject.WriteToDb();
      }
      else
      {
        if (wallObject != null && wallObject.IsKindOf(FilerObject.eObjectType.kWall))
        {
          wallObject.DefinitionPlane = polyPlane;
          wallObject.Thickness = thickness;
          Polyline3d outerPoly = new Polyline3d(astPoints, cornerRadii, true, astPoly.Normal, false, 0, true, true);
          IEnumerable<ObjectId> deletedFeaturesIds = null;
          IEnumerable<ObjectId> newFeaturesIds = null;

          if (defaultData != null)
          {
            Utils.SetParameters(wallObject, defaultData);
          }

          wallObject.SetOuterContour(outerPoly, out deletedFeaturesIds, out newFeaturesIds);
        }
        else
          throw new System.Exception("Not a Slab");
      }

      SetHandle(wallObject);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(wallObject, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(wallObject);
    }

    /// <summary>
    /// Create an Advance Steel Rectangular Wall by defining the extents of the face of the wall by Coordinate System
    /// </summary>
    /// <param name="coordinateSystem"> Input Dynamo Coordinate System</param>
    /// <param name="length"> Input Wall Length</param>
    /// <param name="height"> Input Wall Height</param>
    /// <param name="thickness"> Input Wall Thickness</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns name="walls"> walls</returns>
    public static Walls FaceByLengthHeightByCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                double length, double height, double thickness,
                                                [DefaultArgument("null")] List<Property> additionalConcParameters)
    {
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new Walls(Utils.ToAstPoint(coordinateSystem.Origin, true), Utils.ToInternalDistanceUnits(length, true), Utils.ToInternalDistanceUnits(height, true),
                                 Utils.ToInternalDistanceUnits(thickness, true), Utils.ToAstVector3d(coordinateSystem.ZAxis, true),
                                 additionalConcParameters);
    }

    /// <summary>
    /// Create an Advance Steel Rectangular Wall by defining the extents of the face of the wall by Dynamo Plane
    /// </summary>
    /// <param name="plane"> Input Dynamo Plane to insert Wall</param>
    /// <param name="length"> Input Wall Length</param>
    /// <param name="height"> Input Wall Height</param>
    /// <param name="thickness"> Input Wall Thickness</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns name="walls"> walls</returns>
    public static Walls FaceByLengthHeightByPlane(Autodesk.DesignScript.Geometry.Plane plane,
                                                  double length, double height, double thickness,
                                                  [DefaultArgument("null")] List<Property> additionalConcParameters)
    {
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new Walls(Utils.ToAstPoint(plane.Origin, true), Utils.ToInternalDistanceUnits(length, true), Utils.ToInternalDistanceUnits(height, true),
                                 Utils.ToInternalDistanceUnits(thickness, true), Utils.ToAstVector3d(plane.Normal, true),
                                 additionalConcParameters);
    }

    /// <summary>
    /// Create an Advance Steel Wall by Polygon and Thickness
    /// </summary>
    /// <param name="poly"> Input Dynamo Polygon for Wall shape</param>
    /// <param name="thickness"> Input Wall Thickness</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns name="walls"> walls</returns>
    public static Walls FaceByPolygon(Autodesk.DesignScript.Geometry.Polygon poly,
                                      double thickness,
                                      [DefaultArgument("null")] List<Property> additionalConcParameters)
    {
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new Walls(poly, Utils.ToInternalDistanceUnits(thickness, true), additionalConcParameters);
    }

    /// <summary>
    /// Creates an Advance Steel Wall at CS by Length and by Height - Z axis is assumed Wall creation
    /// </summary>
    /// <param name="coordinateSystem"> Input Dynamo Coordinate System</param>
    /// <param name="length"> Input Wall Length</param>
    /// <param name="height"> Input Wall Height</param>
    /// <param name="thickness"> Input Wall Thickness</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns name="walls"> walls</returns>
    public static Walls AtBaseByLengthHeightByCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                  double length,
                                                  double height,
                                                  double thickness,
                                                  [DefaultArgument("null")] List<Property> additionalConcParameters)
    {
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new Walls(Utils.ToAstMatrix3d(coordinateSystem, true), Utils.ToInternalDistanceUnits(length, true), Utils.ToInternalDistanceUnits(height, true),
                                 Utils.ToInternalDistanceUnits(thickness, true), additionalConcParameters);
    }

    private static List<Property> PreSetDefaults(List<Property> listOfProps)
    {
      if (listOfProps == null)
      {
        listOfProps = new List<Property>() { };
      }
      return listOfProps;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      var walls = Utils.GetObject(Handle) as ASWall;

      SteelGeometry.Polyline3d astPoly = null;
      walls.GetBaseContourPolygon(0.0, out astPoly);

      var dynPoints = Utils.ToDynPoints(astPoly.Vertices, true);
      var poly = Autodesk.DesignScript.Geometry.Polygon.ByPoints(dynPoints, astPoly.IsClosed);

      foreach (var pt in dynPoints) { pt.Dispose(); }

      return poly;
    }

  }
}
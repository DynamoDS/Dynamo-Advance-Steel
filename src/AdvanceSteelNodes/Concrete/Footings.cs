using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using ASFootingIsolated = Autodesk.AdvanceSteel.Modelling.FootingIsolated;

namespace AdvanceSteel.Nodes.Concrete
{
  /// <summary>
  /// Advance Steel Isloated Footing
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class Footings : GraphicObject
  {
    private Footings(Point3d ptCenter, Vector3d vNormal,
                      double depth, double radius,
                      List<Property> concreteProperties)
    {
      SafeInit(() => InitFootings(ptCenter, vNormal, depth, radius, concreteProperties));
    }

    private Footings(Point3d ptCenter, Vector3d vNormal,
                      double depth, double width, double length,
                      List<Property> concreteProperties)
    {
      SafeInit(() => InitFootings(ptCenter, vNormal, depth, width, length, concreteProperties));
    }

    private Footings(ASFootingIsolated padFooting)
    {
      SafeInit(() => SetHandle(padFooting));
    }

    internal static Footings FromExisting(ASFootingIsolated padFooting)
    {
      return new Footings(padFooting)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitFootings(Point3d ptCenter, Vector3d vNormal,
                      double depth, double radius,
                      List<Property> concreteProperties)
    {
      List<Property> defaultData = concreteProperties.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> postWriteDBData = concreteProperties.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();

      Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(ptCenter, vNormal);

      ASFootingIsolated padFooting = SteelServices.ElementBinder.GetObjectASFromTrace<ASFootingIsolated>();
      if (padFooting == null)
      {
        padFooting = new ASFootingIsolated(plane, ptCenter, radius);
        padFooting.Thickness = depth;
        if (defaultData != null)
        {
          Utils.SetParameters(padFooting, defaultData);
        }

        padFooting.WriteToDb();
      }
      else
      {
        if (!padFooting.IsKindOf(FilerObject.eObjectType.kFootingIsolated))
          throw new System.Exception("Not an Isolated Footing");

        padFooting.DefinitionPlane = plane;
        padFooting.Thickness = depth;
        padFooting.setRadius(radius, true);

        if (defaultData != null)
        {
          Utils.SetParameters(padFooting, defaultData);
        }
      }

      SetHandle(padFooting);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(padFooting, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(padFooting);
    }

    private void InitFootings(Point3d ptCenter, Vector3d vNormal,
                      double depth, double width, double length,
                      List<Property> concreteProperties)
    {
      List<Property> defaultData = concreteProperties.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> postWriteDBData = concreteProperties.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();

      Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(ptCenter, vNormal);

      ASFootingIsolated padFooting = SteelServices.ElementBinder.GetObjectASFromTrace<ASFootingIsolated>();
      if (padFooting == null)
      {
        padFooting = new ASFootingIsolated(plane, ptCenter, width, length);
        padFooting.SetLength(length, false);
        padFooting.SetWidth(width, false);
        padFooting.Thickness = depth;

        if (defaultData != null)
        {
          Utils.SetParameters(padFooting, defaultData);
        }

        padFooting.WriteToDb();
      }
      else
      {
        if (!padFooting.IsKindOf(FilerObject.eObjectType.kFootingIsolated))
          throw new System.Exception("Not an Isolated Footing");

        padFooting.DefinitionPlane = plane;
        padFooting.Thickness = depth;
        padFooting.SetLength(length, false);
        padFooting.SetWidth(width, false);

        if (defaultData != null)
        {
          Utils.SetParameters(padFooting, defaultData);
        }
      }

      SetHandle(padFooting);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(padFooting, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(padFooting);
    }

    /// <summary>
    /// Create an Advance Steel Isolated Footing - Circular
    /// </summary>
    /// <param name="coordinateSystem"> Input Dynamo Coordinate System to Input location of footing</param>
    /// <param name="footingDepth"> Input Depth of Footing</param>
    /// <param name="footingRadius"> Input Footing Radius</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns name="footings"> footings</returns>
    public static Footings ByRadiusOnCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                        double footingDepth, double footingRadius,
                                        [DefaultArgument("null")] List<Property> additionalConcParameters)
    {
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new Footings(Utils.ToAstPoint(coordinateSystem.Origin, true),
                          Utils.ToAstVector3d(coordinateSystem.ZAxis, true),
                          footingDepth, footingRadius,
                          additionalConcParameters);
    }

    /// <summary>
    /// Create an Advance Steel Isolated Footing - Rectangular
    /// </summary>
    /// <param name="coordinateSystem"> Input Dynamo Coordinate System to Input location of footing</param>
    /// <param name="footingDepth"> Input Depth of Footing</param>
    /// <param name="footingWidth"> Input Width of Footing</param>
    /// <param name="footingLength"> Input Length of Footing</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns name="footings"> footings</returns>
    public static Footings ByLengthWidthOnCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                              double footingDepth, double footingWidth, double footingLength,
                                              [DefaultArgument("null")] List<Property> additionalConcParameters)
    {
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new Footings(Utils.ToAstPoint(coordinateSystem.Origin, true),
                          Utils.ToAstVector3d(coordinateSystem.ZAxis, true),
                          footingDepth, footingWidth, footingLength,
                          additionalConcParameters);
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
      var padFooting = Utils.GetObject(Handle) as ASFootingIsolated;

      Polyline3d astPoly = null;
      padFooting.GetBaseContourPolygon(0.0, out astPoly);

      var dynPoints = Utils.ToDynPoints(astPoly.Vertices, true);
      var poly = Autodesk.DesignScript.Geometry.Polygon.ByPoints(dynPoints, astPoly.IsClosed);

      foreach (var pt in dynPoints) { pt.Dispose(); }

      return poly;
    }

  }
}
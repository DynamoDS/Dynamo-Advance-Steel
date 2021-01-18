using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Concrete
{
  /// <summary>
  /// Advance Steel Isloated Footing
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class Footings : GraphicObject
  {
    internal Footings()
    {
    }

    internal Footings(Point3d ptCenter, Vector3d vNormal,
                      double depth, double radius,
                      List<ASProperty> concreteProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          List<ASProperty> defaultData = concreteProperties.Where(x => x.PropLevel == ".").ToList<ASProperty>();
          List<ASProperty> postWriteDBData = concreteProperties.Where(x => x.PropLevel == "Z_PostWriteDB").ToList<ASProperty>();

          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(ptCenter, vNormal);

          Autodesk.AdvanceSteel.Modelling.FootingIsolated padFooting = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            padFooting = new Autodesk.AdvanceSteel.Modelling.FootingIsolated(plane, ptCenter, radius);
            padFooting.Thickness = depth;
            if (defaultData != null)
            {
              Utils.SetParameters(padFooting, defaultData);
            }

            padFooting.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(padFooting, postWriteDBData);
            }
          }
          else
          {
            padFooting = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.FootingIsolated;

            if (padFooting != null && padFooting.IsKindOf(FilerObject.eObjectType.kFootingIsolated))
            {
              padFooting.DefinitionPlane = plane;
              padFooting.Thickness = depth;
              padFooting.setRadius(radius, true);

              if (defaultData != null)
              {
                Utils.SetParameters(padFooting, defaultData);
              }

              if (postWriteDBData != null)
              {
                Utils.SetParameters(padFooting, postWriteDBData);
              }
            }
            else
              throw new System.Exception("Not an Isolated Footing");
          }

          Handle = padFooting.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(padFooting);
        }
      }
    }

    internal Footings(Point3d ptCenter, Vector3d vNormal,
                      double depth, double width, double length,
                      List<ASProperty> concreteProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          List<ASProperty> defaultData = concreteProperties.Where(x => x.PropLevel == ".").ToList<ASProperty>();
          List<ASProperty> postWriteDBData = concreteProperties.Where(x => x.PropLevel == "Z_PostWriteDB").ToList<ASProperty>();

          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(ptCenter, vNormal);

          Autodesk.AdvanceSteel.Modelling.FootingIsolated padFooting = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            padFooting = new Autodesk.AdvanceSteel.Modelling.FootingIsolated(plane, ptCenter, width, length);
            padFooting.SetLength(length, false);
            padFooting.SetWidth(width, false);
            padFooting.Thickness = depth;

            if (defaultData != null)
            {
              Utils.SetParameters(padFooting, defaultData);
            }

            padFooting.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(padFooting, postWriteDBData);
            }
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

              if (defaultData != null)
              {
                Utils.SetParameters(padFooting, defaultData);
              }

              if (postWriteDBData != null)
              {
                Utils.SetParameters(padFooting, postWriteDBData);
              }
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
    /// <param name="coordinateSystem"> Input Dynamo Coordinate System to Input location of footing</param>
    /// <param name="footingDepth"> Input Depth of Footing</param>
    /// <param name="footingRadius"> Input Footing Radius</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns></returns>
    public static Footings ByRadiusOnCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                        double footingDepth, double footingRadius,
                                        [DefaultArgument("null")] List<ASProperty> additionalConcParameters)
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
    /// <returns></returns>
    public static Footings ByLengthWidthOnCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                              double footingDepth, double footingWidth, double footingLength,
                                              [DefaultArgument("null")] List<ASProperty> additionalConcParameters)
    {
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new Footings(Utils.ToAstPoint(coordinateSystem.Origin, true),
                          Utils.ToAstVector3d(coordinateSystem.ZAxis, true),
                          footingDepth, footingWidth, footingLength,
                          additionalConcParameters);
    }

    private static List<ASProperty> PreSetDefaults(List<ASProperty> listOfProps)
    {
      if (listOfProps == null)
      {
        listOfProps = new List<ASProperty>() { };
      }
      return listOfProps;
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
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.CADLink.Database;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Concrete
{
  /// <summary>
  /// Advance Steel Slabs
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class Slabs : GraphicObject
  {
    internal Slabs()
    {
    }

    internal Slabs(Autodesk.DesignScript.Geometry.Polygon poly,
                    double thickness,
                    List<Property> concreteProperties)
    {
      if (poly.IsPlanar == false)
        throw new System.Exception("Polygon is not planar");

      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          List<Property> defaultData = concreteProperties.Where(x => x.Level == ".").ToList<Property>();
          List<Property> postWriteDBData = concreteProperties.Where(x => x.Level == "Z_PostWriteDB").ToList<Property>();

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
            if (defaultData != null)
            {
              Utils.SetParameters(floorSlab, defaultData);
            }

            floorSlab.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(floorSlab, postWriteDBData);
            }
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

              if (defaultData != null)
              {
                Utils.SetParameters(floorSlab, defaultData);
              }

              floorSlab.SetOuterContour(outerPoly, out deletedFeaturesIds, out newFeaturesIds);

              if (postWriteDBData != null)
              {
                Utils.SetParameters(floorSlab, postWriteDBData);
              }
            }
            else
              throw new System.Exception("Not a Slab");
          }

          Handle = floorSlab.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(floorSlab);
        }
      }
    }

    internal Slabs(Point3d ptCenter,
                    double dWidth, double dLength, double thickness,
                    Vector3d vNormal,
                    List<Property> concreteProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          List<Property> defaultData = concreteProperties.Where(x => x.Level == ".").ToList<Property>();
          List<Property> postWriteDBData = concreteProperties.Where(x => x.Level == "Z_PostWriteDB").ToList<Property>();

          Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(ptCenter, vNormal);
          Autodesk.AdvanceSteel.Modelling.Slab floorSlab = null;
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            floorSlab = new Autodesk.AdvanceSteel.Modelling.Slab(plane, ptCenter, dWidth, dLength);
            floorSlab.SetLength(dWidth, true);
            floorSlab.SetWidth(dLength, true);
            floorSlab.Thickness = thickness;
            if (defaultData != null)
            {
              Utils.SetParameters(floorSlab, defaultData);
            }

            floorSlab.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(floorSlab, postWriteDBData);
            }
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

              if (defaultData != null)
              {
                Utils.SetParameters(floorSlab, defaultData);
              }

              if (postWriteDBData != null)
              {
                Utils.SetParameters(floorSlab, postWriteDBData);
              }

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

    internal Slabs(Point3d ptCenter,
                    double dRadius, double thickness,
                    Vector3d vNormal,
                    List<Property> concreteProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          List<Property> defaultData = concreteProperties.Where(x => x.Level == ".").ToList<Property>();
          List<Property> postWriteDBData = concreteProperties.Where(x => x.Level == "Z_PostWriteDB").ToList<Property>();

          Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(ptCenter, vNormal);
          Autodesk.AdvanceSteel.Modelling.Slab floorSlab = null;
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            floorSlab = new Autodesk.AdvanceSteel.Modelling.Slab(plane, ptCenter, dRadius, 0);
            floorSlab.setRadius(dRadius, true); //Not Working
            floorSlab.Thickness = thickness;
            if (defaultData != null)
            {
              Utils.SetParameters(floorSlab, defaultData);
            }

            floorSlab.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(floorSlab, postWriteDBData);
            }
          }
          else
          {
            floorSlab = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Slab;
            if (floorSlab != null && floorSlab.IsKindOf(FilerObject.eObjectType.kSlab))
            {
              floorSlab.DefinitionPlane = plane;
              floorSlab.setRadius(dRadius, true); //Not Working
              floorSlab.Thickness = thickness;

              if (defaultData != null)
              {
                Utils.SetParameters(floorSlab, defaultData);
              }

              if (postWriteDBData != null)
              {
                Utils.SetParameters(floorSlab, postWriteDBData);
              }

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
    /// <param name="poly"> Input Dynamo Polygon</param>
    /// <param name="thickness"> Slab Thickness in Current Model Length Unit Settings</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns></returns>
    public static Slabs ByPolygon(Autodesk.DesignScript.Geometry.Polygon poly,
                                  double thickness,
                                  [DefaultArgument("null")] List<Property> additionalConcParameters)
    {
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new Slabs(poly, thickness, additionalConcParameters);
    }

    /// <summary>
    /// Create an Advance Steel Rectangular Slab By Thickness
    /// </summary>
    /// <param name="coordinateSystem"> Input Dynamo Coordinate System</param>
    /// <param name="width"> Slab Width in Current Model Advance Steel Length Unit Settings</param>
    /// <param name="length"> Slab Length in Current Model Advance Steel Length Unit Settings</param>
    /// <param name="thickness"> Slab Thickness in Current Model Length Unit Settings</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns></returns>
    public static Slabs ByRectangularByCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                          double width, double length, double thickness,
                                          [DefaultArgument("null")] List<Property> additionalConcParameters)
    {
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new Slabs(Utils.ToAstPoint(coordinateSystem.Origin, true),
                                  Utils.ToInternalUnits(width, true),
                                  Utils.ToInternalUnits(length, true),
                                  Utils.ToInternalUnits(thickness, true),
                                  Utils.ToAstVector3d(coordinateSystem.ZAxis, true),
                                  additionalConcParameters);
    }

    /// <summary>
    /// Create an Advance Steel Radial Slab By Thickness and Radius
    /// </summary>
    /// <param name="coordinateSystem"> Input Dynamo Coordinate System</param>
    /// <param name="radius"> Slab Radius in Current Model Length Unit Settings</param>
    /// <param name="thickness"> Slab Thickness in Current Model Length Unit Settings</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns></returns>
    public static Slabs ByCircularByCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                        double radius, double thickness,
                                        [DefaultArgument("null")] List<Property> additionalConcParameters)
    {
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new Slabs(Utils.ToAstPoint(coordinateSystem.Origin, true),
                        Utils.ToInternalUnits(radius, true),
                        Utils.ToInternalUnits(thickness, true),
                        Utils.ToAstVector3d(coordinateSystem.ZAxis, true),
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
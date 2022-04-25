using AdvanceSteel.Nodes.Beams;
using AdvanceSteel.Nodes.Concrete;
using AdvanceSteel.Nodes.Gratings;
using AdvanceSteel.Nodes.Plates;
using AdvanceSteel.Nodes.ConnectionObjects.Anchors;
using AdvanceSteel.Nodes.ConnectionObjects.Bolts;
using AdvanceSteel.Nodes.ConnectionObjects.ShearStuds;
using AdvanceSteel.Nodes.ConnectionObjects.Welds;
using AdvanceSteel.Nodes.Features;
using AdvanceSteel.Nodes.Miscellaneous;
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using Dynamo.Applications.AdvanceSteel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AdvanceSteel.CADLink.Database;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;
using static Autodesk.AdvanceSteel.DotNetRoots.Units.Unit;

namespace AdvanceSteel.Nodes
{
  [IsVisibleInDynamoLibrary(false)]
  public static class Utils
  {
    private static readonly string separator = "#@§@#";
   
    private static readonly Dictionary<Autodesk.AdvanceSteel.CADAccess.FilerObject.eObjectType, string> filterSteelObjects = new Dictionary<Autodesk.AdvanceSteel.CADAccess.FilerObject.eObjectType, string>()
    {
      { FilerObject.eObjectType.kUnknown , "Select Object Type..." },
      { FilerObject.eObjectType.kAnchorPattern, "Anchor Pattern" },
      { FilerObject.eObjectType.kBeamNotch2Ortho, "Beam Cope" },
      { FilerObject.eObjectType.kBeamNotchEx, "Beam Cope Rotated" },
      { FilerObject.eObjectType.kBentBeam, "Bent Beam" },
      { FilerObject.eObjectType.kBeamMultiContourNotch, "Beam Polycut" },
      { FilerObject.eObjectType.kBeamShortening, "Beam Shortening" },
      { FilerObject.eObjectType.kCamera, "Camera" },
      { FilerObject.eObjectType.kGrid, "Grid" },
      { FilerObject.eObjectType.kCircleScrewBoltPattern, "Circular Bolt Pattern" },
      { FilerObject.eObjectType.kCompoundStraightBeam, "Compound Beam" },
      { FilerObject.eObjectType.kConcreteBentBeam, "Concrete Bent Beam" },
      { FilerObject.eObjectType.kConcreteBeam, "Concrete Striaght Beam" },
      { FilerObject.eObjectType.kGrating, "Grating" },
      { FilerObject.eObjectType.kFootingIsolated, "Isolated Footings" },
      { FilerObject.eObjectType.kPlate, "Plate" },
      { FilerObject.eObjectType.kPolyBeam, "Poly Beam" },
      { FilerObject.eObjectType.kPlateFeatContour, "Plate Polycut" },
      { FilerObject.eObjectType.kPlateFeatVertFillet, "Plate Corner Cut" },
      { FilerObject.eObjectType.kInfinitMidScrewBoltPattern, "Rectangular Bolt Pattern" },
      { FilerObject.eObjectType.kStraightBeam, "Straight Beam" },
      { FilerObject.eObjectType.kSpecialPart, "Special Part" },
      { FilerObject.eObjectType.kSlab, "Slabs" },
      { FilerObject.eObjectType.kConnector, "Shear Studs" },
      { FilerObject.eObjectType.kBeamTapered, "Tapered Beam" },
      { FilerObject.eObjectType.kUnfoldedStraightBeam, "Unfolded Straight Beam" },
      { FilerObject.eObjectType.kWall, "Wals" },
      { FilerObject.eObjectType.kWeldStraight, "Weld Line" },
      { FilerObject.eObjectType.kWeldLevel, "Weld Point" },
      { FilerObject.eObjectType.kConnectionHolePlate, "Connection Holes in Plate" }
    };

    private static readonly Dictionary<Autodesk.AdvanceSteel.CADAccess.FilerObject.eObjectType, Dictionary<string, Property>> steelObjectPropertySets = new Dictionary<Autodesk.AdvanceSteel.CADAccess.FilerObject.eObjectType, Dictionary<string, Property>>()
    {
      { FilerObject.eObjectType.kAnchorPattern, Build_Master_AnchorPattern() },
      { FilerObject.eObjectType.kBeamNotch2Ortho, Build_Master_BeamNotch2Ortho() },
      { FilerObject.eObjectType.kBeamNotchEx, Build_Master_BeamNotchEx() },
      { FilerObject.eObjectType.kBentBeam, Build_Master_BentBeam() },
      { FilerObject.eObjectType.kBeamMultiContourNotch, Build_Master_BeamMultiContourNotch() },
      { FilerObject.eObjectType.kBeamShortening, Build_Master_BeamShortening() },
      { FilerObject.eObjectType.kCamera, Build_Master_Camera() },
      { FilerObject.eObjectType.kGrid, Build_Master_Grid() },
      { FilerObject.eObjectType.kCircleScrewBoltPattern, Build_Master_CircleScrewBoltPattern() },
      { FilerObject.eObjectType.kCompoundStraightBeam, Build_Master_CompoundStraightBeam() },
      { FilerObject.eObjectType.kConcreteBentBeam, Build_Master_ConcreteBentBeam() },
      { FilerObject.eObjectType.kConcreteBeam, Build_Master_ConcreteBeam() },
      { FilerObject.eObjectType.kGrating, Build_Master_Grating() },
      { FilerObject.eObjectType.kFootingIsolated, Build_Master_FootingIsolated()},
      { FilerObject.eObjectType.kPlate, Build_Master_Plate() },
      { FilerObject.eObjectType.kPolyBeam, Build_Master_PolyBeam() },
      { FilerObject.eObjectType.kPlateFeatContour, Build_Master_PlateFeatContour() },
      { FilerObject.eObjectType.kPlateFeatVertFillet, Build_Master_PlateFeatVertFillet() },
      { FilerObject.eObjectType.kInfinitMidScrewBoltPattern, Build_Master_InfinitMidScrewBoltPattern() },
      { FilerObject.eObjectType.kStraightBeam, Build_Master_StraightBeam() },
      { FilerObject.eObjectType.kSpecialPart, Build_Master_SpecialPart() },
      { FilerObject.eObjectType.kSlab, Build_Master_Slab() },
      { FilerObject.eObjectType.kConnector, Build_Master_Connector() },
      { FilerObject.eObjectType.kBeamTapered, Build_Master_BeamTapered() },
      { FilerObject.eObjectType.kUnfoldedStraightBeam, Build_Master_UnfoldedStraightBeam() },
      { FilerObject.eObjectType.kWall, Build_Master_Wall() },
      { FilerObject.eObjectType.kWeldStraight, Build_Master_WeldLine() },
      { FilerObject.eObjectType.kWeldLevel, Build_Master_WeldPoint() },
      { FilerObject.eObjectType.kConnectionHolePlate, Build_Master_ConnectionHolePlate() }
    };

    public static double RadToDegree(double rad)
    {
      return 180.0 * rad / System.Math.PI;
    }

    public static double DegreeToRad(double deg)
    {
      return (System.Math.PI / 180.0) * deg;
    }

    static public Autodesk.AdvanceSteel.Geometry.Point3d GetMidPointBetween(this Point3d OriginPoint, Point3d SecondPoint)
    {
      Autodesk.AdvanceSteel.Geometry.Point3d returnPnt = new Autodesk.AdvanceSteel.Geometry.Point3d();
      Autodesk.AdvanceSteel.Geometry.Vector3d v = SecondPoint.Subtract(OriginPoint);
      v.Normalize();
      returnPnt = OriginPoint + ((OriginPoint.DistanceTo(SecondPoint) / 2) * v);
      return returnPnt;
    }

    static public Autodesk.AdvanceSteel.Geometry.Point3d GetMidPointOnArc(this Point3d startPointOnArc, Point3d endPointOnArc, Point3d arcCentrePoint)
    {
      Autodesk.AdvanceSteel.Geometry.Point3d returnPnt = new Autodesk.AdvanceSteel.Geometry.Point3d();
      double radius = arcCentrePoint.DistanceTo(startPointOnArc);
      Autodesk.AdvanceSteel.Geometry.Point3d biSectPoint = startPointOnArc.GetMidPointBetween(endPointOnArc);
      Autodesk.AdvanceSteel.Geometry.Vector3d v = biSectPoint.Subtract(arcCentrePoint);
      v.Normalize();
      returnPnt = startPointOnArc + (radius * v);
      return returnPnt;
    }

    static public Autodesk.AdvanceSteel.Geometry.Point3d ToAstPoint(Autodesk.DesignScript.Geometry.Point pt, bool bConvertToAstUnits)
    {
      double factor = 1.0;
      if (bConvertToAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }

      return new Autodesk.AdvanceSteel.Geometry.Point3d(pt.X, pt.Y, pt.Z) * factor;
    }

    static public Double ToInternalUnits(double value, eUnitType unitType, bool bConvert)
    {
      double factor = 1.0;
      if (bConvert)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.GetUnit(unitType).Factor;
      }

      return (value * factor);
    }

    static public Double FromInternalUnits(double value, eUnitType unitType, bool bConvert)
    {
      double factor = 1.0;
      if (bConvert)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.GetUnit(unitType).Factor;
      }

      return (value * (1 / factor));
    }

    static public Double ToInternalDistanceUnits(double value, bool bConvert)
    {
      double factor = 1.0;
      if (bConvert)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }

      return (value * factor);
    }

    static public Double FromInternalDistanceUnits(double value, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }
      return (value * (1 / factor));
    }

    static public Double ToInternalAngleUnits(double value, bool bConvert)
    {
      double factor = 1.0;
      if (bConvert)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfAngle.Factor;
      }

      return (value * factor);
    }

    static public Double FromInternalAngleUnits(double value, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfAngle.Factor;
      }
      return (value * (1 / factor));
    }

    static public Double ToInternalAreaUnits(double value, bool bConvert)
    {
      double factor = 1.0;
      if (bConvert)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfArea.Factor;
      }

      return (value * factor);
    }

    static public Double FromInternalAreaUnits(double value, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfArea.Factor;
      }
      return (value * (1 / factor));
    }

    static public Double ToInternalAreaPerDistanceUnits(double value, bool bConvert)
    {
      double factor = 1.0;
      if (bConvert)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfAreaPerDistance.Factor;
      }

      return (value * factor);
    }

    static public Double FromInternalAreaPerDistanceUnits(double value, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfAreaPerDistance.Factor;
      }
      return (value * (1 / factor));
    }

    static public Double ToInternalWeightUnits(double value, bool bConvert)
    {
      double factor = 1.0;
      if (bConvert)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfWeight.Factor;
      }

      return (value * factor);
    }

    static public Double FromInternalWeightUnits(double value, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfWeight.Factor;
      }
      return (value * (1 / factor));
    }

    static public Double ToInternalWeightPerDistanceUnits(double value, bool bConvert)
    {
      double factor = 1.0;
      if (bConvert)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfWeightPerDistance.Factor;
      }

      return (value * factor);
    }

    static public Double FromInternalWeightPerDistanceUnits(double value, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfWeightPerDistance.Factor;
      }
      return (value * (1 / factor));
    }

    static public List<Autodesk.DesignScript.Geometry.Curve> ToDynPolyCurves(Autodesk.AdvanceSteel.Geometry.Polyline3d poly, bool bConvertFromAstUnits)
    {
      List<Autodesk.DesignScript.Geometry.Curve> retData = new List<Autodesk.DesignScript.Geometry.Curve>();
      Curve3d[] foundPolyCurves;
      poly.GetCurves(out foundPolyCurves);
      for (int i = 0; i < foundPolyCurves.Length; i++)
      {
        Curve3d nextCurve;
        nextCurve = foundPolyCurves[i];
        LineSeg3d line = nextCurve as LineSeg3d;
        if (line != null)
        {
          Point3d lStartPoint;
          Point3d lEndPoint;
          line.HasStartPoint(out lStartPoint);
          line.HasEndPoint(out lEndPoint);

          retData.Add(Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(Utils.ToDynPoint(lStartPoint, true),
                                                                                Utils.ToDynPoint(lEndPoint, true)));
        }
        CircArc3d arc = nextCurve as CircArc3d;
        if (line != null)
        {
          Point3d aStartPoint = arc.StartPoint;
          Point3d aEndPoint = arc.EndPoint;
          Point3d aCentrePoint = arc.Center;
          Vector3d arcNormal = arc.Normal;

          retData.Add(Autodesk.DesignScript.Geometry.Arc.ByCenterPointStartPointEndPoint(Utils.ToDynPoint(aCentrePoint, true),
                                                                                         Utils.ToDynPoint(aStartPoint, true),
                                                                                         Utils.ToDynPoint(aEndPoint, true)));
        }
      }
      return retData;
    }

    static public Autodesk.AdvanceSteel.Geometry.Polyline3d ToAstPolyline3d(Autodesk.DesignScript.Geometry.PolyCurve poly, bool bConvertToAstUnits)
    {
      Polyline3d newReturnPoly = new Polyline3d();

      if (poly.IsClosed == true)
      {
        Autodesk.DesignScript.Geometry.Curve[] curves = poly.Curves();
        Point3d[] PolyPoint = new Point3d[curves.Length];
        VertexInfo[] PolyVertexs = new VertexInfo[curves.Length];

        for (int i = 0; i < curves.Length; i++)
        {
          Autodesk.DesignScript.Geometry.Curve nextCurve = curves[i];

          Autodesk.DesignScript.Geometry.Vector startNormal = nextCurve.NormalAtParameter(0);
          Autodesk.DesignScript.Geometry.Vector midNormal = nextCurve.NormalAtParameter(nextCurve.Length / 2);
          Autodesk.DesignScript.Geometry.Vector endNormal = nextCurve.NormalAtParameter(nextCurve.Length);


          if (startNormal.IsParallel(endNormal)) //Line
          {
            PolyPoint[i] = Utils.ToAstPoint(nextCurve.StartPoint, bConvertToAstUnits);
            PolyVertexs[i] = new VertexInfo();
          }
          else //Other Curve
          {
            double angleStart = Utils.ToInternalAngleUnits(startNormal.AngleWithVector(midNormal), bConvertToAstUnits);
            double angleEnd = Utils.ToInternalAngleUnits(midNormal.AngleWithVector(endNormal), bConvertToAstUnits);
            angleStart = Math.Round(angleStart, 5);
            angleEnd = Math.Round(angleEnd, 5);

            if (angleStart == angleEnd)
            {
              Autodesk.DesignScript.Geometry.Point midPointOnCurve = nextCurve.PointAtSegmentLength(nextCurve.Length / 2);
              Autodesk.DesignScript.Geometry.Arc tempArc = Autodesk.DesignScript.Geometry.Arc.ByThreePoints(nextCurve.StartPoint,
                                                                                                            midPointOnCurve,
                                                                                                            nextCurve.EndPoint);

              PolyPoint[i] = Utils.ToAstPoint(tempArc.StartPoint, bConvertToAstUnits);
              PolyVertexs[i] = new VertexInfo(Utils.FromInternalDistanceUnits(tempArc.Radius, bConvertToAstUnits),
                                    Utils.ToAstPoint(tempArc.CenterPoint, bConvertToAstUnits),
                                    Utils.ToAstVector3d(tempArc.Normal, bConvertToAstUnits));
            }
            else
            {
              throw new System.Exception("Curve is not made of Lines and Arc");
            }
          }
        }
        newReturnPoly.CreateFrom(PolyPoint, PolyVertexs, true, true);
      }

      if (newReturnPoly.IsClosed != true)
      {
        throw new System.Exception("Not a Valid PolyLine3D");
      }
      return newReturnPoly;
    }

    static public Autodesk.AdvanceSteel.Geometry.Polyline3d ToAstPolyline3d(List<Autodesk.DesignScript.Geometry.Curve> curves, bool bConvertToAstUnits)
    {
      Polyline3d newReturnPoly = new Polyline3d();
      Point3d[] PolyPoint = new Point3d[curves.Count];
      VertexInfo[] PolyVertexs = new VertexInfo[curves.Count];
      for (int i = 0; i < curves.Count; i++)
      {
        Autodesk.DesignScript.Geometry.Curve nextCurve = curves[i];
        Autodesk.DesignScript.Geometry.Line line = nextCurve as Autodesk.DesignScript.Geometry.Line;
        if (line != null)
        {
          PolyPoint[i] = Utils.ToAstPoint(line.StartPoint, bConvertToAstUnits);
          PolyVertexs[i] = new VertexInfo();
        }
        Autodesk.DesignScript.Geometry.Arc arc = nextCurve as Autodesk.DesignScript.Geometry.Arc;
        if (arc != null)
        {
          PolyPoint[i] = Utils.ToAstPoint(arc.StartPoint, bConvertToAstUnits);
          PolyVertexs[i] = new VertexInfo(Utils.FromInternalDistanceUnits(arc.Radius, bConvertToAstUnits),
                                Utils.ToAstPoint(arc.CenterPoint, bConvertToAstUnits),
                                Utils.ToAstVector3d(arc.Normal, bConvertToAstUnits));
        }

      }

      newReturnPoly.CreateFrom(PolyPoint, PolyVertexs, true, true);
      if (newReturnPoly.IsClosed != true)
      {
        throw new System.Exception("Not a Valid PolyLine3D");
      }
      return newReturnPoly;
    }

    static public Autodesk.DesignScript.Geometry.Point ToDynPoint(Autodesk.AdvanceSteel.Geometry.Point3d pt, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }
      pt = pt * (1 / factor);
      return Autodesk.DesignScript.Geometry.Point.ByCoordinates(pt.x, pt.y, pt.z);
    }

    static public Autodesk.DesignScript.Geometry.Vector ToDynVector(Autodesk.AdvanceSteel.Geometry.Vector3d vect, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }
      vect = vect * (1 / factor);
      return Autodesk.DesignScript.Geometry.Vector.ByCoordinates(vect.x, vect.y, vect.z);
    }

    static public Autodesk.DesignScript.Geometry.CoordinateSystem ToDynCoordinateSys(Autodesk.AdvanceSteel.Geometry.Matrix3d matrix, bool bConvertToAstUnits)
    {
      Autodesk.AdvanceSteel.Geometry.Point3d origin = new Point3d();
      Autodesk.AdvanceSteel.Geometry.Vector3d xAxis = new Autodesk.AdvanceSteel.Geometry.Vector3d();
      Autodesk.AdvanceSteel.Geometry.Vector3d yAxis = new Autodesk.AdvanceSteel.Geometry.Vector3d();
      Autodesk.AdvanceSteel.Geometry.Vector3d zAxis = new Autodesk.AdvanceSteel.Geometry.Vector3d();
      matrix.GetCoordSystem(out origin, out xAxis, out yAxis, out zAxis);

      return Autodesk.DesignScript.Geometry.CoordinateSystem.ByOriginVectors(ToDynPoint(origin, bConvertToAstUnits), ToDynVector(xAxis, bConvertToAstUnits),
        ToDynVector(yAxis, bConvertToAstUnits), ToDynVector(zAxis, bConvertToAstUnits));
    }

    static public Autodesk.AdvanceSteel.Geometry.Vector3d ToAstVector3d(Autodesk.DesignScript.Geometry.Vector v, bool bConvertToAstUnits)
    {
      double factor = 1.0;
      if (bConvertToAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }
      return new Autodesk.AdvanceSteel.Geometry.Vector3d(v.X, v.Y, v.Z) * factor;
    }

    static public Autodesk.AdvanceSteel.Geometry.Matrix3d ToAstMatrix3d(Autodesk.DesignScript.Geometry.CoordinateSystem cs, bool bConvertToAstUnits)
    {
      Autodesk.AdvanceSteel.Geometry.Matrix3d matrix = new Autodesk.AdvanceSteel.Geometry.Matrix3d();
      matrix.SetCoordSystem(ToAstPoint(cs.Origin, bConvertToAstUnits), ToAstVector3d(cs.XAxis, bConvertToAstUnits), ToAstVector3d(cs.YAxis, bConvertToAstUnits), ToAstVector3d(cs.ZAxis, bConvertToAstUnits));
      return matrix;
    }

    static public Autodesk.AdvanceSteel.Geometry.Plane ToAstPlane(Autodesk.DesignScript.Geometry.Plane dynPlane, bool bConvertToAstUnits)
    {
      Point3d planeOrigin = ToAstPoint(dynPlane.Origin, bConvertToAstUnits);
      Vector3d planeNormal = ToAstVector3d(dynPlane.Normal, bConvertToAstUnits);
      Autodesk.AdvanceSteel.Geometry.Plane plane = new Autodesk.AdvanceSteel.Geometry.Plane(planeOrigin, planeNormal);
      return plane;
    }

    static public Autodesk.DesignScript.Geometry.Plane ToDynPlane(Autodesk.AdvanceSteel.Geometry.Plane astPlane, bool bConvertToAstUnits)
    {
      Autodesk.DesignScript.Geometry.Point planeOrigin = ToDynPoint(astPlane.Origin, bConvertToAstUnits);
      Autodesk.DesignScript.Geometry.Vector planeNormal = ToDynVector(astPlane.Normal, bConvertToAstUnits);
      Autodesk.DesignScript.Geometry.Plane plane = Autodesk.DesignScript.Geometry.Plane.ByOriginNormal(planeOrigin, planeNormal);
      return plane;
    }

    static public Autodesk.AdvanceSteel.Geometry.Line3d ToAstLine3D(Autodesk.DesignScript.Geometry.Line line, bool bConvertToAstUnits)
    {
      Autodesk.AdvanceSteel.Geometry.Line3d ret = null;

      if (line != null)
      {
        Autodesk.AdvanceSteel.Geometry.Point3d startPoint = ToAstPoint(line.StartPoint, bConvertToAstUnits);
        Autodesk.AdvanceSteel.Geometry.Point3d endPoint = ToAstPoint(line.EndPoint, bConvertToAstUnits);
        ret = new Line3d(startPoint, endPoint);
      }

      return ret;
    }

    static public Autodesk.DesignScript.Geometry.Line ToDynLine(Autodesk.AdvanceSteel.Geometry.Line3d line, bool bConvertToAstUnits)
    {
      Autodesk.DesignScript.Geometry.Line ret = null;

      if (line != null)
      {
        Autodesk.DesignScript.Geometry.Point startPoint = ToDynPoint(line.EvalPoint(0), bConvertToAstUnits);
        Autodesk.DesignScript.Geometry.Point endPoint = ToDynPoint(line.EvalPoint(line.GetLength()), bConvertToAstUnits);
        ret = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(startPoint, endPoint);
      }

      return ret;
    }

    static public Autodesk.AdvanceSteel.Geometry.Point3d[] ToAstPoints(Autodesk.DesignScript.Geometry.Point[] pts, bool bConvertToAstUnits)
    {
      if (pts == null)
        return new Autodesk.AdvanceSteel.Geometry.Point3d[0];

      Autodesk.AdvanceSteel.Geometry.Point3d[] astPts = new Autodesk.AdvanceSteel.Geometry.Point3d[pts.Length];
      for (int nIdx = 0; nIdx < pts.Length; nIdx++)
      {
        astPts[nIdx] = ToAstPoint(pts[nIdx], bConvertToAstUnits);
      }

      return astPts;
    }

    static public Autodesk.DesignScript.Geometry.Point[] ToDynPoints(Autodesk.AdvanceSteel.Geometry.Point3d[] astPts, bool bConvertToAstUnits)
    {
      if (astPts == null)
        return new Autodesk.DesignScript.Geometry.Point[0];

      Autodesk.DesignScript.Geometry.Point[] pts = new Autodesk.DesignScript.Geometry.Point[astPts.Length];
      for (int nIdx = 0; nIdx < pts.Length; nIdx++)
      {
        pts[nIdx] = ToDynPoint(astPts[nIdx], bConvertToAstUnits);
      }

      return pts;
    }

    static public FilerObject GetObject(string handle)
    {
      return FilerObject.GetFilerObjectByHandle(handle);
    }

    internal static void SetOrientation(Autodesk.AdvanceSteel.Modelling.Beam beam, Autodesk.AdvanceSteel.Geometry.Vector3d vOrientation)
    {
      beam.PhysCSStart.GetCoordSystem(out _, out Vector3d vXAxis, out Vector3d vYAxis, out Vector3d vZAxis);
      if (!vXAxis.IsParallelTo(vOrientation))
      {
        Vector3d vProj = vOrientation.OrthoProject(vXAxis);

        double dAngle = vZAxis.GetAngleTo(vProj, vXAxis);

        beam.SetXRotation(dAngle * 180 / Math.PI);
      }
    }

    internal static void AdjustBeamEnd(Autodesk.AdvanceSteel.Modelling.Beam beam, Autodesk.AdvanceSteel.Geometry.Point3d newPtStart)
    {
      Autodesk.AdvanceSteel.Geometry.Point3d beamPtStart = beam.GetPointAtStart();
      Autodesk.AdvanceSteel.Geometry.Point3d beamPtEnd = beam.GetPointAtEnd();

      if (beamPtEnd.IsEqualTo(newPtStart))
      {
        Autodesk.AdvanceSteel.Geometry.Point3d newBeamEnd = beamPtEnd + (beamPtEnd - beamPtStart) * 0.5;
        beam.SetSysEnd(newBeamEnd);
      }
    }

    internal static string Separator
    {
      get { return separator; }
    }

    internal static string[] SplitSectionName(string sectionName)
    {
      string[] result = sectionName.Split(new string[] { Separator }, System.StringSplitOptions.None);

      if (2 == result.Length)
      {
        return result;
      }
      else
      {
        throw new System.Exception("Invalid section name");
      }
    }

    internal static bool CompareCompoundSectionTypes(string first, string second)
    {
      if (first.Equals(second) || (first.Contains("Welded") && second.Contains("Welded")) || (first.Contains("Compound") && second.Contains("Compound")) || (first.Contains("Tapered") && second.Contains("Tapered")))
      {
        return true;
      }
      return false;
    }

    public static double GetDiagonalLength(Autodesk.AdvanceSteel.Geometry.Point3d point1, Autodesk.AdvanceSteel.Geometry.Point3d point2)
    {
      return (point2 - point1).GetLength();
    }

    public static double GetRectangleAngle(Autodesk.AdvanceSteel.Geometry.Point3d point1, Autodesk.AdvanceSteel.Geometry.Point3d point2, Autodesk.AdvanceSteel.Geometry.Vector3d vx)
    {
      return (point2 - point1).GetAngleTo(vx);
    }

    public static double GetRectangleLength(Autodesk.AdvanceSteel.Geometry.Point3d point1, Autodesk.AdvanceSteel.Geometry.Point3d point2, Autodesk.AdvanceSteel.Geometry.Vector3d vx)
    {
      var diagLen = GetDiagonalLength(point1, point2);
      var alpha = GetRectangleAngle(point1, point2, vx);

      return diagLen * Math.Cos(alpha);
    }

    public static double GetRectangleHeight(Autodesk.AdvanceSteel.Geometry.Point3d point1, Autodesk.AdvanceSteel.Geometry.Point3d point2, Autodesk.AdvanceSteel.Geometry.Vector3d vx)
    {
      var diagLen = GetDiagonalLength(point1, point2);
      var alpha = GetRectangleAngle(point1, point2, vx);

      return diagLen * Math.Sin(alpha);
    }

    public static Dictionary<Autodesk.AdvanceSteel.CADAccess.FilerObject.eObjectType, string> GetASObjectFilters()
    {
      return filterSteelObjects;
    }

    public static IEnumerable<SteelDbObject> GetDynObjects(IEnumerable<string> handlesToFind)
    {
      var retListOfSteelObjects = new List<SteelDbObject>();
      using (var ctx = new DocContext())
      {
        foreach (var objHandle in handlesToFind)
        {
          FilerObject obj = Utils.GetObject(objHandle);
          if (obj == null)
          {
            throw new System.Exception("Object is empty");
          }

          if (filterSteelObjects.ContainsKey(obj.Type()))
          {
            SteelDbObject foundSteelObj = obj.ToDSType();
            retListOfSteelObjects.Add(foundSteelObj);
          }

        }
      }
      return retListOfSteelObjects;
    }

    public static IEnumerable<SteelDbObject> GetDynObjects(int objectSelectionType)
    {
      var retListOfSteelObjects = new List<SteelDbObject>();
      using (var ctx = new DocContext())
      {
        List<string> ret = new List<string>() { };

        ObjectId[] OIDxs = null;

        ClassTypeFilter xFilter = new ClassTypeFilter();
        xFilter.RejectAllFirst();
        xFilter.AppendAcceptedClass((FilerObject.eObjectType)objectSelectionType);

        DatabaseManager.GetModelObjectIds(out OIDxs, xFilter);
        List<ObjectId> OIDx = OIDxs.ToList<ObjectId>();
        if (OIDx.Count > 0)
        {
          for (int i = 0; i < OIDx.Count; i++)
          {
            FilerObject obj = FilerObject.GetFilerObject(OIDx[i]);
            if (obj != null)
            {
              string objHandle = obj.Handle;
              if (filterSteelObjects.ContainsKey(obj.Type()))
              {
                SteelDbObject foundSteelObj = obj.ToDSType();
                retListOfSteelObjects.Add(foundSteelObj);
              }
            }
          }
        }
      }
      return retListOfSteelObjects;
    }

    public static FilerObject[] GetSteelObjectsToConnect(IEnumerable<string> handlesToConnect)
    {
      var ret = new List<FilerObject>();
      bool Objs = false;
      foreach (var objHandle in handlesToConnect)
      {
        FilerObject obj = Utils.GetObject(objHandle);
        if (obj != null && (obj.IsKindOf(FilerObject.eObjectType.kBeam) ||
                            obj.IsKindOf(FilerObject.eObjectType.kBentBeam) ||
                            obj.IsKindOf(FilerObject.eObjectType.kPlate) ||
                            obj.IsKindOf(FilerObject.eObjectType.kSpecialPart)))
        {
          Objs = true;
          ret.Add(obj);
        }
        else
        {
          throw new System.Exception("Object is empty");
        }
      }
      return Objs ? ret.ToArray() : Array.Empty<FilerObject>();
    }

    public static FilerObject[] GetFilerObjects(IEnumerable<string> handles)
    {
      var ret = new List<FilerObject>();
      bool Objs = false;
      foreach (var objHandle in handles)
      {
        Objs = true;
        ret.Add(Utils.GetObject(objHandle));
      }

      return Objs ? ret.ToArray() : Array.Empty<FilerObject>();
    }

    public static double GetPaintArea(string handle)
    {
      double ret = 0;
      FilerObject filerObj = Utils.GetObject(handle);
      if (filerObj != null)
      {
        if (filerObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
        {
          Autodesk.AdvanceSteel.Modelling.PlateBase selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.PlateBase;
          ret = (double)selectedObj.GetPaintArea();
        }
        else if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
        {
          Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
          ret = (double)selectedObj.GetPaintArea();
        }
        else
          throw new System.Exception("Not a Supported Object for Paint Area");
      }
      else
        throw new System.Exception("AS Object is null");

      return ret;
    }

    public static double GetWeight(string handle, int weightCode)
    {
      double ret = 0;
      FilerObject filerObj = Utils.GetObject(handle);
      if (filerObj != null)
      {
        if (filerObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
        {
          Autodesk.AdvanceSteel.Modelling.PlateBase selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.PlateBase;
          ret = (double)selectedObj.GetWeight(weightCode);
        }
        else if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
        {
          Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
          ret = (double)selectedObj.GetWeight(weightCode);
        }
        else if (filerObj.IsKindOf(FilerObject.eObjectType.kScrewBoltPattern))
        {
          Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern;
          ret = (double)selectedObj.GetWeight();
        }
        else if (filerObj.IsKindOf(FilerObject.eObjectType.kConnector))
        {
          Autodesk.AdvanceSteel.Modelling.Connector selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Connector;
          ret = (double)selectedObj.GetWeight();
        }
        else
          throw new System.Exception("Not a Supported Object for Paint Area");
      }
      else
        throw new System.Exception("AS Object is null");

      return ret;
    }

    public static List<string> GetSteelDbObjectsToConnect(IEnumerable<SteelDbObject> objectsToConnect)
    {
      List<string> handlesList = new List<string>();
      foreach (var obj in objectsToConnect)
      {
        if (obj is AdvanceSteel.Nodes.Beams.BentBeam ||
            obj is AdvanceSteel.Nodes.Beams.StraightBeam ||
            obj is AdvanceSteel.Nodes.Beams.UnFoldedBeam ||
            obj is AdvanceSteel.Nodes.Beams.PolyBeam ||
            obj is AdvanceSteel.Nodes.Beams.TaperedBeam ||
            obj is AdvanceSteel.Nodes.Beams.CompoundBeam ||
            obj is AdvanceSteel.Nodes.Plates.Plate ||
            obj is AdvanceSteel.Nodes.Miscellaneous.SpecialPart)
        {
          handlesList.Add(obj.Handle);
        }
        else
        {
          throw new Exception("Only beams and plates can be connected");
        }
      }
      return handlesList;
    }


    #region "UI Get Methods for Properties

    public static Dictionary<string, Property> GetMidScrewBoltPattern()
    {
      return Build_Master_InfinitMidScrewBoltPattern();
    }

    public static Dictionary<string, Property> GetCircleScrewBoltPattern()
    {
      return Build_Master_CircleScrewBoltPattern();
    }

    public static Dictionary<string, Property> GetConcreteSlabProperties()
    {
      return Build_Master_Slab(); 
    }

    public static Dictionary<string, Property> GetConcreteIsolatedFootingProperties()
    {
      return Build_Master_FootingIsolated();
    }

    public static Dictionary<string, Property> GetConcreteWallProperties()
    {
      return Build_Master_Wall();
    }

    public static Dictionary<string, Property> GetConcreteStraightBeamProperties()
    {
      return Build_Master_ConcreteBeam();
    }

    public static Dictionary<string, Property> GetUnfoldedStraightBeamProperties()
    {
      return Build_Master_UnfoldedStraightBeam();
    }

    public static Dictionary<string, Property> GetWeldPointPropertiesList()
    {
      return Build_Master_WeldPoint();
    }

    public static Dictionary<string, Property> GetWeldLinePropertiesList()
    {
      return Build_Master_WeldLine();
    }

    public static Dictionary<string, Property> GetConcreteBentBeamProperties()
    {
      return Build_Master_ConcreteBentBeam();
    }

    public static Dictionary<string, Property> GetAnchorBoltPropertyList()
    {
      return Build_Master_AnchorPattern();
    }

    public static Dictionary<string, Property> GetShearStudPropertyList()
    {
      return Build_Master_Connector();
    }

    public static Dictionary<string, Property> GetConnectionHolePlatePropertyList()
    {
      return Build_Master_ConnectionHolePlate();
    }


    public static Dictionary<string, Property> GetPlatePropertyList()
    {
      return Build_Master_Plate();
    }

    public static Dictionary<string, Property> GetCameraPropertyList()
    {
      return Build_Master_Camera();
    }

    public static Dictionary<string, Property> GetPlateFeaturePropertyList()
    {
      return Build_Master_PlateFeatContour(); 
    }

    public static Dictionary<string, Property> GetBeamCutPlanePropertyList()
    {
      return Build_Master_BeamShortening();
    }

    public static Dictionary<string, Property> GetBeamMultiNotchPropertyList()
    {
      return Build_Master_BeamMultiContourNotch();
    }

    public static Dictionary<string, Property> GetPlateNotchContourPropertyList()
    {
      return Build_Master_PlateContourNotch();
    }

    public static Dictionary<string, Property> GetBeamNotchOrthoPropertyList()
    {
      return Build_Master_BeamNotch2Ortho();
    }

    public static Dictionary<string, Property> GetBeamNotchRotatedPropertyList()
    {
      return Build_Master_BeamNotchEx();
    }

    public static Dictionary<string, Property> GetPlateVertexPropertyList()
    {
      return Build_Master_PlateFeatVertFillet();
    }

    public static Dictionary<string, Property> GetGratingPropertyList()
    {
      return Build_Master_Grating();
    }

    public static Dictionary<string, Property> GetStraighBeamPropertyList()
    {
      return Build_Master_StraightBeam(); ;
    }

    public static Dictionary<string, Property> GetBentBeamPropertyList()
    {
      return Build_Master_BentBeam();
    }

    public static Dictionary<string, Property> GetPolyBeamPropertyList()
    {
      return Build_Master_PolyBeam(); 
    }

    public static Dictionary<string, Property> GetSpecialPartPropertyList()
    {
      return Build_Master_SpecialPart(); ;
    }

    public static Dictionary<string, Property> GetTaperBeamPropertyList()
    {
      return Build_Master_BeamTapered();
    }

    public static Dictionary<string, Property> GetCompoundStraightBeamPropertyList()
    {
      return Build_Master_CompoundStraightBeam(); ;
    }

    #endregion


    public static Property GetProperty(string keyValue)
    {
      foreach (KeyValuePair<eObjectType, Dictionary<string, Property>> item in steelObjectPropertySets)
      {
        if (item.Value.TryGetValue(keyValue, out Property retValue))
        {
          return new Property(retValue);
        }
      }
      return null;
    }

    public static Dictionary<string, Property> GetAllProperties(FilerObject steelFiler)
    {
      Dictionary<string, Property> ret = new Dictionary<string, Property>() { };
      foreach (KeyValuePair<string, Property> item in steelObjectPropertySets[steelFiler.Type()])
      {
        ret.Add(item.Key, new Property(item.Value));
      }
      return ret; 
    }

    private static Dictionary<string, Property> addElementTypes(Dictionary<string, Property> dictProps, List<eObjectType> elementTypes)
    {
      foreach (string key in dictProps.Keys.ToList())
      {
        if (key.EndsWith("..."))
        {
          dictProps[key].ElementTypeList = new List<eObjectType>() { eObjectType.kUnknown };
        }
        else
        {
          dictProps[key].ElementTypeList.AddRange(elementTypes);
        }
      }
      return dictProps;
    }

    public static void CheckListUpdateOrAddValue(List<Property> listOfPropertyData,
                                              string propName,
                                              object propValue,
                                              string propLevel = "")
    {
      var foundItem = listOfPropertyData.FirstOrDefault<Property>(props => props.Name == propName);
      if (foundItem != null)
      {
        foundItem.InternalValue = propValue;
      }
      else
      {
        listOfPropertyData.Add(new Property(propName, propValue, propValue.GetType(), propLevel));
      }
    }


    #region Set Parameters Methods
    
    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.CountableScrewBoltPattern objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.Grid objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.PlateContourNotch objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.PlateFeatContour objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }
    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.BeamShortening objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.BeamNotch2Ortho objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.BeamNotchEx objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.BeamMultiContourNotch objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.PlateFeatVertFillet objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.ConstructionHelper.Camera objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Arrangement.Arranger objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.ConstructionTypes.AtomicElement objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.AnchorPattern objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    #endregion


    #region Property Base Class Definitions

    private static Dictionary<string, Property> Build_ActiveConstructionElement()
    {
      Dictionary<string, Property> dictProps = Build_ConstructionElement();
      dictProps.Add("Coordinate System", new Property("CS", typeof(Matrix3d), ".", true));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kActConstructionElem });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_AtomicElement()
    {
      Dictionary<string, Property> dictProps = Build_ActiveConstructionElement();
      dictProps.Add("Volume", new Property("Volume", typeof(double), ".", true));
      dictProps.Add("Used For Numbering - Assembly", new Property("AssemblyUsedForNumbering", typeof(int)));
      dictProps.Add("Used For Numbering - Note", new Property("NoteUsedForNumbering", typeof(int)));
      dictProps.Add("Used For Numbering - Role", new Property("RoleUsedForNumbering", typeof(int)));
      dictProps.Add("Used For BOM - SinglePart", new Property("SinglePartUsedForBOM", typeof(int)));
      dictProps.Add("Used For BOM - MainPart", new Property("MainPartUsedForBOM", typeof(int)));
      dictProps.Add("Used For CollisionCheck - SinglePart", new Property("SinglePartUsedForCollisionCheck", typeof(int)));
      dictProps.Add("Used For CollisionCheck - MainPart", new Property("MainPartUsedForCollisionCheck", typeof(int)));
      dictProps.Add("Structural Member", new Property("StructuralMember", typeof(int)));
      dictProps.Add("Used For Numbering - Holes", new Property("HolesUsedForNumbering", typeof(int)));
      dictProps.Add("MainPart Number", new Property("MainPartNumber", typeof(string)));
      dictProps.Add("SinglePart Number", new Property("SinglePartNumber", typeof(string)));
      dictProps.Add("PreliminaryPart Prefix", new Property("PreliminaryPartPrefix", typeof(string)));
      dictProps.Add("PreliminaryPart Number", new Property("PreliminaryPartNumber", typeof(string)));
      dictProps.Add("PreliminaryPart PositionNumber", new Property("PreliminaryPartPositionNumber", typeof(string)));
      dictProps.Add("Used For Numbering - ItemNumber", new Property("ItemNumberUsedForNumbering", typeof(int)));
      dictProps.Add("Used For Numbering - Dennotation", new Property("DennotationUsedForNumbering", typeof(int)));
      dictProps.Add("Used For Numbering - Coating", new Property("CoatingUsedForNumbering", typeof(int)));
      dictProps.Add("Used For Numbering - Material", new Property("MaterialUsedForNumbering", typeof(int)));
      dictProps.Add("Unwind StartFactor", new Property("UnwindStartFactor", typeof(double)));
      dictProps.Add("Denotation", new Property("Denotation", typeof(string)));
      dictProps.Add("Assembly", new Property("Assembly", typeof(string)));
      dictProps.Add("Note", new Property("Note", typeof(string)));
      dictProps.Add("ItemNumber", new Property("ItemNumber", typeof(string)));
      dictProps.Add("Specific Gravity", new Property("SpecificGravity", typeof(double)));
      dictProps.Add("Coating", new Property("Coating", typeof(string)));
      dictProps.Add("Number Of Holes", new Property("NumberOfHoles", typeof(int), ".", true));
      dictProps.Add("Is AttachedPart", new Property("IsAttachedPart", typeof(bool), ".", true));
      dictProps.Add("Is MainPart", new Property("IsMainPart", typeof(bool)));
      dictProps.Add("MainPart Prefix", new Property("MainPartPrefix", typeof(string)));
      dictProps.Add("SinglePart Prefix", new Property("SinglePartPrefix", typeof(string)));
      dictProps.Add("Used For Numbering - SinglePart", new Property("SinglePartUsedForNumbering", typeof(int)));
      dictProps.Add("Used For Numbering - MainPart", new Property("MainPartUsedForNumbering", typeof(int)));
      dictProps.Add("Explicit Quantity", new Property("ExplicitQuantity", typeof(int)));
      dictProps.Add("Material Description", new Property("MaterialDescription", typeof(string), ".", true));
      dictProps.Add("Coating Description", new Property("CoatingDescription", typeof(string), ".", true));
      dictProps.Add("Material", new Property("Material", typeof(string)));
      dictProps.Add("Unwind", new Property("Unwind", typeof(bool)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kAtomicElem });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_MainAlias()
    {
      Dictionary<string, Property> dictProps = Build_AtomicElement();

      dictProps.Add("Used For Numbering - Fabrication Station", new Property("FabricationStationUsedForNumbering", typeof(bool)));
      dictProps.Add("Load Number", new Property("LoadNumber", typeof(string)));
      dictProps.Add("Carrier", new Property("Carrier", typeof(string)));
      dictProps.Add("Fabrication Station", new Property("FabricationStation", typeof(string)));
      dictProps.Add("Supplier", new Property("Supplier", typeof(string)));
      dictProps.Add("PO Number", new Property("PONumber", typeof(string)));
      dictProps.Add("Requisition Number", new Property("RequisitionNumber", typeof(string)));
      dictProps.Add("Heat Number", new Property("HeatNumber", typeof(string)));
      dictProps.Add("Shipped Date", new Property("ShippedDate", typeof(string)));
      dictProps.Add("Delivery Date", new Property("DeliveryDate", typeof(string)));
      dictProps.Add("Used For Numbering - Supplier", new Property("SupplierUsedForNumbering", typeof(bool)));
      dictProps.Add("Used For Numbering - RequisitionNumber", new Property("RequisitionNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Approval Comment", new Property("ApprovalComment", typeof(string)));
      dictProps.Add("Used For Numbering - Heat Number", new Property("HeatNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Used For Numbering - PO Number", new Property("PONumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Approval Status Code", new Property("ApprovalStatusCode", typeof(string)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kMainAlias });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Platebase()
    {
      Dictionary<string, Property> dictProps = Build_MainAlias();

      dictProps.Add("Plate Portioning", new Property("Portioning", typeof(double)));
      dictProps.Add("Upper Plane", new Property("UpperPlane", typeof(Plane), ".", true));
      dictProps.Add("Length", new Property("Length", typeof(double), ".", true));
      dictProps.Add("Width", new Property("Width", typeof(double), ".", true));
      dictProps.Add("Length Increment", new Property("LengthIncrement", typeof(double)));
      dictProps.Add("Radius", new Property("Radius", typeof(double), ".", true));
      dictProps.Add("Radius Increment", new Property("RadIncrement", typeof(double)));
      dictProps.Add("Lower Z Position", new Property("LowerZPos", typeof(double), ".", true));
      dictProps.Add("Plate Normal", new Property("PlateNormal", typeof(Vector3d), ".", true));
      dictProps.Add("Thickness", new Property("Thickness", typeof(double)));
      dictProps.Add("Lower Plane", new Property("LowerPlane", typeof(Plane), ".", true));
      dictProps.Add("Upper Z Position", new Property("UpperZPos", typeof(double), ".", true));
      dictProps.Add("Top Is Z Positive", new Property("TopIsZPositive", typeof(bool)));
      dictProps.Add("DefinitionPlane", new Property("DefinitionPlane", typeof(Plane)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPlateBase });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Beam()
    {
      Dictionary<string, Property> dictProps = Build_MainAlias();

      dictProps.Add("Coordinate System at System Mid", new Property("SysCSMid", typeof(Matrix3d), ".", true));
      dictProps.Add("Profile Section Name", new Property("ProfSectionName", typeof(string), ".", true));
      dictProps.Add("Coordinate System at Physical End", new Property("PhysCSEnd", typeof(Matrix3d), ".", true));
      dictProps.Add("Profile Section Type", new Property("ProfSectionType", typeof(string)));
      dictProps.Add("Systemline Length", new Property("SysLength", typeof(double), ".", true));
      dictProps.Add("Deviation", new Property("Deviation", typeof(double)));
      dictProps.Add("Beam Shrink Value", new Property("ShrinkValue", typeof(double), ".", true));
      dictProps.Add("Coordinate System at System Start", new Property("SysCSStart", typeof(Matrix3d), ".", true));
      dictProps.Add("Is Cross Section Mirrored", new Property("IsCrossSectionMirrored", typeof(bool), ".", true));
      dictProps.Add("Angle", new Property("Angle", typeof(double)));
      dictProps.Add("Profile Name", new Property("ProfName", typeof(string)));
      dictProps.Add("Coordinate System at System End", new Property("SysCSEnd", typeof(Matrix3d), ".", true));
      dictProps.Add("Beam Runname", new Property("Runname", typeof(string), ".", true));
      dictProps.Add("Coordinate System at Physical Start", new Property("PhysCSStart", typeof(Matrix3d), ".", true));
      dictProps.Add("Coordinate System at Physical Mid", new Property("PhysCSMid", typeof(Matrix3d), ".", true));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBeam });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_BentBeamBase()
    {
      Dictionary<string, Property> dictProps = Build_Beam();
      dictProps.Add("Offset Curve Radius", new Property("OffsetCurveRadius", typeof(double)));
      dictProps.Add("CurveOffset", new Property("CurveOffset", typeof(double), ".", true));
      dictProps.Add("Definition Plane Coordinate System", new Property("DefinitionPlane", typeof(Matrix3d), ".", true));
      dictProps.Add("Systemline Radius", new Property("SystemlineRadius", typeof(double), ".", true));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBentBeamBase });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_CompoundBeam()
    {
      Dictionary<string, Property> dictProps = Build_Beam();
      dictProps.Add("CompoundTypeName", new Property("CompoundTypeName", typeof(string), ".", true));
      dictProps.Add("CompoundClassName", new Property("CompoundClassName", typeof(string), ".", true));
      dictProps.Add("Use Compound As One Beam", new Property("UseCompoundAsOneBeam", typeof(bool)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kCompoundBeam });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_PolyBeam()
    {
      Dictionary<string, Property> dictProps = Build_Beam();
      dictProps.Add("Vector Reference Orientation", new Property("VecRefOrientation", typeof(Vector3d), ".", true));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPolyBeam });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_UnfoldedStraightBeam()
    {
      Dictionary<string, Property> dictProps = Build_Beam();
      dictProps.Add("Unfolded Bem Thickness", new Property("Thickness", typeof(double)));
      dictProps.Add("Unfolded Bem Portioning", new Property("Portioning", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kUnfoldedStraightBeam });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Wall()
    {
      Dictionary<string, Property> dictProps = Build_Platebase();
      dictProps.Add("Wall Height", new Property("Height", typeof(double), ".", true));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kWall });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Camera()
    {
      Dictionary<string, Property> dictProps = Build_FilerObject();
      dictProps.Add("Camera Description", new Property("Description", typeof(string)));
      dictProps.Add("Camera Type", new Property("CameraType", typeof(string)));
      dictProps.Add("Coordinate System of Camera", new Property("CameraCS", typeof(Matrix3d)));
      dictProps.Add("Scale", new Property("Scale", typeof(double)));
      dictProps.Add("Type Description", new Property("TypeDescription", typeof(string), ".", true));
      dictProps.Add("Disable Supports Detailing", new Property("SupportsDetailingDisable", typeof(bool), ".", true));
      dictProps.Add("Detail Style Location Index Number", new Property("DetailStyleLocation", typeof(int)));
      dictProps.Add("Detail Style Index Number", new Property("DetailStyle", typeof(int)));
      dictProps.Add("Disable Detailing", new Property("DisableDetailing", typeof(bool)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kCamera });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Grid()
    {
      Dictionary<string, Property> dictProps = Build_ConstructionElement();
      dictProps.Add("Coordinate System of Grid", new Property("CS", typeof(Matrix3d)));
      dictProps.Add("Grid Numbering Start Text", new Property("NumberingStart", typeof(string)));
      dictProps.Add("Vertical Series", new Property("VerticalSeries", typeof(bool)));
      dictProps.Add("Axis Frame", new Property("AxisFrame", typeof(bool)));
      dictProps.Add("Grid Numbering Prefix", new Property("NumberingPrefix", typeof(string)));
      dictProps.Add("Grid Numbering Suffix", new Property("NumberingSuffix", typeof(string)));
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kGrid });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_WeldPattern()
    {
      Dictionary<string, Property> dictProps = Build_AtomicElement();
      dictProps.Add("Weld Thickness", new Property("Thickness", typeof(double)));
      dictProps.Add("Weld Main Effective Throat", new Property("MainEffectiveThroat", typeof(double)));
      dictProps.Add("Weld Double Effective Throat", new Property("DoubleEffectiveThroat", typeof(double)));
      dictProps.Add("Weld Main Preparation Depth", new Property("MainPreparationDepth", typeof(double)));
      dictProps.Add("Weld Double Preparation Depth", new Property("DoublePreparationDepth", typeof(double)));
      dictProps.Add("Weld Prefix", new Property("Prefix", typeof(string)));
      dictProps.Add("Weld Is Closed", new Property("IsClosed", typeof(bool)));
      dictProps.Add("Weld Length", new Property("Length", typeof(double), ".", true));
      dictProps.Add("Weld Double Weld Text", new Property("DoubleWeldText", typeof(string)));
      dictProps.Add("Weld Double Root Opening", new Property("DoubleRootOpening", typeof(double)));
      dictProps.Add("Weld Main Weld Text", new Property("MainWeldText", typeof(string)));
      dictProps.Add("Weld Pitch", new Property("Pitch", typeof(double)));
      dictProps.Add("Weld Additional Data", new Property("AdditionalData", typeof(string)));
      dictProps.Add("Weld Single Seam Length", new Property("SingleSeamLength", typeof(double)));
      dictProps.Add("Weld Seam Distance", new Property("SeamDistance", typeof(double)));
      dictProps.Add("Weld Main Root Opening", new Property("MainRootOpening", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kWeldPattern });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Arranger()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Arranger No of Holes in the Y Direction", new Property("Ny", typeof(int), "Arranger", false));
      dictProps.Add("Arranger No of Holes in the X Direction", new Property("Nx", typeof(int), "Arranger", false));
      dictProps.Add("Arranger Spacing of holes in the X Direction", new Property("Dx", typeof(double), "Arranger", false));
      dictProps.Add("Arranger Spacing of holes in the X Direction", new Property("Dy", typeof(double), "Arranger", false));
      dictProps.Add("Arranger Hole Pattern Width in the X Direction", new Property("Wy", typeof(double), "Arranger", false));
      dictProps.Add("Arranger Hole Pattern Width in the X Direction", new Property("Wx", typeof(double), "Arranger", false));
      dictProps.Add("Arranger Width", new Property("Width", typeof(double), "Arranger", false));
      dictProps.Add("Arranger Length", new Property("Length", typeof(double), "Arranger", false));
      dictProps.Add("Arranger Number Of Elements", new Property("NumberOfElements", typeof(int), "Arranger", false));
      dictProps.Add("Arranger Radius", new Property("Radius", typeof(double), "Arranger", false));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBoltPattern });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_InfinitMidScrewBoltPattern()
    {
      Dictionary<string, Property> dictProps = Build_CountableScrewBoltPattern();
      dictProps.Add("Bolt Pattern Midpoint On LowerLeft", new Property("MidpointOnLowerLeft", typeof(Point3d), ".", true));
      dictProps.Add("Bolt Pattern Midpoint On LowerRight", new Property("MidpointOnLowerRight", typeof(Point3d), ".", true));
      dictProps.Add("Bolt Pattern Midpoint On UpperLeft", new Property("MidpointOnUpperLeft", typeof(Point3d), ".", true));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kInfinitMidScrewBoltPattern });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_CountableScrewBoltPattern()
    {
      Dictionary<string, Property> dictProps = Build_ScrewBoltPattern();
      dictProps.Add("Bolt Pattern Height", new Property("Height", typeof(double), ".", true));
      dictProps.Add("Bolt Pattern Length", new Property("Length", typeof(double), ".", true));
      dictProps.Add("Bolt Pattern Spacing of holes in the X Direction", new Property("Dx", typeof(double)));
      dictProps.Add("Bolt Pattern Spacing of holes in the Y Direction", new Property("Dy", typeof(double)));
      dictProps.Add("Bolt Pattern No of Holes in the Y Direction", new Property("Ny", typeof(int)));
      dictProps.Add("Bolt Pattern No of Holes in the X Direction", new Property("Nx", typeof(int)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kCountableScrewBoltPattern });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_ScrewBoltPattern()
    {
      Dictionary<string, Property> dictProps = Build_BoltPattern();
      dictProps.Add("Bolt Top Tool Diameter", new Property("TopToolDiameter", typeof(double), ".", true));
      dictProps.Add("Bolt Bottom Tool Diameter", new Property("BottomToolDiameter", typeof(double), ".", true));
      dictProps.Add("Bolt Bottom Tool Height", new Property("BottomToolHeight", typeof(double), ".", true));
      dictProps.Add("Bolt Head Number of Edges", new Property("BoltHeadNumEdges", typeof(int), ".", true));
      dictProps.Add("Bolt Head Diameter", new Property("BoltHeadDiameter", typeof(double), ".", true));
      dictProps.Add("Bolt Head Height", new Property("BoltHeadHeight", typeof(double), ".", true));
      dictProps.Add("Bolt Top Tool Height", new Property("TopToolHeight", typeof(double), ".", true));
      dictProps.Add("Bolt Assembly", new Property("BoltAssembly", typeof(string)));
      dictProps.Add("Bolt Grade", new Property("Grade", typeof(string)));
      dictProps.Add("Bolt Standard", new Property("Standard", typeof(string)));
      dictProps.Add("Bolt Hole Tolerance", new Property("HoleTolerance", typeof(double)));
      dictProps.Add("Bolt Binding Length Addition", new Property("BindingLengthAddition", typeof(double)));
      dictProps.Add("Bolt Annotation", new Property("Annotation", typeof(string), ".", true));
      dictProps.Add("Bolt Screw Length", new Property("ScrewLength", typeof(double)));
      dictProps.Add("Bolt Sum Top Height", new Property("SumTopHeight", typeof(double), ".", true));
      dictProps.Add("Bolt Sum Top Set Height", new Property("SumTopSetHeight", typeof(double), ".", true));
      dictProps.Add("Bolt Sum Bottom Set Height", new Property("SumBottomSetHeight", typeof(double), ".", true));
      dictProps.Add("Bolt Sum Bottom Height", new Property("SumBottomHeight", typeof(double), ".", true));
      dictProps.Add("Bolt Max Top Diameter", new Property("MaxTopDiameter", typeof(double), ".", true));
      dictProps.Add("Bolt Max Bottom Diameter", new Property("MaxBottomDiameter", typeof(double), ".", true));
      dictProps.Add("Bolt Nut Height", new Property("NutHeight", typeof(double), ".", true));
      dictProps.Add("Bolt Nut Diameter", new Property("NutDiameter", typeof(double), ".", true));
      dictProps.Add("Bolt Screw Diameter", new Property("ScrewDiameter", typeof(double)));
      dictProps.Add("Bolt Binding Length", new Property("BindingLength", typeof(double)));
      dictProps.Add("Ignore Max Gap", new Property("IgnoreMaxGap", typeof(bool)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kScrewBoltPattern });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_CircleScrewBoltPattern()
    {
      Dictionary<string, Property> dictProps = Build_ScrewBoltPattern();
      dictProps.Add("Circular Number Of Bolts", new Property("NumberOfScrews", typeof(int)));
      dictProps.Add("Circular Bolt Pattern Radius", new Property("Radius", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kCircleScrewBoltPattern });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Connector()
    {
      Dictionary<string, Property> dictProps = Build_AtomicElement();
      dictProps.Add("ShearStud Head Height", new Property("HeadHeight", typeof(double), ".", true));
      dictProps.Add("ShearStud Head Diameter", new Property("HeadDiameter", typeof(double), ".", true));
      dictProps.Add("ShearStud Length", new Property("Length", typeof(double)));
      dictProps.Add("ShearStud Diameter", new Property("Diameter", typeof(double)));
      dictProps.Add("ShearStud Grade", new Property("Grade", typeof(string)));
      dictProps.Add("ShearStud Standard", new Property("Standard", typeof(string)));
      dictProps.Add("ShearStud Normal Vector", new Property("Normal", typeof(Vector3d), ".", true));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kCircleScrewBoltPattern });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_BoltPattern()
    {
      Dictionary<string, Property> dictProps = Build_AtomicElement();

      dictProps.Add("RefPoint", new Property("RefPoint", typeof(Point3d)));
      dictProps.Add("NumberOfScrews", new Property("NumberOfScrews", typeof(int), ".", true));
      dictProps.Add("IsInverted", new Property("IsInverted", typeof(bool)));
      dictProps.Add("Center", new Property("Center", typeof(Point3d), ".", true));
      dictProps.Add("XDirection", new Property("XDirection", typeof(Vector3d)));
      dictProps.Add("BoltNormal", new Property("BoltNormal", typeof(Vector3d), ".", true));
      dictProps.Add("Normal", new Property("Normal", typeof(Vector3d), ".", true));
      dictProps.Add("YDirection", new Property("YDirection", typeof(Vector3d)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBoltPattern });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_AnchorPattern()
    {
      Dictionary<string, Property> dictProps = Build_ScrewBoltPattern();
      dictProps.Add("Anchor Pattern Size Y Direction", new Property("Wy", typeof(double)));
      dictProps.Add("Anchor Pattern Size X Direction", new Property("Wx", typeof(double)));
      dictProps.Add("Anchor Count", new Property("NumberOfScrews", typeof(int)));
      dictProps.Add("Anchor Spacing Y Direction", new Property("Dy", typeof(double)));
      dictProps.Add("Anchor Spacing X Direction", new Property("Dx", typeof(double)));
      dictProps.Add("Anchor Count X Direction", new Property("Nx", typeof(int)));
      dictProps.Add("Anchor Count Y Direction", new Property("Ny", typeof(int)));
      dictProps.Add("Anchor Unfolded Length", new Property("AnchorUnfoldedLength", typeof(double), ".", true));
      dictProps.Add("Anchor Thread Length", new Property("ThreadLength", typeof(double), ".", true));
      dictProps.Add("Anchor Midpoint On Upper Right", new Property("MidpointOnUpperRight", typeof(Point3d), ".", true));
      dictProps.Add("Anchor MidpointOnLowerLeft", new Property("MidpointOnLowerLeft", typeof(Point3d), ".", true));
      dictProps.Add("Anchor Midpoint On Lower Right", new Property("MidpointOnLowerRight", typeof(Point3d), ".", true));
      dictProps.Add("Anchor Midpoint On Upper Left", new Property("MidpointOnUpperLeft", typeof(Point3d), ".", true));
      dictProps.Add("Anchor Radius", new Property("Radius", typeof(double)));
      dictProps.Add("Anchor Part Name", new Property("AnchorPartName", typeof(string), ".", true));
      dictProps.Add("Anchor Height", new Property("Height", typeof(double), ".", true));
      dictProps.Add("Anchor Max Top Diameter", new Property("MaxTopDiameter", typeof(double), ".", true));
      dictProps.Add("Anchor Max Bottom Diameter", new Property("MaxBottomDiameter", typeof(double), ".", true));
      dictProps.Add("Anchor Length", new Property("Length", typeof(double), ".", true));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kAnchorPattern });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Grating()
    {
      Dictionary<string, Property> dictProps = Build_Platebase();
      dictProps.Add("BearingBarSpacing", new Property("BearingBarSpacing", typeof(int), ".", true));
      dictProps.Add("CrossBarQuantity", new Property("CrossBarQuantity", typeof(int), ".", true));
      dictProps.Add("BearingBarQuantity", new Property("BearingBarQuantity", typeof(int), ".", true));
      dictProps.Add("ConnectorKey", new Property("ConnectorKey", typeof(int)));
      dictProps.Add("OEDValue", new Property("OEDValue", typeof(double), ".", true));
      dictProps.Add("EDValue", new Property("EDValue", typeof(double)));
      dictProps.Add("IsUsingStandardED", new Property("IsUsingStandardED", typeof(bool)));
      dictProps.Add("WidthExtensionRight", new Property("WidthExtensionRight", typeof(double)));
      dictProps.Add("WidthExtensionLeft", new Property("WidthExtensionLeft", typeof(double)));
      dictProps.Add("GratingClass", new Property("GratingClass", typeof(string)));
      dictProps.Add("GratingSize", new Property("GratingSize", typeof(string)));
      dictProps.Add("BearingBarSpacingDistance", new Property("BearingBarSpacingDistance", typeof(double), ".", true));
      dictProps.Add("IsMatCoatDbDefined", new Property("IsMatCoatDbDefined", typeof(bool)));
      dictProps.Add("Direction", new Property("Direction", typeof(Vector3d)));
      dictProps.Add("IsTopOnUpperPlane", new Property("IsTopOnUpperPlane", typeof(bool)));
      dictProps.Add("ConnectorQuantity", new Property("ConnectorQuantity", typeof(int)));
      dictProps.Add("ConnectorName", new Property("ConnectorName", typeof(string)));
      dictProps.Add("CrossBar", new Property("CrossBar", typeof(string)));
      dictProps.Add("CrossBarSpacingDistance", new Property("CrossBarSpacingDistance", typeof(double), ".", true));
      dictProps.Add("ThicknessOfABearingBar", new Property("ThicknessOfABearingBar", typeof(double), ".", true));
      dictProps.Add("CrossBarSpacing", new Property("CrossBarSpacing", typeof(double), ".", true));
      dictProps.Add("IsUsingStandardHatch", new Property("IsUsingStandardHatch", typeof(bool)));
      dictProps.Add("StandardHatch", new Property("StandardHatch", typeof(string), ".", true));
      dictProps.Add("ThicknessOfACrossBar", new Property("ThicknessOfACrossBar", typeof(double), ".", true));
      dictProps.Add("CustomHatch", new Property("CustomHatch", typeof(string)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kGrating });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_SpecialPart()
    {
      Dictionary<string, Property> dictProps = Build_AtomicElement();
      dictProps.Add("SpecialPart BlockName", new Property("BlockName", typeof(string), ".", true));
      dictProps.Add("SpecialPart Depth", new Property("Depth", typeof(double), ".", true));
      dictProps.Add("SpecialPart Width", new Property("Width", typeof(double), ".", true));
      dictProps.Add("SpecialPart Length", new Property("Length", typeof(double), ".", true));
      dictProps.Add("SpecialPart PaintArea", new Property("PaintArea", typeof(double)));
      dictProps.Add("SpecialPart Scale", new Property("Scale", typeof(double)));
      dictProps.Add("SpecialPart Weight", new Property("Weight", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kSpecialPart });

      return dictProps;
    }
    
    private static Dictionary<string, Property> Build_ConnectionHolePlate()
    {
      Dictionary<string, Property> dictProps = Build_ConnectionFeature();

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kConnectionHolePlate  });

      return dictProps;
    }
    
    private static Dictionary<string, Property> Build_ConnectionFeature()
    {
      Dictionary<string, Property> dictProps = Build_FeatureObject();

      dictProps.Add("Hole Depth", new Property("Depth", typeof(double)));
      dictProps.Add("Hole Angle", new Property("Angle", typeof(double)));
      dictProps.Add("Use Hole Definition for Numbering", new Property("UsedForNumbering", typeof(double)));

      dictProps.Add("Hole Exact Coordinate System", new Property("CSExact", typeof(Matrix3d), ".", true));
      dictProps.Add("Hole Local Coordinate System", new Property("CSLocal", typeof(Matrix3d)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kConnectionFeature });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_PlateFeatVertFillet()
    {
      Dictionary<string, Property> dictProps = Build_PlateFeatVertex();
      dictProps.Add("Vertex Fillet Length 1", new Property("Length1", typeof(double)));
      dictProps.Add("Vertex Fillet Length 2", new Property("Length2", typeof(double)));
      dictProps.Add("Vertex Fillet Radius Increment", new Property("RadiusIncrement", typeof(double)));
      dictProps.Add("Vertex Fillet Radius", new Property("Radius", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                      eObjectType.kPlateFeatVertFillet });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_PlateFeatVertex()
    {
      Dictionary<string, Property> dictProps = Build_FeatureObject();
      dictProps.Add("Plate Feature VertexIndex", new Property("VertexIndex", typeof(int)));
      dictProps.Add("Plate Feature Contour Index", new Property("ContourIndex", typeof(int)));

      addElementTypes(dictProps, new List<eObjectType>() {
                      eObjectType.kPlateFeatVertFillet });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_FeatureObject()
    {
      Dictionary<string, Property> dictProps = Build_ActiveConstructionElement();
      dictProps.Add("Feature Is From Fitter", new Property("IsFromFitter", typeof(bool), ".", true));
      dictProps.Add("Feature Coordinate System", new Property("CS", typeof(Matrix3d)));
      dictProps.Add("Feature Use Gap", new Property("UseGap", typeof(bool)));
      dictProps.Add("Feature Object Index", new Property("ObjectIndex", typeof(int)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPlateFeatVertFillet });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_PlateFeatContour()
    {
      Dictionary<string, Property> dictProps = Build_FeatureObject();
      dictProps.Add("Plate Feature Contour Gap", new Property("Gap", typeof(double)));
      dictProps.Add("Plate Feature Contour Length", new Property("Length", typeof(double)));
      dictProps.Add("Plate Feature Contour Length Increment", new Property("LengthIncrement", typeof(double)));
      dictProps.Add("Plate Feature Contour Radius", new Property("Radius", typeof(double)));
      dictProps.Add("Plate Feature Contour Radius Increment", new Property("RadIncrement", typeof(double)));
      dictProps.Add("Plate Feature Contour Width", new Property("Width", typeof(double)));
      dictProps.Add("Plate Feature Contour Boring Out Option", new Property("BoringOut", typeof(int)));

      addElementTypes(dictProps, new List<eObjectType>() {
                      eObjectType.kPlateFeatContour });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_PlateContourNotch()
    {
      Dictionary<string, Property> dictProps = Build_PlateFeatVertex();
      dictProps.Add("Plate Contour Notch Gap", new Property("Gap", typeof(double)));
      dictProps.Add("Plate Contour Notch Length", new Property("Length", typeof(double)));
      dictProps.Add("Plate Contour Notch Length Increment", new Property("LengthIncrement", typeof(double)));
      dictProps.Add("Plate Contour Notch Radius", new Property("Radius", typeof(double)));
      dictProps.Add("Plate Contour Notch Radius Increment", new Property("RadIncrement", typeof(double)));
      dictProps.Add("Plate Contour Notch Width", new Property("Width", typeof(double)));
      dictProps.Add("Plate Contour Notch Boring Out Option", new Property("BoringOut", typeof(int)));
      dictProps.Add("Plate Contour Notch Normal", new Property("Normal", typeof(Vector3d), ".", true));
      dictProps.Add("Plate Contour Notch Straight Cut", new Property("IsStraightCut", typeof(bool)));

      addElementTypes(dictProps, new List<eObjectType>() {
                      eObjectType.kPlateContourNotch });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_BeamMultiContourNotch()
    {
      Dictionary<string, Property> dictProps = Build_FeatureObject();
      dictProps.Add("Beam Multi Contour Notch Gap", new Property("Gap", typeof(double)));
      dictProps.Add("Beam Multi Contour Notch Radius Increment", new Property("RadIncrement", typeof(double)));
      dictProps.Add("Beam Multi Contour Notch BoringOut", new Property("BoringOut", typeof(int)));
      dictProps.Add("Beam Multi Contour Notch Normal", new Property("Normal", typeof(Vector3d), ".", true));
      dictProps.Add("Beam Multi Contour Notch Length", new Property("Length", typeof(double)));
      dictProps.Add("Beam Multi Contour Notch Width", new Property("Width", typeof(double)));
      dictProps.Add("Beam Multi Contour Notch Length Increment", new Property("LengthIncrement", typeof(double)));
      dictProps.Add("Beam Multi Contour Notch Radius", new Property("Radius", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                        eObjectType.kBeamMultiContourNotch });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_BeamShortening()
    {
      Dictionary<string, Property> dictProps = Build_FeatureObject();
      dictProps.Add("Beam Shortening Ins Length", new Property("InsLength", typeof(double)));
      dictProps.Add("Beam Shortening Cut Straight Relative Offset", new Property("CutStraightRelativeOffset", typeof(double)));
      dictProps.Add("Beam Shortening Cut Straight", new Property("CutStraight", typeof(int)));
      dictProps.Add("Beam Shortening Cut Straight Type", new Property("CutStraightType", typeof(int)));
      dictProps.Add("Beam Shortening Angle On Y", new Property("AngleOnY", typeof(double)));
      dictProps.Add("Beam Shortening Cut Straight Offset", new Property("CutStraightOffset", typeof(double)));
      dictProps.Add("Beam Shortening Angle On Z", new Property("AngleOnZ", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                          eObjectType.kBeamShortening });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_BeamNotch()
    {
      Dictionary<string, Property> dictProps = Build_FeatureObject();
      dictProps.Add("Beam Notch Radius", new Property("CornerRadius", typeof(double)));
      dictProps.Add("Beam Notch Depth", new Property("ReferenceDepth", typeof(double)));
      dictProps.Add("Beam Notch Length", new Property("ReferenceLength", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                            eObjectType.kBeamNotch });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_BeamNotchEx()
    {
      Dictionary<string, Property> dictProps = Build_BeamNotch();
      dictProps.Add("Beam Rotate Axis Angle", new Property("AxisAngle", typeof(double)));
      dictProps.Add("Beam Rotate Z Angle", new Property("ZAngle", typeof(double)));
      dictProps.Add("Beam Rotate X Angle", new Property("XAngle", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                            eObjectType.kBeamNotchEx });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Title(string prefix)
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select " + prefix + " Property...", new Property("0_none", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kUnknown });

      return dictProps;
    }

    #endregion

    #region Property Master Definations

    private static Dictionary<string, Property> Build_Master_StraightBeam()
    {
      Dictionary<string, Property> dictProps = Build_Title("Straight Beam").Union(SortDict(Build_Beam())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kStraightBeam });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_BentBeam()
    {
      Dictionary<string, Property> dictProps = Build_Title("Bent Beam").Union(SortDict(Build_BentBeamBase())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBentBeam });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_CompoundStraightBeam()
    {
      Dictionary<string, Property> dictProps = Build_Title("Compound Straight Beam").Union(SortDict(Build_CompoundBeam())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kCompoundStraightBeam });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_PolyBeam()
    {
      Dictionary<string, Property> dictProps = Build_Title("Poly Beam").Union(SortDict(Build_PolyBeam())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPolyBeam });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_BeamTapered()
    {
      Dictionary<string, Property> dictProps = Build_Title("Tapered Straight Beam").Union(SortDict(Build_CompoundBeam())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBeamTapered });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_UnfoldedStraightBeam()
    {
      Dictionary<string, Property> dictProps = Build_Title("Unfolded Beam").Union(SortDict(Build_UnfoldedStraightBeam())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBentBeam });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_ConcreteBentBeam()
    {
      Dictionary<string, Property> dictProps = Build_Title("Concrete Bent Beam").Union(SortDict(Build_BentBeamBase())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kConcreteBentBeam });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_ConcreteBeam()
    {
      Dictionary<string, Property> dictProps = Build_Title("Concrete Straight Beam").Union(SortDict(Build_Beam())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kConcreteBeam });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_FootingIsolated()
    {
      Dictionary<string, Property> dictProps = Build_Title("Isolated Footing").Union(SortDict(Build_Platebase())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kFootingIsolated });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_Slab()
    {
      Dictionary<string, Property> dictProps = Build_Title("Slab").Union(SortDict(Build_Platebase())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kSlab });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_Wall()
    {
      Dictionary<string, Property> dictProps = Build_Title("Wall").Union(SortDict(Build_Wall())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kWall });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_AnchorPattern()
    {
      Dictionary<string, Property> dictProps = Build_Title("Anchor Pattern").Union(SortDict(Build_AnchorPattern())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kAnchorPattern });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_CircleScrewBoltPattern()
    {
      Dictionary<string, Property> dictProps = Build_Title("Bolt Pattern - Circle").Union(SortDict(Build_CircleScrewBoltPattern())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kCircleScrewBoltPattern });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_InfinitMidScrewBoltPattern()
    {
      Dictionary<string, Property> dictProps = Build_Title("Bolt Pattern - Mid").Union(SortDict(Build_CountableScrewBoltPattern())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kInfinitMidScrewBoltPattern });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_Connector()
    {
      Dictionary<string, Property> dictProps = Build_Title("Shearstud").Union(SortDict(Build_Connector())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kConnector });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_WeldLine()
    {
      Dictionary<string, Property> dictProps = Build_Title("Weld Line").Union(SortDict(Build_WeldPattern())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kWeldStraight}); 

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_WeldPoint()
    {
      Dictionary<string, Property> dictProps = Build_Title("Weld Point").Union(SortDict(Build_WeldPattern())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kWeldLevel });

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_ConnectionHolePlate()
    {
      Dictionary<string, Property> dictProps = Build_Title("Connection Holes in Plate").Union(SortDict(Build_ConnectionHolePlate())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kConnectionHolePlate});

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_BeamNotch2Ortho() //Square
    {
      Dictionary<string, Property> dictProps = Build_Title("Beam Square Cope").Union(SortDict(Build_BeamNotch())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBeamNotch2Ortho}); 

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_BeamNotchEx() //Rotated
    {
      Dictionary<string, Property> dictProps = Build_Title("Beam Rotated Cope").Union(SortDict(Build_BeamNotchEx())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBeamNotchEx});

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_BeamShortening()
    {
      Dictionary<string, Property> dictProps = Build_Title("Beam Shortening").Union(SortDict(Build_BeamShortening())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBeamShortening});

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_BeamMultiContourNotch()
    {
      Dictionary<string, Property> dictProps = Build_Title("Beam Multi Contour Notch").Union(SortDict(Build_BeamMultiContourNotch())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBeamMultiContourNotch});

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_PlateFeatContour()
    {
      Dictionary<string, Property> dictProps = Build_Title("Plate Feature").Union(SortDict(Build_PlateFeatContour())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPlateFeatContour});

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_PlateContourNotch()
    {
      Dictionary<string, Property> dictProps = Build_Title("Plate Countour Notch").Union(SortDict(Build_PlateContourNotch())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPlateContourNotch});

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_PlateFeatVertFillet()
    {
      Dictionary<string, Property> dictProps = Build_Title("Plate Vertex Feature").Union(SortDict(Build_PlateFeatVertFillet())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPlateFeatVertFillet});

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_Grating()
    {
      Dictionary<string, Property> dictProps = Build_Title("Grating").Union(SortDict(Build_Grating())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kGrating});

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_Camera()
    {
      Dictionary<string, Property> dictProps = Build_Title("Camera").Union(SortDict(Build_Camera())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kCamera});

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_Grid()
    {
      Dictionary<string, Property> dictProps = Build_Title("Grid").Union(SortDict(Build_Grid())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kGrid});

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_SpecialPart()
    {
      Dictionary<string, Property> dictProps = Build_Title("Special Part").Union(SortDict(Build_SpecialPart())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kSpecialPart});

      return dictProps;
    }

    private static Dictionary<string, Property> Build_Master_Plate()
    {
      

      Dictionary<string, Property> dictProps = Build_Title("Plate").Union(SortDict(Build_Platebase())).ToDictionary(s => s.Key, s => s.Value);
      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kSpecialPart});

      return dictProps;
    }

    private static Dictionary<string, Property> SortDict(Dictionary<string, Property> dataSource)
    {
      return (from pair in dataSource orderby pair.Key ascending select pair).ToDictionary(s => s.Key, s => s.Value);

    }

    #endregion





  }
}
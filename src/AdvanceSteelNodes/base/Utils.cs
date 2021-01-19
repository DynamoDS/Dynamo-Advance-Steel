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

namespace AdvanceSteel.Nodes
{
  [IsVisibleInDynamoLibrary(false)]
  public static class Utils
  {
    private static readonly string separator = "#@§@#";
    //GetDynGrating
    private static Dictionary<FilerObject.eObjectType, Func<string, SteelDbObject>> avaliableSteelObjects = new Dictionary<FilerObject.eObjectType, Func<string, SteelDbObject>>()
    {
      { FilerObject.eObjectType.kStraightBeam, (string handle) => new StraightBeam() },
      { FilerObject.eObjectType.kPolyBeam, (string handle) => new PolyBeam() },
      { FilerObject.eObjectType.kUnfoldedStraightBeam, (string handle) => new UnFoldedBeam() },
      { FilerObject.eObjectType.kCompoundStraightBeam, (string handle) => new CompoundBeam() },
      { FilerObject.eObjectType.kBeamTapered, (string handle) => new TaperedBeam() },
      { FilerObject.eObjectType.kBentBeam, (string handle) => new BentBeam() },
      { FilerObject.eObjectType.kConcreteBentBeam, (string handle) => new ConcreteBentBeam() },
      { FilerObject.eObjectType.kConcreteBeam, (string handle) => new ConcreteStraightBeam() },
      { FilerObject.eObjectType.kFootingIsolated, (string handle) => new Footings() },
      { FilerObject.eObjectType.kSlab, (string handle) => new Slabs() },
      { FilerObject.eObjectType.kWall, (string handle) => new Walls() },
      { FilerObject.eObjectType.kPlate, (string handle) => new Plate() },
      { FilerObject.eObjectType.kGrating, (string handle) => GetDynGrating(handle) },
      { FilerObject.eObjectType.kCamera, (string handle) => new Camera() },
      { FilerObject.eObjectType.kSpecialPart, (string handle) => new SpecialPart() },
      { FilerObject.eObjectType.kAnchorPattern, (string handle) => GetDynAnchor(handle) },
      { FilerObject.eObjectType.kCircleScrewBoltPattern, (string handle) => new CircularBoltPattern() },
      { FilerObject.eObjectType.kConnector, (string handle) => GetDynShearStud(handle) },
      { FilerObject.eObjectType.kInfinitMidScrewBoltPattern, (string handle) => new RectangularBoltPattern() },
      { FilerObject.eObjectType.kWeldStraight, (string handle) => new WeldLine() },
      { FilerObject.eObjectType.kWeldPattern, (string handle) => new WeldPoint() },
      { FilerObject.eObjectType.kBeamNotch2Ortho, (string handle) => new BeamCope() },
      { FilerObject.eObjectType.kBeamShortening, (string handle) => new BeamPlaneCut() },
      { FilerObject.eObjectType.kBeamMultiContourNotch, (string handle) => new BeamPolycut() },
      { FilerObject.eObjectType.kPlateFeatContour, (string handle) => new PlatePolycut() },
      { FilerObject.eObjectType.kPlateFeatVertFillet, (string handle) => new PlateVertexCut() }
    };

    private static readonly Dictionary<Autodesk.AdvanceSteel.CADAccess.FilerObject.eObjectType, string> filterSteelObjects = new Dictionary<Autodesk.AdvanceSteel.CADAccess.FilerObject.eObjectType, string>()
    {
      { FilerObject.eObjectType.kUnknown , "Select Object Type..." },
      { FilerObject.eObjectType.kStraightBeam, "Straight Beam" },
      { FilerObject.eObjectType.kPolyBeam, "Poly Beam" },
      { FilerObject.eObjectType.kUnfoldedStraightBeam, "Unfolded Straight Beam" },
      { FilerObject.eObjectType.kCompoundStraightBeam, "Compound Beam" },
      { FilerObject.eObjectType.kBeamTapered, "Tapered Beam" },
      { FilerObject.eObjectType.kBentBeam, "Bent Beam" },
      { FilerObject.eObjectType.kConcreteBentBeam, "Concrete Bent Beam" },
      { FilerObject.eObjectType.kConcreteBeam, "Concrete Striaght Beam" },
      { FilerObject.eObjectType.kFootingIsolated, "Isolated Footings" },
      { FilerObject.eObjectType.kSlab, "Slabs" },
      { FilerObject.eObjectType.kWall, "Wals" },
      { FilerObject.eObjectType.kPlate, "Plate" },
      { FilerObject.eObjectType.kGrating, "Grating" },
      { FilerObject.eObjectType.kSpecialPart, "Special Part" },
      { FilerObject.eObjectType.kAnchorPattern, "Anchor Pattern" },
      { FilerObject.eObjectType.kCircleScrewBoltPattern, "Circular Bolt Pattern" },
      { FilerObject.eObjectType.kConnector, "Shear Studs" },
      { FilerObject.eObjectType.kInfinitMidScrewBoltPattern, "Rectangular Bolt Pattern" },
      { FilerObject.eObjectType.kWeldStraight, "Weld Line" },
      { FilerObject.eObjectType.kWeldPattern, "Weld Point" },
      { FilerObject.eObjectType.kBeamNotch2Ortho, "Beam Cope" },
      { FilerObject.eObjectType.kBeamShortening, "Beam Shortening" },
      { FilerObject.eObjectType.kBeamMultiContourNotch, "Beam Polycut" },
      { FilerObject.eObjectType.kPlateFeatContour, "Plate Polycut" },
      { FilerObject.eObjectType.kPlateFeatVertFillet, "Plate Corner Cut" },
      { FilerObject.eObjectType.kCamera, "Camera" }
    };

    public static double RadToDegree(double rad)
    {
      return 180.0 * rad / System.Math.PI;
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

    private static SteelDbObject GetDynGrating(string handle)
    {
      SteelDbObject foundSteelObj = null;
      FilerObject obj = Utils.GetObject(handle);
      if (obj != null)
      {
        if (avaliableSteelObjects.ContainsKey(obj.Type()))
        {
          if (obj.Type() == FilerObject.eObjectType.kGrating)
          {
            var gratings = obj as Autodesk.AdvanceSteel.Modelling.Grating;
            switch (gratings.GratingType)
            {
              case Autodesk.AdvanceSteel.Modelling.Grating.eGratingType.kStandard:
                foundSteelObj = new StandardGrating();
                break;
              case Autodesk.AdvanceSteel.Modelling.Grating.eGratingType.kVariable:
                foundSteelObj = new VariableGrating();
                break;
              case Autodesk.AdvanceSteel.Modelling.Grating.eGratingType.kBar:
                foundSteelObj = new BarGrating();
                break;
            }
          }
        }
      }
      return foundSteelObj;
    }

    private static SteelDbObject GetDynShearStud(string handle)
    {
      SteelDbObject foundSteelObj = null;
      FilerObject obj = Utils.GetObject(handle);
      if (obj != null)
      {
        if (avaliableSteelObjects.ContainsKey(obj.Type()))
        {
          if (obj.Type() == FilerObject.eObjectType.kConnector)
          {
            var shearStuds = obj as Autodesk.AdvanceSteel.Modelling.Connector;
            switch (shearStuds.Arranger.Type)
            {
              case Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kCircle:
                foundSteelObj = new CircularShearStudsPattern();
                break;
              case Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kRectangular:
                foundSteelObj = new RectangularShearStudsPattern();
                break;
            }
          }
        }
      }
      return foundSteelObj;
    }

    private static SteelDbObject GetDynAnchor(string handle)
    {
      SteelDbObject foundSteelObj = null;
      FilerObject obj = Utils.GetObject(handle);
      if (obj != null)
      {
        if (avaliableSteelObjects.ContainsKey(obj.Type()))
        {
          if (obj.Type() == FilerObject.eObjectType.kAnchorPattern)
          {
            var anchors = obj as Autodesk.AdvanceSteel.Modelling.AnchorPattern;
            switch (anchors.ArrangerType)
            {
              case Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kCircle:
                break;
              case Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kRectangular:
                break;
            }
          }
        }
      }
      return foundSteelObj;
    }

    public static IEnumerable<SteelDbObject> GetDynObjects(IEnumerable<string> handlesToFind)
    {
      var retListOfSteelObjects = new List<SteelDbObject>();
      using (var ctx = new DocContext())
      {
        foreach (var objHandle in handlesToFind)
        {
          FilerObject obj = Utils.GetObject(objHandle);
          if (obj != null)
          {
            if (avaliableSteelObjects.ContainsKey(obj.Type()))
            {
              SteelDbObject foundSteelObj = avaliableSteelObjects[obj.Type()](objHandle);
              foundSteelObj.Handle = objHandle;
              retListOfSteelObjects.Add(foundSteelObj);
            }
          }
          else
          {
            throw new System.Exception("Object is empty");
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
              if (avaliableSteelObjects.ContainsKey(obj.Type()))
              {
                SteelDbObject foundSteelObj = avaliableSteelObjects[obj.Type()](objHandle);
                foundSteelObj.IsOwnedByDynamo = false;
                foundSteelObj.Handle = objHandle;
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

    public static Dictionary<string, Property> GetBoltProperties()
    {
      return BuildBoltPropertyList();
    }

    public static Dictionary<string, Property> GetConcretePlanarProperties()
    {
      return BuildGenericConcretePropertyList("Concrete ");
    }

    public static Dictionary<string, Property> GetConcreteStraightBeamProperties()
    {
      Dictionary<string, Property> combinedData = BuildStriaghtBeamPropertyList("Concrete ").Union(
                                      BuildGenericBeamPropertyList("Concrete Straight ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, Property> GetUnfoldedStraightBeamProperties()
    {
      Dictionary<string, Property> combinedData = BuildUnfoldedBeamPropertyList("Unfolded ").Union(
                                      BuildGenericBeamPropertyList("Unfolded Straight ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, Property> GetConcreteBentBeamProperties()
    {
      Dictionary<string, Property> combinedData = BuildBentBeamPropertyList("Concrete ").Union(
                                          BuildGenericBeamPropertyList("Concrete Bend ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, Property> GetAnchorBoltPropertyList()
    {
      return BuildAnchorBoltPropertyList();
    }

    public static Dictionary<string, Property> GetShearStudPropertyList()
    {
      return BuildShearStudPropertyList();
    }

    public static Dictionary<string, Property> GetPlatePropertyList()
    {
      return BuildGenericPlatePropertyList();
    }

    public static Dictionary<string, Property> GetCameraPropertyList()
    {
      return BuildCameraPropertyList();
    }

    public static Dictionary<string, Property> GetPlateFeaturePropertyList()
    {
      return BuildPlateFeaturePropertyList();
    }

    public static Dictionary<string, Property> GetBeamCutPlanePropertyList()
    {
      return BuildBeamCutPlanePropertyList();
    }

    public static Dictionary<string, Property> GetBeamMultiNotchPropertyList()
    {
      return BuildBeamMultiNotchPropertyList();
    }

    public static Dictionary<string, Property> GetPlateNotchContourPropertyList()
    {
      return BuildPlateNotchContourPropertyList();
    }

    public static Dictionary<string, Property> GetBeamNotchOrthoPropertyList()
    {
      Dictionary<string, Property> combinedData = BuildBeamNotchOrthoPropertyList().Union(
                                            BuildBaseBeamNotchPropertyList("Othro ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, Property> GetBeamNotchRotatedPropertyList()
    {
      Dictionary<string, Property> combinedData = BuildBeamNotchRotatedPropertyList().Union(
                                            BuildBaseBeamNotchPropertyList("Rotated ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, Property> GetPlateVertexPropertyList()
    {
      return BuildPlateVertexFilletPropertyList();
    }

    public static Dictionary<string, Property> GetGratingPropertyList()
    {
      return BuildGenericGratingPropertyList();
    }

    public static Dictionary<string, Property> GetStraighBeamPropertyList()
    {
      Dictionary<string, Property> combinedData = BuildStriaghtBeamPropertyList().Union(
                                            BuildGenericBeamPropertyList("Straight ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, Property> GetBentBeamPropertyList()
    {
      Dictionary<string, Property> combinedData = BuildBentBeamPropertyList().Union(
                                            BuildGenericBeamPropertyList("Bend ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, Property> GetPolyBeamPropertyList()
    {
      Dictionary<string, Property> combinedData = BuildPolyBeamPropertyList().Union(
                                            BuildGenericBeamPropertyList("Poly ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, Property> GetSpecialPartPropertyList()
    {
      Dictionary<string, Property> combinedData = BuildSpecialPartPropertyList().Union(
                                            BuildGenericSpecialPartPropertyList("Special Part ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, Property> GetTaperBeamPropertyList()
    {
      Dictionary<string, Property> combinedData = BuildTaperedBeamPropertyList().Union(
                                                  BuildCompundBaseBeamPropertyList("Tapered ")).Union(
                                                  BuildGenericBeamPropertyList("Tapered ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, Property> GetCompoundStraightBeamPropertyList()
    {
      Dictionary<string, Property> combinedData = BuildCompoundStraightBeamPropertyList().Union(
                                                  BuildCompundBaseBeamPropertyList("Compound ")).Union(
                                                  BuildGenericBeamPropertyList("Compound ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Property GetProperty(string keyValue)
    {
      Dictionary<string, Property> searchData = CombineAllLists();
      Property retValue = null;
      searchData.TryGetValue(keyValue, out retValue);
      return retValue;
    }

    public static Dictionary<string, Property> GetAllProperties()
    {
      return CombineAllLists();
    }

    private static Dictionary<string, Property> CombineAllLists()
    {
      Dictionary<string, Property> searchData = GetBoltProperties().Union(
                                                  GetAnchorBoltPropertyList()).Union(
                                                  GetShearStudPropertyList()).Union(
                                                  GetPlatePropertyList()).Union(
                                                  GetCameraPropertyList()).Union(
                                                  GetPlateFeaturePropertyList()).Union(
                                                  GetBeamCutPlanePropertyList()).Union(
                                                  GetBeamMultiNotchPropertyList()).Union(
                                                  GetPlateNotchContourPropertyList()).Union(
                                                  GetBeamNotchOrthoPropertyList()).Union(
                                                  GetBeamNotchRotatedPropertyList()).Union(
                                                  GetPlateVertexPropertyList()).Union(
                                                  GetGratingPropertyList()).Union(
                                                  GetStraighBeamPropertyList()).Union(
                                                  GetBentBeamPropertyList()).Union(
                                                  GetPolyBeamPropertyList()).Union(
                                                  GetSpecialPartPropertyList()).Union(
                                                  GetTaperBeamPropertyList()).Union(
                                                  GetConcretePlanarProperties()).Union(
                                                  GetConcreteStraightBeamProperties()).Union(
                                                  GetConcreteBentBeamProperties()).Union(
                                                  GetCompoundStraightBeamPropertyList()).ToDictionary(s => s.Key, s => s.Value);
      return searchData;
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
          dictProps[key].ElementTypeList = elementTypes;
        }
      }
      return dictProps;
    }

    private static Dictionary<string, Property> BuildGenericConcretePropertyList(string prefix = "")
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select " + prefix + "Property...", new Property("none", typeof(string)));
      dictProps.Add(prefix + "Approval Comment", new Property("ApprovalComment", typeof(string)));
      dictProps.Add(prefix + "Approval Status Code", new Property("ApprovalStatusCode", typeof(string)));
      dictProps.Add(prefix + "Assembly", new Property("Assembly", typeof(string)));
      dictProps.Add(prefix + "Assembly Used For Numbering", new Property("AssemblyUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Center Point", new Property("CenterPoint", typeof(Point3d), ".", true));
      dictProps.Add(prefix + "Carrier", new Property("Carrier", typeof(string)));
      dictProps.Add(prefix + "Coating", new Property("Coating", typeof(string)));
      dictProps.Add(prefix + "Coating Description", new Property("CoatingDescription", typeof(string), ".", true));
      dictProps.Add(prefix + "Coating Used For Numbering", new Property("CoatingUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Delivery Date", new Property("DeliveryDate", typeof(string)));
      dictProps.Add(prefix + "Denotation Used For Numbering", new Property("DennotationUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Denotation", new Property("Denotation", typeof(string)));
      dictProps.Add(prefix + "Explicit Quantity", new Property("ExplicitQuantity", typeof(int)));
      dictProps.Add(prefix + "Fabrication Station", new Property("FabricationStation", typeof(string)));
      dictProps.Add(prefix + "Fabrication Station UsedF or Numbering", new Property("FabricationStationUsedForNumbering", typeof(bool)));
      dictProps.Add(prefix + "Handle", new Property("Handle", typeof(string), ".", true));
      dictProps.Add(prefix + "Heat Number", new Property("HeatNumber", typeof(string)));
      dictProps.Add(prefix + "Heat Number Used For Numbering", new Property("HeatNumberUsedForNumbering", typeof(bool)));
      dictProps.Add(prefix + "Height", new Property("Height", typeof(double), ".", true));
      dictProps.Add(prefix + "Holes Used For Numbering", new Property("HolesUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Set IsAttached Flag", new Property("IsAttachedPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add(prefix + "Set IsMainPart Flag", new Property("IsMainPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add(prefix + "ItemNumber", new Property("ItemNumber", typeof(string)));
      dictProps.Add(prefix + "ItemNumber Used For Numbering", new Property("ItemNumberUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Layer", new Property("Layer", typeof(string)));
      dictProps.Add(prefix + "Length", new Property("Length", typeof(double), ".", true));
      dictProps.Add(prefix + "Load Number", new Property("LoadNumber", typeof(string)));
      dictProps.Add(prefix + "MainPart Number", new Property("MainPartNumber", typeof(string)));
      dictProps.Add(prefix + "MainPart Number Prefix", new Property("MainPartPrefix", typeof(string)));
      dictProps.Add(prefix + "MainPart Used For BOM", new Property("MainPartUsedForBOM", typeof(int)));
      dictProps.Add(prefix + "MainPart Used For Collision Check", new Property("MainPartUsedForCollisionCheck", typeof(int)));
      dictProps.Add(prefix + "MainPart Used For Numbering", new Property("MainPartUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Material", new Property("Material", typeof(string)));
      dictProps.Add(prefix + "Material Description", new Property("MaterialDescription", typeof(string), ".", true));
      dictProps.Add(prefix + "Material Used For Numbering", new Property("MaterialUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Note", new Property("Note", typeof(string)));
      dictProps.Add(prefix + "Note Used For Numbering", new Property("NoteUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Number Of Holes", new Property("NumberOfHoles", typeof(int), ".", true));
      dictProps.Add(prefix + "PONumber", new Property("PONumber", typeof(string)));
      dictProps.Add(prefix + "PONumber Used For Numbering", new Property("PONumberUsedForNumbering", typeof(bool)));
      dictProps.Add(prefix + "Face Alignment", new Property("Portioning", typeof(double)));
      dictProps.Add(prefix + "Preliminary Part Number", new Property("PreliminaryPartNumber", typeof(string)));
      dictProps.Add(prefix + "Preliminary Part Position Number", new Property("PreliminaryPartPositionNumber", typeof(string)));
      dictProps.Add(prefix + "Preliminary Part Prefix", new Property("PreliminaryPartPrefix", typeof(string)));
      dictProps.Add(prefix + "Radius Increment", new Property("RadIncrement", typeof(double)));
      dictProps.Add(prefix + "Requisition Number", new Property("RequisitionNumber", typeof(string)));
      dictProps.Add(prefix + "Requisition Number Used For Numbering", new Property("RequisitionNumberUsedForNumbering", typeof(bool)));
      dictProps.Add(prefix + "Model Role", new Property("Role", typeof(string)));
      dictProps.Add(prefix + "Model Role Description", new Property("RoleDescription", typeof(string), ".", true));
      dictProps.Add(prefix + "Role Used For Numbering", new Property("RoleUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Shipped Date", new Property("ShippedDate", typeof(string)));
      dictProps.Add(prefix + "Single Part Number", new Property("SinglePartNumber", typeof(string)));
      dictProps.Add(prefix + "Single Part Prefix", new Property("SinglePartPrefix", typeof(string)));
      dictProps.Add(prefix + "Single Part Used For BOM", new Property("SinglePartUsedForBOM", typeof(int)));
      dictProps.Add(prefix + "Single Part Used For CollisionCheck", new Property("SinglePartUsedForCollisionCheck", typeof(int)));
      dictProps.Add(prefix + "Single Part Used For Numbering", new Property("SinglePartUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "SpecificGravity", new Property("SpecificGravity", typeof(double), ".", true));
      dictProps.Add(prefix + "Supplier", new Property("Supplier", typeof(string)));
      dictProps.Add(prefix + "SupplierUsedForNumbering", new Property("SupplierUsedForNumbering", typeof(bool)));
      dictProps.Add(prefix + "Thickness", new Property("Thickness", typeof(double)));
      dictProps.Add(prefix + "Volume", new Property("Volume", typeof(double), ".", true));
      dictProps.Add(prefix + "Width", new Property("Width", typeof(double), ".", true));
      dictProps.Add("Change " + prefix + "Display Mode", new Property("ReprMode", typeof(int), "Z_PostWriteDB"));


      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kWall,
                    eObjectType.kFootingIsolated,
                    eObjectType.kConcreteBeam,
                    eObjectType.kConcreteBentBeam,
                    eObjectType.kSlab});

      return dictProps;
    }

    private static Dictionary<string, Property> BuildBoltPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Bolt Property...", new Property("none", typeof(string)));
      dictProps.Add("Bolt Standard", new Property("Standard", typeof(string)));
      dictProps.Add("Bolt Assembly", new Property("BoltAssembly", typeof(string)));
      dictProps.Add("Bolt Grade", new Property("Grade", typeof(string)));
      dictProps.Add("Bolt Diameter", new Property("ScrewDiameter", typeof(double)));
      dictProps.Add("Bolt Hole Tolerance", new Property("HoleTolerance", typeof(double)));
      dictProps.Add("No of Bolts Circle", new Property("NumberOfScrews", typeof(int)));
      dictProps.Add("Bolt X Count", new Property("Nx", typeof(int)));
      dictProps.Add("Bolt Y Count", new Property("Ny", typeof(int)));
      dictProps.Add("Bolt X Spacing", new Property("Dx", typeof(double)));
      dictProps.Add("Bolt Y Spacing", new Property("Dy", typeof(double)));
      dictProps.Add("Bolt Pattern Radius", new Property("Radius", typeof(double)));
      dictProps.Add("Bolt Length Addition", new Property("BindingLengthAddition", typeof(double)));
      dictProps.Add("Bolt Inverted", new Property("IsInverted", typeof(bool)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kInfinitMidScrewBoltPattern,
                    eObjectType.kCircleScrewBoltPattern });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildAnchorBoltPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Anchor Property...", new Property("none", typeof(string)));
      dictProps.Add("Anchor Standard", new Property("Standard", typeof(string)));
      dictProps.Add("Anchor Assembly", new Property("BoltAssembly", typeof(string)));
      dictProps.Add("Anchor Grade", new Property("Grade", typeof(string)));
      dictProps.Add("Anchor Length", new Property("ScrewLength", typeof(double)));
      dictProps.Add("Anchor Diameter", new Property("ScrewDiameter", typeof(double)));
      dictProps.Add("Anchor Hole Tolerance", new Property("HoleTolerance", typeof(double)));
      dictProps.Add("No of Anchor Circle", new Property("NumberOfScrews", typeof(int)));
      dictProps.Add("Anchor X Count", new Property("Nx", typeof(int)));
      dictProps.Add("Anchor Y Count", new Property("Ny", typeof(int)));
      dictProps.Add("Anchor X Spacing", new Property("Dx", typeof(double)));
      dictProps.Add("Anchor Y Spacing", new Property("Dy", typeof(double)));
      dictProps.Add("Anchor Pattern Radius", new Property("Radius", typeof(double)));
      dictProps.Add("Anchor Inverted", new Property("IsInverted", typeof(bool)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kAnchorPattern });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildShearStudPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Stud Property...", new Property("none", typeof(string)));
      dictProps.Add("Stud Standard", new Property("Standard", typeof(string)));
      dictProps.Add("Stud Grade", new Property("Grade", typeof(string)));
      dictProps.Add("Stud Length", new Property("Length", typeof(double)));
      dictProps.Add("Stud Diameter", new Property("Diameter", typeof(double)));
      dictProps.Add("Stud Hole Tolerance", new Property("HoleTolerance", typeof(double)));
      dictProps.Add("No of Shear Studs Circle", new Property("NumberOfElements", typeof(int), "Arranger"));
      dictProps.Add("Shear Stud Radius", new Property("Radius", typeof(double), "Arranger"));
      dictProps.Add("Stud X Count", new Property("Nx", typeof(int), "Arranger"));
      dictProps.Add("Stud Y Count", new Property("Ny", typeof(int), "Arranger"));
      dictProps.Add("Stud X Spacing", new Property("Dx", typeof(double), "Arranger"));
      dictProps.Add("Stud Y Spacing", new Property("Dy", typeof(double), "Arranger"));
      dictProps.Add("Display Stud As Solid", new Property("ReprMode", typeof(int), "Z_PostWriteDB"));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kConnector });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildSpecialPartPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Special Part Property...", new Property("none", typeof(string)));
      dictProps.Add("Special Part BlockName", new Property("BlockName", typeof(string), ".", true));
      dictProps.Add("Special Part Depth", new Property("Depth", typeof(double), ".", true));
      dictProps.Add("Special Part Width", new Property("Width", typeof(double), ".", true));
      dictProps.Add("Special Part Length", new Property("Length", typeof(double), ".", true));
      dictProps.Add("Special Part PaintArea", new Property("PaintArea", typeof(double)));
      dictProps.Add("Special Part Scale", new Property("Scale", typeof(double)));
      dictProps.Add("Special Part Weight", new Property("Weight", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kSpecialPart });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildGenericSpecialPartPropertyList(string prefix = "")
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add(prefix + "Assembly", new Property("Assembly", typeof(string)));
      dictProps.Add(prefix + "Assembly Used For Numbering", new Property("AssemblyUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Center Point", new Property("CenterPoint", typeof(Point3d), ".", true));
      dictProps.Add(prefix + "Coating", new Property("Coating", typeof(string)));
      dictProps.Add(prefix + "Coating Description", new Property("CoatingDescription", typeof(string), ".", true));
      dictProps.Add(prefix + "Coating Used For Numbering", new Property("CoatingUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Denotation Used For Numbering", new Property("DennotationUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Denotation Role", new Property("Denotation", typeof(string)));
      dictProps.Add(prefix + "Explicit Quantity", new Property("ExplicitQuantity", typeof(int)));
      dictProps.Add(prefix + "Handle", new Property("Handle", typeof(string), ".", true));
      dictProps.Add(prefix + "Holes Used For Numbering", new Property("HolesUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Set IsMainPart Flag", new Property("IsMainPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add(prefix + "Get IsAttachedPart Flag", new Property("IsAttachedPart", typeof(bool), ".", true));
      dictProps.Add(prefix + "ItemNumber", new Property("ItemNumber", typeof(string)));
      dictProps.Add(prefix + "ItemNumber Used For Numbering", new Property("ItemNumberUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Layer", new Property("Layer", typeof(string)));
      dictProps.Add(prefix + "MainPart Number", new Property("MainPartNumber", typeof(string)));
      dictProps.Add(prefix + "MainPart Number Prefix", new Property("MainPartPrefix", typeof(string)));
      dictProps.Add(prefix + "MainPart Used For BOM", new Property("MainPartUsedForBOM", typeof(int)));
      dictProps.Add(prefix + "MainPart Used For Collision Check", new Property("MainPartUsedForCollisionCheck", typeof(int)));
      dictProps.Add(prefix + "MainPart Used For Numbering", new Property("MainPartUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Material", new Property("Material", typeof(string)));
      dictProps.Add(prefix + "Material Description", new Property("MaterialDescription", typeof(string), ".", true));
      dictProps.Add(prefix + "Material Used For Numbering", new Property("MaterialUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Note", new Property("Note", typeof(string)));
      dictProps.Add(prefix + "Note Used For Numbering", new Property("NoteUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Number Of Holes", new Property("NumberOfHoles", typeof(int), ".", true));
      dictProps.Add(prefix + "Preliminary Part Number", new Property("PreliminaryPartNumber", typeof(string)));
      dictProps.Add(prefix + "Preliminary Part Position Number", new Property("PreliminaryPartPositionNumber", typeof(string), ".", true));
      dictProps.Add(prefix + "Preliminary Part Prefix", new Property("PreliminaryPartPrefix", typeof(string)));
      dictProps.Add(prefix + "Model Role", new Property("Role", typeof(string)));
      dictProps.Add(prefix + "Model Role Description", new Property("RoleDescription", typeof(string)));
      dictProps.Add(prefix + "Role Used For Numbering", new Property("RoleUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Runname", new Property("Runname", typeof(string), ".", true));
      dictProps.Add(prefix + "Single Part Number", new Property("SinglePartNumber", typeof(string)));
      dictProps.Add(prefix + "Single Part Prefix", new Property("SinglePartPrefix", typeof(string)));
      dictProps.Add(prefix + "Single Part Used For BOM", new Property("SinglePartUsedForBOM", typeof(int)));
      dictProps.Add(prefix + "Single Part Used For CollisionCheck", new Property("SinglePartUsedForCollisionCheck", typeof(int)));
      dictProps.Add(prefix + "Single Part Used For Numbering", new Property("SinglePartUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Specific Gravity", new Property("SpecificGravity", typeof(double), ".", true));
      dictProps.Add(prefix + "Structural Member", new Property("StructuralMember", typeof(int)));
      dictProps.Add(prefix + "Unwind / Unfolder", new Property("Unwind", typeof(bool)));
      dictProps.Add(prefix + "UnwindStartFactor", new Property("UnwindStartFactor", typeof(double)));
      dictProps.Add(prefix + "Volume", new Property("Volume", typeof(double), ".", true));
      dictProps.Add(prefix + "Change Display Mode", new Property("ReprMode", typeof(int), "Z_PostWriteDB"));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kSpecialPart});

      return dictProps;
    }

    private static Dictionary<string, Property> BuildGenericBeamPropertyList(string prefix = "")
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add(prefix + "Beam Angle", new Property("Angle", typeof(double)));
      dictProps.Add(prefix + "Beam Approval Comment", new Property("ApprovalComment", typeof(string)));
      dictProps.Add(prefix + "Beam Approval Status Code", new Property("ApprovalStatusCode", typeof(string)));
      dictProps.Add(prefix + "Beam Assembly", new Property("Assembly", typeof(string)));
      dictProps.Add(prefix + "Beam Assembly Used For Numbering", new Property("AssemblyUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Beam Center Point", new Property("CenterPoint", typeof(Point3d), ".", true));
      dictProps.Add(prefix + "Beam Carrier", new Property("Carrier", typeof(string)));
      dictProps.Add(prefix + "Beam Coating", new Property("Coating", typeof(string)));
      dictProps.Add(prefix + "Beam Coating Description", new Property("CoatingDescription", typeof(string), ".", true));
      dictProps.Add(prefix + "Beam Coating Used For Numbering", new Property("CoatingUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Beam Delivery Date", new Property("DeliveryDate", typeof(string)));
      dictProps.Add(prefix + "Beam Denotation Used For Numbering", new Property("DennotationUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Beam Denotation Role", new Property("Denotation", typeof(string)));
      dictProps.Add(prefix + "Beam Deviation", new Property("Deviation", typeof(double)));
      dictProps.Add(prefix + "Beam Explicit Quantity", new Property("ExplicitQuantity", typeof(int)));
      dictProps.Add(prefix + "Beam Fabrication Station", new Property("FabricationStation", typeof(string)));
      dictProps.Add(prefix + "Beam Fabrication Station UsedF or Numbering", new Property("FabricationStationUsedForNumbering", typeof(bool)));
      dictProps.Add(prefix + "Beam Handle", new Property("Handle", typeof(string), ".", true));
      dictProps.Add(prefix + "Beam Heat Number", new Property("HeatNumber", typeof(string)));
      dictProps.Add(prefix + "Beam Heat Number Used For Numbering", new Property("HeatNumberUsedForNumbering", typeof(bool)));
      dictProps.Add(prefix + "Beam Holes Used For Numbering", new Property("HolesUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Beam Set IsMainPart Flag", new Property("IsMainPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add(prefix + "Beam Get IsAttachedPart Flag", new Property("IsAttachedPart", typeof(bool), ".", true));
      dictProps.Add(prefix + "Beam Get IsCrossSectionMirrored Flag", new Property("IsCrossSectionMirrored", typeof(bool), ".", true));
      dictProps.Add(prefix + "Beam ItemNumber", new Property("ItemNumber", typeof(string)));
      dictProps.Add(prefix + "Beam ItemNumber Used For Numbering", new Property("ItemNumberUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Beam Layer", new Property("Layer", typeof(string)));
      dictProps.Add(prefix + "Beam Load Number", new Property("LoadNumber", typeof(string)));
      dictProps.Add(prefix + "Beam MainPart Number", new Property("MainPartNumber", typeof(string)));
      dictProps.Add(prefix + "Beam MainPart Number Prefix", new Property("MainPartPrefix", typeof(string)));
      dictProps.Add(prefix + "Beam MainPart Used For BOM", new Property("MainPartUsedForBOM", typeof(int)));
      dictProps.Add(prefix + "Beam MainPart Used For Collision Check", new Property("MainPartUsedForCollisionCheck", typeof(int)));
      dictProps.Add(prefix + "Beam MainPart Used For Numbering", new Property("MainPartUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Beam Material", new Property("Material", typeof(string)));
      dictProps.Add(prefix + "Beam Material Description", new Property("MaterialDescription", typeof(string), ".", true));
      dictProps.Add(prefix + "Beam Material Used For Numbering", new Property("MaterialUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Beam Note", new Property("Note", typeof(string)));
      dictProps.Add(prefix + "Beam Note Used For Numbering", new Property("NoteUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Beam Number Of Holes", new Property("NumberOfHoles", typeof(int), ".", true));
      dictProps.Add(prefix + "Beam PONumber", new Property("PONumber", typeof(string)));
      dictProps.Add(prefix + "Beam PONumber Used For Numbering", new Property("PONumberUsedForNumbering", typeof(bool)));
      dictProps.Add(prefix + "Beam Preliminary Part Number", new Property("PreliminaryPartNumber", typeof(string)));
      dictProps.Add(prefix + "Beam Preliminary Part Position Number", new Property("PreliminaryPartPositionNumber", typeof(string), ".", true));
      dictProps.Add(prefix + "Beam Preliminary Part Prefix", new Property("PreliminaryPartPrefix", typeof(string)));
      dictProps.Add(prefix + "Beam Profile Name", new Property("ProfName", typeof(string)));
      dictProps.Add(prefix + "Beam Profile Section Type", new Property("ProfSectionType", typeof(string), ".", true));
      dictProps.Add(prefix + "Beam Profile Section name", new Property("ProfSectionName", typeof(string), ".", true));
      dictProps.Add(prefix + "Beam Requisition Number", new Property("RequisitionNumber", typeof(string)));
      dictProps.Add(prefix + "Beam Requisition Number Used For Numbering", new Property("RequisitionNumberUsedForNumbering", typeof(bool)));
      dictProps.Add(prefix + "Beam Model Role", new Property("Role", typeof(string)));
      dictProps.Add(prefix + "Beam Model Role Description", new Property("RoleDescription", typeof(string)));
      dictProps.Add(prefix + "Beam Role Used For Numbering", new Property("RoleUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Beam Runname", new Property("Runname", typeof(string), ".", true));
      dictProps.Add(prefix + "Beam Shipped Date", new Property("ShippedDate", typeof(string)));
      dictProps.Add(prefix + "Beam ShrinkValue", new Property("ShrinkValue", typeof(double), ".", true));
      dictProps.Add(prefix + "Beam Single Part Number", new Property("SinglePartNumber", typeof(string)));
      dictProps.Add(prefix + "Beam Single Part Prefix", new Property("SinglePartPrefix", typeof(string)));
      dictProps.Add(prefix + "Beam Single Part Used For BOM", new Property("SinglePartUsedForBOM", typeof(int)));
      dictProps.Add(prefix + "Beam Single Part Used For CollisionCheck", new Property("SinglePartUsedForCollisionCheck", typeof(int)));
      dictProps.Add(prefix + "Beam Single Part Used For Numbering", new Property("SinglePartUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Beam Specific Gravity", new Property("SpecificGravity", typeof(double), ".", true));
      dictProps.Add(prefix + "Beam Structural Member", new Property("StructuralMember", typeof(int)));
      dictProps.Add(prefix + "Beam System Line Length", new Property("SysLength", typeof(double), ".", true));
      dictProps.Add(prefix + "Beam Supplier", new Property("Supplier", typeof(string)));
      dictProps.Add(prefix + "Beam SupplierUsedForNumbering", new Property("SupplierUsedForNumbering", typeof(bool)));
      dictProps.Add(prefix + "Beam Unwind / Unfolder", new Property("Unwind", typeof(bool)));
      dictProps.Add(prefix + "Beam UnwindStartFactor", new Property("UnwindStartFactor", typeof(double)));
      dictProps.Add(prefix + "Beam Volume", new Property("Volume", typeof(double), ".", true));
      dictProps.Add(prefix + "Change Beam Display Mode", new Property("ReprMode", typeof(int), "Z_PostWriteDB"));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kStraightBeam,
                    eObjectType.kConcreteBeam,
                    eObjectType.kBeamTapered,
                    eObjectType.kBentBeam,
                    eObjectType.kPolyBeam,
                    eObjectType.kUnfoldedStraightBeam});

      return dictProps;
    }

    private static Dictionary<string, Property> BuildGenericPlatePropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Plate Property...", new Property("none", typeof(string)));
      dictProps.Add("Plate Approval Comment", new Property("ApprovalComment", typeof(string)));
      dictProps.Add("Plate Approval Status Code", new Property("ApprovalStatusCode", typeof(string)));
      dictProps.Add("Plate Assembly", new Property("Assembly", typeof(string)));
      dictProps.Add("Plate Assembly Used For Numbering", new Property("AssemblyUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Center Point", new Property("CenterPoint", typeof(Point3d), ".", true));
      dictProps.Add("Plate Carrier", new Property("Carrier", typeof(string)));
      dictProps.Add("Plate Coating", new Property("Coating", typeof(string)));
      dictProps.Add("Plate Coating Description", new Property("CoatingDescription", typeof(string), ".", true));
      dictProps.Add("Plate Coating Used For Numbering", new Property("CoatingUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Delivery Date", new Property("DeliveryDate", typeof(string)));
      dictProps.Add("Plate Denotation Used For Numbering", new Property("DennotationUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Denotation", new Property("Denotation", typeof(string)));
      dictProps.Add("Plate Explicit Quantity", new Property("ExplicitQuantity", typeof(int)));
      dictProps.Add("Plate Fabrication Station", new Property("FabricationStation", typeof(string)));
      dictProps.Add("Plate Fabrication Station UsedF or Numbering", new Property("FabricationStationUsedForNumbering", typeof(bool)));
      dictProps.Add("Plate Handle", new Property("Handle", typeof(string), ".", true));
      dictProps.Add("Plate Heat Number", new Property("HeatNumber", typeof(string)));
      dictProps.Add("Plate Heat Number Used For Numbering", new Property("HeatNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Plate Holes Used For Numbering", new Property("HolesUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Set IsAttached Flag", new Property("IsAttachedPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add("Plate Set IsMainPart Flag", new Property("IsMainPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add("Plate ItemNumber", new Property("ItemNumber", typeof(string)));
      dictProps.Add("Plate ItemNumber Used For Numbering", new Property("ItemNumberUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Layer", new Property("Layer", typeof(string)));
      dictProps.Add("Plate Length", new Property("Length", typeof(double), ".", true));
      dictProps.Add("Plate Load Number", new Property("LoadNumber", typeof(string)));
      dictProps.Add("Plate MainPart Number", new Property("MainPartNumber", typeof(string)));
      dictProps.Add("Plate MainPart Number Prefix", new Property("MainPartPrefix", typeof(string)));
      dictProps.Add("Plate MainPart Used For BOM", new Property("MainPartUsedForBOM", typeof(int)));
      dictProps.Add("Plate MainPart Used For Collision Check", new Property("MainPartUsedForCollisionCheck", typeof(int)));
      dictProps.Add("Plate MainPart Used For Numbering", new Property("MainPartUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Material", new Property("Material", typeof(string)));
      dictProps.Add("Plate Material Description", new Property("MaterialDescription", typeof(string), ".", true));
      dictProps.Add("Plate Material Used For Numbering", new Property("MaterialUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Note", new Property("Note", typeof(string)));
      dictProps.Add("Plate Note Used For Numbering", new Property("NoteUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Number Of Holes", new Property("NumberOfHoles", typeof(int), ".", true));
      dictProps.Add("Plate PONumber", new Property("PONumber", typeof(string)));
      dictProps.Add("Plate PONumber Used For Numbering", new Property("PONumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Plate Face Alignment", new Property("Portioning", typeof(double)));
      dictProps.Add("Plate Preliminary Part Number", new Property("PreliminaryPartNumber", typeof(string)));
      dictProps.Add("Plate Preliminary Part Position Number", new Property("PreliminaryPartPositionNumber", typeof(string)));
      dictProps.Add("Plate Preliminary Part Prefix", new Property("PreliminaryPartPrefix", typeof(string)));
      dictProps.Add("Plate Radius Increment", new Property("RadIncrement", typeof(double)));
      dictProps.Add("Plate Radius", new Property("Radius", typeof(double), ".", true));
      dictProps.Add("Plate Requisition Number", new Property("RequisitionNumber", typeof(string)));
      dictProps.Add("Plate Requisition Number Used For Numbering", new Property("RequisitionNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Plate Model Role", new Property("Role", typeof(string)));
      dictProps.Add("Plate Model Role Description", new Property("RoleDescription", typeof(string), ".", true));
      dictProps.Add("Plate Role Used For Numbering", new Property("RoleUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Shipped Date", new Property("ShippedDate", typeof(string)));
      dictProps.Add("Plate Single Part Number", new Property("SinglePartNumber", typeof(string)));
      dictProps.Add("Plate Single Part Prefix", new Property("SinglePartPrefix", typeof(string)));
      dictProps.Add("Plate Single Part Used For BOM", new Property("SinglePartUsedForBOM", typeof(int)));
      dictProps.Add("Plate Single Part Used For CollisionCheck", new Property("SinglePartUsedForCollisionCheck", typeof(int)));
      dictProps.Add("Plate Single Part Used For Numbering", new Property("SinglePartUsedForNumbering", typeof(int)));
      dictProps.Add("Plate SpecificGravity", new Property("SpecificGravity", typeof(double), ".", true));
      dictProps.Add("Plate Supplier", new Property("Supplier", typeof(string)));
      dictProps.Add("Plate SupplierUsedForNumbering", new Property("SupplierUsedForNumbering", typeof(bool)));
      dictProps.Add("Plate Thickness", new Property("Thickness", typeof(double)));
      dictProps.Add("Plate Volume", new Property("Volume", typeof(double), ".", true));
      dictProps.Add("Plate Width", new Property("Width", typeof(double), ".", true));
      dictProps.Add("Change Plate Display Mode", new Property("ReprMode", typeof(int), "Z_PostWriteDB"));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPlate });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildGenericGratingPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Grating Property...", new Property("none", typeof(string)));
      dictProps.Add("Grating Approval Comment", new Property("ApprovalComment", typeof(string)));
      dictProps.Add("Grating Approval Status Code", new Property("ApprovalStatusCode", typeof(string)));
      dictProps.Add("Grating Assembly", new Property("Assembly", typeof(string)));
      dictProps.Add("Grating Assembly Used For Numbering", new Property("AssemblyUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Center Point", new Property("CenterPoint", typeof(Point3d), ".", true));
      dictProps.Add("Grating Bearing Bar Quantity", new Property("BearingBarQuantity", typeof(int), ".", true));
      dictProps.Add("Grating Bearing Bar Spacing", new Property("BearingBarSpacing", typeof(int), ".", true));
      dictProps.Add("Grating Bearing Bar Spacing Distance", new Property("BearingBarSpacingDistance", typeof(double), ".", true));
      dictProps.Add("Grating Carrier", new Property("Carrier", typeof(string)));
      dictProps.Add("Grating Coating", new Property("Coating", typeof(string)));
      dictProps.Add("Grating Coating Description", new Property("CoatingDescription", typeof(string), ".", true));
      dictProps.Add("Grating Coating Used For Numbering", new Property("CoatingUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Connector Key", new Property("ConnectorKey", typeof(int)));
      dictProps.Add("Grating Connector Name", new Property("ConnectorName", typeof(string)));
      dictProps.Add("Grating Connector Quantity", new Property("ConnectorQuantity", typeof(int)));
      dictProps.Add("Grating Cross Bar", new Property("CrossBar", typeof(string)));
      dictProps.Add("Grating Cross Bar Quantity", new Property("CrossBarQuantity", typeof(int), ".", true));
      dictProps.Add("Grating Cross Bar Spacing", new Property("CrossBarSpacing", typeof(int), ".", true));
      dictProps.Add("Grating Cross Bar Spacing Distance", new Property("CrossBarSpacingDistance", typeof(double), ".", true));
      dictProps.Add("Grating Custom Hatch", new Property("CustomHatch", typeof(string)));
      dictProps.Add("Grating EDValue", new Property("EDValue", typeof(double)));
      dictProps.Add("Grating Direction", new Property("Direction", typeof(Vector3d)));
      dictProps.Add("Grating Delivery Date", new Property("DeliveryDate", typeof(string)));
      dictProps.Add("Grating Denotation Used For Numbering", new Property("DennotationUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Denotation", new Property("Denotation", typeof(string)));
      dictProps.Add("Grating Explicit Quantity", new Property("ExplicitQuantity", typeof(int)));
      dictProps.Add("Grating Fabrication Station", new Property("FabricationStation", typeof(string)));
      dictProps.Add("Grating Fabrication Station UsedF or Numbering", new Property("FabricationStationUsedForNumbering", typeof(bool)));
      dictProps.Add("Grating Class", new Property("GratingClass", typeof(string)));
      dictProps.Add("Grating Size", new Property("GratingSize", typeof(string)));
      dictProps.Add("Grating Handle", new Property("Handle", typeof(string), ".", true));
      dictProps.Add("Grating Heat Number", new Property("HeatNumber", typeof(string)));
      dictProps.Add("Grating Heat Number Used For Numbering", new Property("HeatNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Grating Holes Used For Numbering", new Property("HolesUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Set IsAttached Flag", new Property("IsAttachedPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add("Grating Set IsMainPart Flag", new Property("IsMainPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add("Grating Set IsMatCoating Database Defined", new Property("IsMatCoatDbDefined", typeof(bool)));
      dictProps.Add("Grating Set IsUsing Standard ED Value", new Property("IsUsingStandardED", typeof(bool)));
      dictProps.Add("Grating Set IsUsing Standard Hatch Value", new Property("IsUsingStandardHatch", typeof(bool)));
      dictProps.Add("Grating ItemNumber", new Property("ItemNumber", typeof(string)));
      dictProps.Add("Grating ItemNumber Used For Numbering", new Property("ItemNumberUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Layer", new Property("Layer", typeof(string)));
      dictProps.Add("Grating Length", new Property("Length", typeof(double), ".", true));
      dictProps.Add("Grating Length Increment", new Property("LengthIncrement", typeof(double)));
      dictProps.Add("Grating Load Number", new Property("LoadNumber", typeof(string)));
      dictProps.Add("Grating MainPart Number", new Property("MainPartNumber", typeof(string)));
      dictProps.Add("Grating MainPart Number Prefix", new Property("MainPartPrefix", typeof(string)));
      dictProps.Add("Grating MainPart Used For BOM", new Property("MainPartUsedForBOM", typeof(int)));
      dictProps.Add("Grating MainPart Used For Collision Check", new Property("MainPartUsedForCollisionCheck", typeof(int)));
      dictProps.Add("Grating MainPart Used For Numbering", new Property("MainPartUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Material", new Property("Material", typeof(string)));
      dictProps.Add("Grating Material Description", new Property("MaterialDescription", typeof(string), ".", true));
      dictProps.Add("Grating Material Used For Numbering", new Property("MaterialUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Note", new Property("Note", typeof(string)));
      dictProps.Add("Grating Note Used For Numbering", new Property("NoteUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Number Of Holes", new Property("NumberOfHoles", typeof(int), ".", true));
      dictProps.Add("Grating Normal", new Property("PlateNormal", typeof(Vector3d), ".", true));
      dictProps.Add("Grating OED Value", new Property("OEDValue", typeof(double)));
      dictProps.Add("Grating PONumber", new Property("PONumber", typeof(string)));
      dictProps.Add("Grating PONumber Used For Numbering", new Property("PONumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Grating Face Alignment", new Property("Portioning", typeof(double)));
      dictProps.Add("Grating Preliminary Part Number", new Property("PreliminaryPartNumber", typeof(string)));
      dictProps.Add("Grating Preliminary Part Position Number", new Property("PreliminaryPartPositionNumber", typeof(string)));
      dictProps.Add("Grating Preliminary Part Prefix", new Property("PreliminaryPartPrefix", typeof(string)));
      dictProps.Add("Grating Radius Increment", new Property("RadIncrement", typeof(double)));
      dictProps.Add("Grating Radius", new Property("Radius", typeof(double), ".", true));
      dictProps.Add("Grating Requisition Number", new Property("RequisitionNumber", typeof(string)));
      dictProps.Add("Grating Requisition Number Used For Numbering", new Property("RequisitionNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Grating Model Role", new Property("Role", typeof(string)));
      dictProps.Add("Grating Model Role Description", new Property("RoleDescription", typeof(string), ".", true));
      dictProps.Add("Grating Role Used For Numbering", new Property("RoleUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Shipped Date", new Property("ShippedDate", typeof(string)));
      dictProps.Add("Grating Single Part Number", new Property("SinglePartNumber", typeof(string)));
      dictProps.Add("Grating Single Part Prefix", new Property("SinglePartPrefix", typeof(string)));
      dictProps.Add("Grating Single Part Used For BOM", new Property("SinglePartUsedForBOM", typeof(int)));
      dictProps.Add("Grating Single Part Used For CollisionCheck", new Property("SinglePartUsedForCollisionCheck", typeof(int)));
      dictProps.Add("Grating Single Part Used For Numbering", new Property("SinglePartUsedForNumbering", typeof(int)));
      dictProps.Add("Grating SpecificGravity", new Property("SpecificGravity", typeof(double), ".", true));
      dictProps.Add("Grating Standard Hatch", new Property("StandardHatch", typeof(string), ".", true));
      dictProps.Add("Grating Supplier", new Property("Supplier", typeof(string)));
      dictProps.Add("Grating SupplierUsedForNumbering", new Property("SupplierUsedForNumbering", typeof(bool)));
      dictProps.Add("Grating Thickness", new Property("Thickness", typeof(double)));
      dictProps.Add("Grating Thickness Of A Bearing Bar", new Property("ThicknessOfABearingBar", typeof(double), ".", true));
      dictProps.Add("Grating Width Extension Left", new Property("WidthExtensionLeft", typeof(double)));
      dictProps.Add("Grating Width Extension Right", new Property("WidthExtensionRight", typeof(double)));
      dictProps.Add("Grating Volume", new Property("Volume", typeof(double), ".", true));
      dictProps.Add("Grating Width", new Property("Width", typeof(double), ".", true));
      dictProps.Add("Change Grating Display Mode", new Property("ReprMode", typeof(int), "Z_PostWriteDB"));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kGrating });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildStriaghtBeamPropertyList(string prefix = "")
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select " + prefix + "Straight Beam Property...", new Property("none", typeof(string)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kUnknown });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildCompoundStraightBeamPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Compound Beam Property...", new Property("none", typeof(string)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kUnknown });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildTaperedBeamPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Tapered Beam Property...", new Property("none", typeof(string)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kUnknown });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildUnfoldedBeamPropertyList(string prefix = "")
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add(prefix + "Property...", new Property("none", typeof(string)));
      dictProps.Add(prefix + "Thickness", new Property("Thickness", typeof(double)));
      dictProps.Add(prefix + "Face Alignment", new Property("Portioning", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kUnfoldedStraightBeam});

      return dictProps;
    }

    private static Dictionary<string, Property> BuildBentBeamPropertyList(string prefix = "")
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add(prefix + "Bend Beam Property...", new Property("none", typeof(string)));
      dictProps.Add(prefix + "Bend Beam Offset Curve Radius", new Property("OffsetCurveRadius", typeof(double)));
      dictProps.Add(prefix + "Bend Beam Curve Offset", new Property("CurveOffset", typeof(double), ".", true));
      dictProps.Add(prefix + "Bend Beam Systemline Radius", new Property("SystemlineRadius", typeof(double), ".", true));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBentBeam,
                    eObjectType.kConcreteBentBeam});

      return dictProps;
    }

    private static Dictionary<string, Property> BuildPolyBeamPropertyList(string prefix = "")
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add(prefix + "Poly Beam Property...", new Property("none", typeof(string)));
      dictProps.Add(prefix + "Poly Beam Reference Orientation", new Property("VecRefOrientation", typeof(Vector3d), ".", true));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPolyBeam});

      return dictProps;
    }

    private static Dictionary<string, Property> BuildCameraPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Camera Property...", new Property("none", typeof(string)));
      dictProps.Add("Camera Description", new Property("Description", typeof(string)));
      dictProps.Add("Camera Scale", new Property("Scale", typeof(double)));
      dictProps.Add("Camera Type", new Property("CameraType", typeof(string)));
      dictProps.Add("Camera 3D Coordinate System", new Property("CameraCS", typeof(Matrix3d)));
      dictProps.Add("Camera SupplierUsedForNumbering", new Property("DetailingFilterEnabled", typeof(bool)));
      dictProps.Add("Camera Type Description", new Property("TypeDescription", typeof(string), ".", true));
      dictProps.Add("Camera Supports Detailing Disable", new Property("SupportsDetailingDisable", typeof(bool), ".", true));
      dictProps.Add("Camera Detail Style", new Property("DetailStyle", typeof(int)));
      dictProps.Add("Camera Detail Style Location", new Property("DetailStyleLocation", typeof(int)));
      dictProps.Add("Camera Disable Detailing", new Property("DisableDetailing", typeof(bool)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kCamera });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildPlateFeaturePropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Plate Feature Property...", new Property("none", typeof(string)));
      dictProps.Add("Plate Feature Boring Out Switch", new Property("BoringOut", typeof(int)));
      dictProps.Add("Plate Feature Center Point", new Property("CenterPoint", typeof(Point3d), ".", true));
      dictProps.Add("Plate Feature Gap", new Property("Gap", typeof(double)));
      dictProps.Add("Plate Feature Handle", new Property("Handle", typeof(string), ".", true));
      dictProps.Add("Plate Feature Layer", new Property("Layer", typeof(string)));
      dictProps.Add("Plate Feature Length", new Property("Length", typeof(double)));
      dictProps.Add("Plate Feature LengthIncrement", new Property("LengthIncrement", typeof(double)));
      dictProps.Add("Plate Feature PureRole", new Property("PureRole", typeof(string), ".", true));
      dictProps.Add("Plate Feature RadIncrement", new Property("RadIncrement", typeof(double)));
      dictProps.Add("Plate Feature Radius", new Property("Radius", typeof(double)));
      dictProps.Add("Plate Feature Display Mode", new Property("ReprMode", typeof(int)));
      dictProps.Add("Plate Feature Role", new Property("Role", typeof(string)));
      dictProps.Add("Plate Feature Role Description", new Property("RoleDescription", typeof(string), ".", true));
      dictProps.Add("Plate Feature Use Gap Switch", new Property("UseGap", typeof(double)));
      dictProps.Add("Plate Feature Width", new Property("Width", typeof(bool)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPlateFeatContour });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildPlateVertexFilletPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Plate Vertex Property...", new Property("none", typeof(string)));
      dictProps.Add("Plate Vertex Center Point", new Property("CenterPoint", typeof(Point3d), ".", true));
      dictProps.Add("Plate Vertex Contour Index", new Property("ContourIndex", typeof(int)));
      dictProps.Add("Plate Vertex Handle", new Property("Handle", typeof(string), ".", true));
      dictProps.Add("Plate Vertex Layer", new Property("Layer", typeof(string)));
      dictProps.Add("Plate Vertex Length 1", new Property("Length1", typeof(double)));
      dictProps.Add("Plate Vertex Length 2", new Property("Length2", typeof(double)));
      dictProps.Add("Plate Vertex Object Index", new Property("ObjectIndex", typeof(int)));
      dictProps.Add("Plate Vertex PureRole", new Property("PureRole", typeof(string), ".", true));
      dictProps.Add("Plate Vertex Radius", new Property("Radius", typeof(double)));
      dictProps.Add("Plate Vertex RadIncrement", new Property("RadIncrement", typeof(double)));
      dictProps.Add("Plate Vertex Display Mode", new Property("ReprMode", typeof(int)));
      dictProps.Add("Plate Vertex Role", new Property("Role", typeof(string)));
      dictProps.Add("Plate Vertex Role Description", new Property("RoleDescription", typeof(string), ".", true));
      dictProps.Add("Plate Vertex Use Gap", new Property("UseGap", typeof(bool)));
      dictProps.Add("Plate Vertex Vertex Index", new Property("VertexIndex", typeof(short)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPlateFeatVertex });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildBeamCutPlanePropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Beam Cut Plane Property...", new Property("none", typeof(string)));
      dictProps.Add("Beam Cut Plane AngleOnY", new Property("AngleOnY", typeof(double)));
      dictProps.Add("Beam Cut Plane AngleOnZ", new Property("AngleOnZ", typeof(double)));
      dictProps.Add("Beam Cut Plane Center Point", new Property("CenterPoint", typeof(Point3d), ".", true));
      dictProps.Add("Beam Cut Plane Handle", new Property("Handle", typeof(string), ".", true));
      dictProps.Add("Beam Cut Plane Cut Length", new Property("InsLength", typeof(double)));
      dictProps.Add("Beam Cut Plane Layer", new Property("Layer", typeof(string)));
      dictProps.Add("Beam Cut Plane PureRole", new Property("PureRole", typeof(string), ".", true));
      dictProps.Add("Beam Cut Plane Display Mode", new Property("ReprMode", typeof(int)));
      dictProps.Add("Beam Cut Plane Role", new Property("Role", typeof(string)));
      dictProps.Add("Beam Cut Plane Role Description", new Property("RoleDescription", typeof(string), ".", true));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBeamShortening });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildBaseBeamNotchPropertyList(string prefix = "")
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add(prefix + "Beam Notch Corner Radius", new Property("CornerRadius", typeof(double), ".", true));
      dictProps.Add(prefix + "Beam Notch Plane Center Point", new Property("CenterPoint", typeof(Point3d), ".", true));
      dictProps.Add(prefix + "Beam Notch Plane Handle", new Property("Handle", typeof(string), ".", true));
      dictProps.Add(prefix + "Beam Notch Plane Cut Depth", new Property("ReferenceDepth", typeof(double)));
      dictProps.Add(prefix + "Beam Notch Plane Cut Length", new Property("ReferenceLength", typeof(double)));
      dictProps.Add(prefix + "Beam Notch Plane Layer", new Property("Layer", typeof(string)));
      dictProps.Add(prefix + "Beam Notch Plane PureRole", new Property("PureRole", typeof(string), ".", true));
      dictProps.Add(prefix + "Beam Notch Plane Display Mode", new Property("ReprMode", typeof(int)));
      dictProps.Add(prefix + "Beam Notch Plane Role", new Property("Role", typeof(string)));
      dictProps.Add(prefix + "Beam Notch Plane Role Description", new Property("RoleDescription", typeof(string), ".", true));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBeamNotch2Ortho,
                    eObjectType.kBeamNotchEx,
                    eObjectType.kBeamMultiContourNotch});

      return dictProps;
    }

    private static Dictionary<string, Property> BuildBeamNotchOrthoPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Beam Notch Ortho Property...", new Property("none", typeof(string)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kUnknown });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildBeamMultiNotchPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Beam Multi Notch Property...", new Property("none", typeof(string)));
      dictProps.Add("Beam Beam Multi Boring Out", new Property("BoringOut", typeof(int)));
      dictProps.Add("Beam Beam Multi Center Point", new Property("CenterPoint", typeof(Point3d), ".", true));
      dictProps.Add("Beam Beam Multi Coordinate System", new Property("CS", typeof(Matrix3d)));
      dictProps.Add("Beam Beam Multi Gap", new Property("Gap", typeof(double)));
      dictProps.Add("Beam Beam Multi Handle", new Property("Handle", typeof(string), ".", true));
      dictProps.Add("Beam Beam Multi Layer", new Property("Layer", typeof(string)));
      dictProps.Add("Beam Beam Multi Length", new Property("Length", typeof(double)));
      dictProps.Add("Beam Beam Multi Length Increment", new Property("LengthIncrement", typeof(double)));
      dictProps.Add("Beam Beam Multi Normal Vector", new Property("Normal", typeof(Vector3d), ".", true));
      dictProps.Add("Beam Beam Multi Radius Increment", new Property("RadIncrement", typeof(double)));
      dictProps.Add("Beam Beam Multi Radius", new Property("Radius", typeof(double)));
      dictProps.Add("Beam Beam Multi Display Mode", new Property("ReprMode", typeof(int)));
      dictProps.Add("Beam Beam Multi Role", new Property("Role", typeof(string)));
      dictProps.Add("Beam Beam Multi Role Description", new Property("RoleDescription", typeof(string), ".", true));
      dictProps.Add("Beam Beam Multi PureRole", new Property("PureRole", typeof(string), ".", true));
      dictProps.Add("Beam Beam Multi Use Gap", new Property("UseGap", typeof(bool)));
      dictProps.Add("Beam Beam Multi Width", new Property("Width", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBeamMultiContourNotch });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildPlateNotchContourPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Plate Notch Contour Property...", new Property("none", typeof(string)));
      dictProps.Add("Beam Plate Notch Contour Boring Out", new Property("BoringOut", typeof(int)));
      dictProps.Add("Beam Plate Notch Contour Center Point", new Property("CenterPoint", typeof(Point3d), ".", true));
      dictProps.Add("Beam Plate Notch Contour Coordinate System", new Property("CS", typeof(Matrix3d)));
      dictProps.Add("Beam Plate Notch Contour Gap", new Property("Gap", typeof(double)));
      dictProps.Add("Beam Plate Notch Contour Handle", new Property("Handle", typeof(string), ".", true));
      dictProps.Add("Beam Plate Notch Contour Layer", new Property("Layer", typeof(string)));
      dictProps.Add("Beam Plate Notch Contour Length", new Property("Length", typeof(double)));
      dictProps.Add("Beam Plate Notch Contour Length Increment", new Property("LengthIncrement", typeof(double)));
      dictProps.Add("Beam Plate Notch Contour Normal Vector", new Property("Normal", typeof(Vector3d), ".", true));
      dictProps.Add("Beam Plate Notch Contour Radius Increment", new Property("RadIncrement", typeof(double)));
      dictProps.Add("Beam Plate Notch Contour Radius", new Property("Radius", typeof(double)));
      dictProps.Add("Beam Plate Notch Contour Display Mode", new Property("ReprMode", typeof(int)));
      dictProps.Add("Beam Plate Notch Contour Role", new Property("Role", typeof(string)));
      dictProps.Add("Beam Plate Notch Contour Role Description", new Property("RoleDescription", typeof(string), ".", true));
      dictProps.Add("Beam Plate Notch Contour PureRole", new Property("PureRole", typeof(string), ".", true));
      dictProps.Add("Beam Plate Notch Contour Use Gap", new Property("UseGap", typeof(bool)));
      dictProps.Add("Beam Plate Notch Contour Width", new Property("Width", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPlateFeatContour });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildBeamNotchRotatedPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Beam Notch Rotated Property...", new Property("none", typeof(string)));
      dictProps.Add("Beam Notch Axis Angle", new Property("AxisAngle", typeof(double)));
      dictProps.Add("Beam Notch Z Angle", new Property("ZAngle", typeof(double)));
      dictProps.Add("Beam Notch X Angle", new Property("XAngle", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBeamNotchEx });

      return dictProps;
    }

    private static Dictionary<string, Property> BuildCompundBaseBeamPropertyList(string prefix = "")
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Use " + prefix + "Beam As One Beam", new Property("UseCompoundAsOneBeam", typeof(bool)));
      dictProps.Add(prefix + "Beam ClassName", new Property("CompoundClassName", typeof(string), ".", true));
      dictProps.Add(prefix + "Beam TypeName", new Property("CompoundTypeName", typeof(string), ".", true));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kCompoundStraightBeam });

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
  }
}
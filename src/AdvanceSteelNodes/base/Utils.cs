using AdvanceSteel.Nodes.Beams;
using AdvanceSteel.Nodes.Concrete;
using AdvanceSteel.Nodes.Gratings;
using AdvanceSteel.Nodes.Plates;
using AdvanceSteel.Nodes.ConnectionObjects.Anchors;
using AdvanceSteel.Nodes.ConnectionObjects.Bolts;
using AdvanceSteel.Nodes.ConnectionObjects.ShearStuds;
using AdvanceSteel.Nodes.ConnectionObjects.Welds;
using AdvanceSteel.Nodes.Modifications;
using AdvanceSteel.Nodes.NonSteelItems;
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
      { FilerObject.eObjectType.kCompoundStraightBeam, (string handle) => new CompoundBeam() },
      { FilerObject.eObjectType.kBeamTapered, (string handle) => new TaperedBeam() },
      { FilerObject.eObjectType.kBentBeam, (string handle) => new BentBeam() },
      { FilerObject.eObjectType.kConcreteBentBeam, (string handle) => new ConcBentBeam() },
      { FilerObject.eObjectType.kConcreteBeam, (string handle) => new ConcStraightBeam() },
      { FilerObject.eObjectType.kFootingIsolated, (string handle) => new Footings() },
      { FilerObject.eObjectType.kSlab, (string handle) => new Slabs() },
      { FilerObject.eObjectType.kWall, (string handle) => new Walls() },
      { FilerObject.eObjectType.kPlate, (string handle) => new Plate() },
      { FilerObject.eObjectType.kGrating, (string handle) => GetDynGrating(handle) },
      { FilerObject.eObjectType.kCamera, (string handle) => new Camera() },
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
      { FilerObject.eObjectType.kUnknown , "Select As Object Type..." },
      { FilerObject.eObjectType.kStraightBeam, "Advance Steel Straight Beam" },
      { FilerObject.eObjectType.kCompoundStraightBeam, "Advance Steel Compound Beam" },
      { FilerObject.eObjectType.kBeamTapered, "Advance Steel Tapered Beam" },
      { FilerObject.eObjectType.kBentBeam, "Advance Steel Bent Beam" },
      { FilerObject.eObjectType.kConcreteBentBeam, "Advance Steel Concrete Bent Beam" },
      { FilerObject.eObjectType.kConcreteBeam, "Advance Steel Concrete Striaght Beam" },
      { FilerObject.eObjectType.kFootingIsolated, "Advance Steel Isolated Footings" },
      { FilerObject.eObjectType.kSlab, "Advance Steel Slabs" },
      { FilerObject.eObjectType.kWall, "Advance Steel Wals" },
      { FilerObject.eObjectType.kPlate, "Advance Steel Plate" },
      { FilerObject.eObjectType.kGrating, "Advance Steel Grating" },
      { FilerObject.eObjectType.kAnchorPattern, "Advance Steel Anchor Pattern" },
      { FilerObject.eObjectType.kCircleScrewBoltPattern, "Advance Steel Circular Bolt Pattern" },
      { FilerObject.eObjectType.kConnector, "Advance Steel Shear Studs" },
      { FilerObject.eObjectType.kInfinitMidScrewBoltPattern, "Advance Steel Rectangular Bolt Pattern" },
      { FilerObject.eObjectType.kWeldStraight, "Advance Steel Weld Line" },
      { FilerObject.eObjectType.kWeldPattern, "Advance Steel Weld Point" },
      { FilerObject.eObjectType.kBeamNotch2Ortho, "Advance Steel Beam Cope" },
      { FilerObject.eObjectType.kBeamShortening, "Advance Steel Beam Shortening" },
      { FilerObject.eObjectType.kBeamMultiContourNotch, "Advance Steel Beam Polycut" },
      { FilerObject.eObjectType.kPlateFeatContour, "Advance Steel Plate Polycut" },
      { FilerObject.eObjectType.kPlateFeatVertFillet, "Advance Steel Plate Corner Cut" },
      { FilerObject.eObjectType.kCamera, "Advance Steel Camera" }
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

    static public Double ToInternalUnits(double value, bool bConvert)
    {
      double factor = 1.0;
      if (bConvert)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }

      return (value * factor);
    }

    static public Double FromInternalUnits(double value, bool bConvertFromAstUnits)
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
          Autodesk.DesignScript.Geometry.Vector midNormal = nextCurve.NormalAtParameter(nextCurve.Length/2);
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
              PolyVertexs[i] = new VertexInfo(Utils.FromInternalUnits(tempArc.Radius, bConvertToAstUnits),
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
          PolyVertexs[i] = new VertexInfo(Utils.FromInternalUnits(arc.Radius, bConvertToAstUnits),
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
                            obj.IsKindOf(FilerObject.eObjectType.kPlate)))
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
            obj is AdvanceSteel.Nodes.Plates.Plate)
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

    public static Dictionary<string, ASProperty> GetBoltProperties(int listFilter)
    {
      return BuildBoltPropertyList(listFilter);
    }

    public static Dictionary<string, ASProperty> GetAnchorBoltPropertyList(int listFilter)
    {
      return BuildAnchorBoltPropertyList(listFilter);
    }

    public static Dictionary<string, ASProperty> GetShearStudPropertyList(int listFilter)
    {
      return BuildShearStudPropertyList(listFilter);
    }
    
    public static Dictionary<string, ASProperty> GetPlatePropertyList(int listFilter)
    {
      return BuildGenericPlatePropertyList(listFilter);
    }
    
    public static Dictionary<string, ASProperty> GetCameraPropertyList(int listFilter)
    {
      return BuildCameraPropertyList(listFilter);
    }

    public static Dictionary<string, ASProperty> GetPlateFeaturePropertyList(int listFilter)
    {
      return BuildPlateFeaturePropertyList(listFilter);
    }

    public static Dictionary<string, ASProperty> GetBeamCutPlanePropertyList(int listFilter)
    {
      return BuildBeamCutPlanePropertyList(listFilter);
    }
    
    public static Dictionary<string, ASProperty> GetBeamMultiNotchPropertyList(int listFilter)
    {
      return BuildBeamMultiNotchPropertyList(listFilter);
    }
    
    public static Dictionary<string, ASProperty> GetPlateNotchContourPropertyList(int listFilter)
    {
      return BuildPlateNotchContourPropertyList(listFilter);
    }

    public static Dictionary<string, ASProperty> GetBeamNotchOrthoPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> combinedData = BuildBeamNotchOrthoPropertyList(listFilter).Union(
                                            BuildBaseBeamNotchPropertyList(listFilter, "Othro ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, ASProperty> GetBeamNotchRotatedPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> combinedData = BuildBeamNotchRotatedPropertyList(listFilter).Union(
                                            BuildBaseBeamNotchPropertyList(listFilter, "Rotated ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, ASProperty> GetPlateVertexPropertyList(int listFilter)
    {
      return BuildPlateVertexFilletPropertyList(listFilter);
    }

    public static Dictionary<string, ASProperty> GetGratingPropertyList(int listFilter)
    {
      return BuildGenericGratingPropertyList(listFilter);
    }

    public static Dictionary<string, ASProperty> GetStraighBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> combinedData = BuildStriaghtBeamPropertyList(listFilter).Union(
                                            BuildGenericBeamPropertyList(listFilter, "Straight ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, ASProperty> GetBentBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> combinedData = BuildBentBeamPropertyList(listFilter).Union(
                                            BuildGenericBeamPropertyList(listFilter, "Bend ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, ASProperty> GetTaperBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> combinedData = BuildTaperedBeamPropertyList(listFilter).Union(
                                                  BuildCompundBaseBeamPropertyList(listFilter, "Tapered ")).Union(
                                                  BuildGenericBeamPropertyList(listFilter, "Tapered ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, ASProperty> GetCompoundStraightBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> combinedData = BuildCompoundStraightBeamPropertyList(listFilter).Union(
                                                  BuildCompundBaseBeamPropertyList(listFilter, "Compound ")).Union(
                                                  BuildGenericBeamPropertyList(listFilter, "Compound ")).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static ASProperty GetProperty(string keyValue, int listFilter)
    {
      Dictionary<string, ASProperty> searchData = CombineAllLists(listFilter);
      ASProperty retValue = null;
      searchData.TryGetValue(keyValue, out retValue);
      return retValue;
    }

    public static Dictionary<string, ASProperty> GetAllProperties(int filter)
    {
      return CombineAllLists(filter);
    }

    private static Dictionary<string, ASProperty> CombineAllLists(int listFilter)
    {
      Dictionary<string, ASProperty> searchData = GetBoltProperties(listFilter).Union(
                                                  GetAnchorBoltPropertyList(listFilter)).Union(
                                                  GetShearStudPropertyList(listFilter)).Union(
                                                  GetPlatePropertyList(listFilter)).Union(
                                                  GetCameraPropertyList(listFilter)).Union(
                                                  GetPlateFeaturePropertyList(listFilter)).Union(
                                                  GetBeamCutPlanePropertyList(listFilter)).Union(
                                                  GetBeamMultiNotchPropertyList(listFilter)).Union(
                                                  GetPlateNotchContourPropertyList(listFilter)).Union(
                                                  GetBeamNotchOrthoPropertyList(listFilter)).Union(
                                                  GetBeamNotchRotatedPropertyList(listFilter)).Union(
                                                  GetPlateVertexPropertyList(listFilter)).Union(
                                                  GetGratingPropertyList(listFilter)).Union(
                                                  GetStraighBeamPropertyList(listFilter)).Union(
                                                  GetBentBeamPropertyList(listFilter)).Union(
                                                  GetTaperBeamPropertyList(listFilter)).Union(
                                                  GetCompoundStraightBeamPropertyList(listFilter)).ToDictionary(s => s.Key, s => s.Value);
      return searchData;
    }

    private static Dictionary<string, ASProperty> filterDictionary(Dictionary<string, ASProperty> dictProps, int listFilter)
    {
      return (listFilter > 0 ? dictProps.Where(x => (x.Value.PropertyDataOp % listFilter) == 0).ToDictionary(x => x.Key, x => x.Value) : dictProps);
    }

    private static Dictionary<string, ASProperty> addElementTypes(Dictionary<string, ASProperty> dictProps, List<eObjectType> elementTypes)
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

    private static Dictionary<string, ASProperty> BuildBoltPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Bolt Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Bolt Standard", new ASProperty("Standard", typeof(string)));
      dictProps.Add("Bolt Assembly", new ASProperty("BoltAssembly", typeof(string)));
      dictProps.Add("Bolt Grade", new ASProperty("Grade", typeof(string)));
      dictProps.Add("Bolt Diameter", new ASProperty("ScrewDiameter", typeof(double)));
      dictProps.Add("Bolt Hole Tolerance", new ASProperty("HoleTolerance", typeof(double)));
      dictProps.Add("No of Bolts Circle", new ASProperty("NumberOfScrews", typeof(int)));
      dictProps.Add("Bolt X Count", new ASProperty("Nx", typeof(int)));
      dictProps.Add("Bolt Y Count", new ASProperty("Ny", typeof(int)));
      dictProps.Add("Bolt X Spacing", new ASProperty("Dx", typeof(double)));
      dictProps.Add("Bolt Y Spacing", new ASProperty("Dy", typeof(double)));
      dictProps.Add("Bolt Pattern Radius", new ASProperty("Radius", typeof(double)));
      dictProps.Add("Bolt Length Addition", new ASProperty("BindingLengthAddition", typeof(double)));
      dictProps.Add("Bolt Inverted", new ASProperty("IsInverted", typeof(bool)));

      addElementTypes(dictProps, new List<eObjectType>() { 
                    eObjectType.kInfinitMidScrewBoltPattern, 
                    eObjectType.kCircleScrewBoltPattern });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildAnchorBoltPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Anchor Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Anchor Standard", new ASProperty("Standard", typeof(string)));
      dictProps.Add("Anchor Assembly", new ASProperty("BoltAssembly", typeof(string)));
      dictProps.Add("Anchor Grade", new ASProperty("Grade", typeof(string))); 
      dictProps.Add("Anchor Length", new ASProperty("ScrewLength", typeof(double)));
      dictProps.Add("Anchor Diameter", new ASProperty("ScrewDiameter", typeof(double)));
      dictProps.Add("Anchor Hole Tolerance", new ASProperty("HoleTolerance", typeof(double)));
      dictProps.Add("No of Anchor Circle", new ASProperty("NumberOfScrews", typeof(int)));
      dictProps.Add("Anchor X Count", new ASProperty("Nx", typeof(int)));
      dictProps.Add("Anchor Y Count", new ASProperty("Ny", typeof(int)));
      dictProps.Add("Anchor X Spacing", new ASProperty("Dx", typeof(double)));
      dictProps.Add("Anchor Y Spacing", new ASProperty("Dy", typeof(double)));
      dictProps.Add("Anchor Pattern Radius", new ASProperty("Radius", typeof(double))); 
      dictProps.Add("Anchor Inverted", new ASProperty("IsInverted", typeof(bool)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kAnchorPattern });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildShearStudPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Stud Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Stud Standard", new ASProperty("Standard", typeof(string)));
      dictProps.Add("Stud Grade", new ASProperty("Grade", typeof(string)));
      dictProps.Add("Stud Length", new ASProperty("Length", typeof(double)));
      dictProps.Add("Stud Diameter", new ASProperty("Diameter", typeof(double)));
      dictProps.Add("Stud Hole Tolerance", new ASProperty("HoleTolerance", typeof(double)));
      dictProps.Add("No of Shear Studs Circle", new ASProperty("NumberOfElements", typeof(int), "Arranger"));
      dictProps.Add("Shear Stud Radius", new ASProperty("Radius", typeof(double), "Arranger")); 
      dictProps.Add("Stud X Count", new ASProperty("Nx", typeof(int), "Arranger"));
      dictProps.Add("Stud Y Count", new ASProperty("Ny", typeof(int), "Arranger"));
      dictProps.Add("Stud X Spacing", new ASProperty("Dx", typeof(double), "Arranger"));
      dictProps.Add("Stud Y Spacing", new ASProperty("Dy", typeof(double), "Arranger"));
      dictProps.Add("Display Stud As Solid", new ASProperty("ReprMode", typeof(int), "Z_PostWriteDB"));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kConnector });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildGenericBeamPropertyList(int listFilter, string prefix = "")
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add(prefix + "Beam Angle", new ASProperty("Angle", typeof(double)));
      dictProps.Add(prefix + "Beam Approval Comment", new ASProperty("ApprovalComment", typeof(string)));
      dictProps.Add(prefix + "Beam Approval Status Code", new ASProperty("ApprovalStatusCode", typeof(string)));
      dictProps.Add(prefix + "Beam Assembly", new ASProperty("Assembly", typeof(string)));
      dictProps.Add(prefix + "Beam Assembly Used For Numbering", new ASProperty("AssemblyUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Beam Center Point", new ASProperty("CenterPoint", typeof(Point3d), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam Carrier", new ASProperty("Carrier", typeof(string)));
      dictProps.Add(prefix + "Beam Coating", new ASProperty("Coating", typeof(string))); 
      dictProps.Add(prefix + "Beam Coating Description", new ASProperty("CoatingDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam Coating Used For Numbering", new ASProperty("CoatingUsedForNumbering", typeof(int))); 
      dictProps.Add(prefix + "Beam Delivery Date", new ASProperty("DeliveryDate", typeof(string)));
      dictProps.Add(prefix + "Beam Denotation Used For Numbering", new ASProperty("DennotationUsedForNumbering", typeof(int))); 
      dictProps.Add(prefix + "Beam Denotation Role", new ASProperty("Denotation", typeof(string)));
      dictProps.Add(prefix + "Beam Deviation", new ASProperty("Deviation", typeof(double)));
      dictProps.Add(prefix + "Beam Explicit Quantity", new ASProperty("ExplicitQuantity", typeof(int))); 
      dictProps.Add(prefix + "Beam Fabrication Station", new ASProperty("FabricationStation", typeof(string)));
      dictProps.Add(prefix + "Beam Fabrication Station UsedF or Numbering", new ASProperty("FabricationStationUsedForNumbering", typeof(bool)));
      dictProps.Add(prefix + "Beam Handle", new ASProperty("Handle", typeof(string),".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam Heat Number", new ASProperty("HeatNumber", typeof(string)));
      dictProps.Add(prefix + "Beam Heat Number Used For Numbering", new ASProperty("HeatNumberUsedForNumbering", typeof(bool)));
      dictProps.Add(prefix + "Beam Holes Used For Numbering", new ASProperty("HolesUsedForNumbering", typeof(int))); 
      dictProps.Add(prefix + "Beam Set IsMainPart Flag", new ASProperty("IsMainPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add(prefix + "Beam Get IsAttachedPart Flag", new ASProperty("IsAttachedPart", typeof(bool), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam Get IsCrossSectionMirrored Flag", new ASProperty("IsCrossSectionMirrored", typeof(bool), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam ItemNumber", new ASProperty("ItemNumber", typeof(string)));
      dictProps.Add(prefix + "Beam ItemNumber Used For Numbering", new ASProperty("ItemNumberUsedForNumbering", typeof(int))); 
      dictProps.Add(prefix + "Beam Layer", new ASProperty("Layer", typeof(string)));
      dictProps.Add(prefix + "Beam Load Number", new ASProperty("LoadNumber", typeof(string)));
      dictProps.Add(prefix + "Beam MainPart Number", new ASProperty("MainPartNumber", typeof(string)));
      dictProps.Add(prefix + "Beam MainPart Number Prefix", new ASProperty("MainPartPrefix", typeof(string)));
      dictProps.Add(prefix + "Beam MainPart Used For BOM", new ASProperty("MainPartUsedForBOM", typeof(int))); 
      dictProps.Add(prefix + "Beam MainPart Used For Collision Check", new ASProperty("MainPartUsedForCollisionCheck", typeof(int))); 
      dictProps.Add(prefix + "Beam MainPart Used For Numbering", new ASProperty("MainPartUsedForNumbering", typeof(int))); 
      dictProps.Add(prefix + "Beam Material", new ASProperty("Material", typeof(string)));
      dictProps.Add(prefix + "Beam Material Description", new ASProperty("MaterialDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam Material Used For Numbering", new ASProperty("MaterialUsedForNumbering", typeof(int))); 
      dictProps.Add(prefix + "Beam Note", new ASProperty("Note", typeof(string)));
      dictProps.Add(prefix + "Beam Note Used For Numbering", new ASProperty("NoteUsedForNumbering", typeof(int))); 
      dictProps.Add(prefix + "Beam Number Of Holes", new ASProperty("NumberOfHoles", typeof(int), ".", ePropertyDataOperator.Get)); 
      dictProps.Add(prefix + "Beam PONumber", new ASProperty("PONumber", typeof(string)));
      dictProps.Add(prefix + "Beam PONumber Used For Numbering", new ASProperty("PONumberUsedForNumbering", typeof(bool)));
      dictProps.Add(prefix + "Beam Preliminary Part Number", new ASProperty("PreliminaryPartNumber", typeof(string))); 
      dictProps.Add(prefix + "Beam Preliminary Part Position Number", new ASProperty("PreliminaryPartPositionNumber", typeof(string), ".", ePropertyDataOperator.Get)); 
      dictProps.Add(prefix + "Beam Preliminary Part Prefix", new ASProperty("PreliminaryPartPrefix", typeof(string)));
      dictProps.Add(prefix + "Beam Profile Name", new ASProperty("ProfName", typeof(string)));
      dictProps.Add(prefix + "Beam Profile Section Type", new ASProperty("ProfSectionType", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam Profile Section name", new ASProperty("ProfSectionName", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam Requisition Number", new ASProperty("RequisitionNumber", typeof(string)));
      dictProps.Add(prefix + "Beam Requisition Number Used For Numbering", new ASProperty("RequisitionNumberUsedForNumbering", typeof(bool)));
      dictProps.Add(prefix + "Beam Model Role", new ASProperty("Role", typeof(string)));
      dictProps.Add(prefix + "Beam Model Role Description", new ASProperty("RoleDescription", typeof(string)));
      dictProps.Add(prefix + "Beam Role Used For Numbering", new ASProperty("RoleUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Beam Runname", new ASProperty("Runname", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam Shipped Date", new ASProperty("ShippedDate", typeof(string)));
      dictProps.Add(prefix + "Beam ShrinkValue", new ASProperty("ShrinkValue", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam Single Part Number", new ASProperty("SinglePartNumber", typeof(string)));
      dictProps.Add(prefix + "Beam Single Part Prefix", new ASProperty("SinglePartPrefix", typeof(string)));
      dictProps.Add(prefix + "Beam Single Part Used For BOM", new ASProperty("SinglePartUsedForBOM", typeof(int))); 
      dictProps.Add(prefix + "Beam Single Part Used For CollisionCheck", new ASProperty("SinglePartUsedForCollisionCheck", typeof(int))); 
      dictProps.Add(prefix + "Beam Single Part Used For Numbering", new ASProperty("SinglePartUsedForNumbering", typeof(int)));
      dictProps.Add(prefix + "Beam Specific Gravity", new ASProperty("SpecificGravity", typeof(double), ".", ePropertyDataOperator.Get)); 
      dictProps.Add(prefix + "Beam Structural Member", new ASProperty("StructuralMember", typeof(int)));
      dictProps.Add(prefix + "Beam System Line Length", new ASProperty("SysLength", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam Supplier", new ASProperty("Supplier", typeof(string)));
      dictProps.Add(prefix + "Beam SupplierUsedForNumbering", new ASProperty("SupplierUsedForNumbering", typeof(bool)));
      dictProps.Add(prefix + "Beam Unwind / Unfolder", new ASProperty("Unwind", typeof(bool)));
      dictProps.Add(prefix + "Beam UnwindStartFactor", new ASProperty("UnwindStartFactor", typeof(double)));
      dictProps.Add(prefix + "Beam Volume", new ASProperty("Volume", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Change Beam Display Mode", new ASProperty("ReprMode", typeof(int), "Z_PostWriteDB"));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kStraightBeam });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildGenericPlatePropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Plate Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Plate Approval Comment", new ASProperty("ApprovalComment", typeof(string)));
      dictProps.Add("Plate Approval Status Code", new ASProperty("ApprovalStatusCode", typeof(string)));
      dictProps.Add("Plate Assembly", new ASProperty("Assembly", typeof(string)));
      dictProps.Add("Plate Assembly Used For Numbering", new ASProperty("AssemblyUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Center Point", new ASProperty("CenterPoint", typeof(Point3d), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Carrier", new ASProperty("Carrier", typeof(string)));
      dictProps.Add("Plate Coating", new ASProperty("Coating", typeof(string)));
      dictProps.Add("Plate Coating Description", new ASProperty("CoatingDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Coating Used For Numbering", new ASProperty("CoatingUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Delivery Date", new ASProperty("DeliveryDate", typeof(string)));
      dictProps.Add("Plate Denotation Used For Numbering", new ASProperty("DennotationUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Denotation", new ASProperty("Denotation", typeof(string)));
      dictProps.Add("Plate Explicit Quantity", new ASProperty("ExplicitQuantity", typeof(int)));
      dictProps.Add("Plate Fabrication Station", new ASProperty("FabricationStation", typeof(string)));
      dictProps.Add("Plate Fabrication Station UsedF or Numbering", new ASProperty("FabricationStationUsedForNumbering", typeof(bool)));
      dictProps.Add("Plate Handle", new ASProperty("Handle", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Heat Number", new ASProperty("HeatNumber", typeof(string)));
      dictProps.Add("Plate Heat Number Used For Numbering", new ASProperty("HeatNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Plate Holes Used For Numbering", new ASProperty("HolesUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Set IsAttached Flag", new ASProperty("IsAttachedPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add("Plate Set IsMainPart Flag", new ASProperty("IsMainPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add("Plate ItemNumber", new ASProperty("ItemNumber", typeof(string)));
      dictProps.Add("Plate ItemNumber Used For Numbering", new ASProperty("ItemNumberUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Layer", new ASProperty("Layer", typeof(string)));
      dictProps.Add("Plate Length", new ASProperty("Length", typeof(double)));
      dictProps.Add("Plate Load Number", new ASProperty("LoadNumber", typeof(string)));
      dictProps.Add("Plate MainPart Number", new ASProperty("MainPartNumber", typeof(string)));
      dictProps.Add("Plate MainPart Number Prefix", new ASProperty("MainPartPrefix", typeof(string)));
      dictProps.Add("Plate MainPart Used For BOM", new ASProperty("MainPartUsedForBOM", typeof(int)));
      dictProps.Add("Plate MainPart Used For Collision Check", new ASProperty("MainPartUsedForCollisionCheck", typeof(int)));
      dictProps.Add("Plate MainPart Used For Numbering", new ASProperty("MainPartUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Material", new ASProperty("Material", typeof(string)));
      dictProps.Add("Plate Material Description", new ASProperty("MaterialDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Material Used For Numbering", new ASProperty("MaterialUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Note", new ASProperty("Note", typeof(string)));
      dictProps.Add("Plate Note Used For Numbering", new ASProperty("NoteUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Number Of Holes", new ASProperty("NumberOfHoles", typeof(int), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate PONumber", new ASProperty("PONumber", typeof(string)));
      dictProps.Add("Plate PONumber Used For Numbering", new ASProperty("PONumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Plate Face Alignment", new ASProperty("Portioning", typeof(double)));
      dictProps.Add("Plate Preliminary Part Number", new ASProperty("PreliminaryPartNumber", typeof(string)));
      dictProps.Add("Plate Preliminary Part Position Number", new ASProperty("PreliminaryPartPositionNumber", typeof(string)));
      dictProps.Add("Plate Preliminary Part Prefix", new ASProperty("PreliminaryPartPrefix", typeof(string)));
      dictProps.Add("Plate Radius Increment", new ASProperty("RadIncrement", typeof(double)));
      dictProps.Add("Plate Radius", new ASProperty("Radius", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Requisition Number", new ASProperty("RequisitionNumber", typeof(string)));
      dictProps.Add("Plate Requisition Number Used For Numbering", new ASProperty("RequisitionNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Plate Model Role", new ASProperty("Role", typeof(string)));
      dictProps.Add("Plate Model Role Description", new ASProperty("RoleDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Role Used For Numbering", new ASProperty("RoleUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Shipped Date", new ASProperty("ShippedDate", typeof(string)));
      dictProps.Add("Plate Single Part Number", new ASProperty("SinglePartNumber", typeof(string)));
      dictProps.Add("Plate Single Part Prefix", new ASProperty("SinglePartPrefix", typeof(string)));
      dictProps.Add("Plate Single Part Used For BOM", new ASProperty("SinglePartUsedForBOM", typeof(int)));
      dictProps.Add("Plate Single Part Used For CollisionCheck", new ASProperty("SinglePartUsedForCollisionCheck", typeof(int)));
      dictProps.Add("Plate Single Part Used For Numbering", new ASProperty("SinglePartUsedForNumbering", typeof(int)));
      dictProps.Add("Plate SpecificGravity", new ASProperty("SpecificGravity", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Supplier", new ASProperty("Supplier", typeof(string)));
      dictProps.Add("Plate SupplierUsedForNumbering", new ASProperty("SupplierUsedForNumbering", typeof(bool)));
      dictProps.Add("Plate Thickness", new ASProperty("Thickness", typeof(double)));
      dictProps.Add("Plate Volume", new ASProperty("Volume", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Width", new ASProperty("Width", typeof(double)));
      dictProps.Add("Change Plate Display Mode", new ASProperty("ReprMode", typeof(int), "Z_PostWriteDB"));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPlate });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildGenericGratingPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Grating Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Grating Approval Comment", new ASProperty("ApprovalComment", typeof(string)));
      dictProps.Add("Grating Approval Status Code", new ASProperty("ApprovalStatusCode", typeof(string)));
      dictProps.Add("Grating Assembly", new ASProperty("Assembly", typeof(string)));
      dictProps.Add("Grating Assembly Used For Numbering", new ASProperty("AssemblyUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Center Point", new ASProperty("CenterPoint", typeof(Point3d), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Bearing Bar Quantity", new ASProperty("BearingBarQuantity", typeof(int), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Bearing Bar Spacing", new ASProperty("BearingBarSpacing", typeof(int), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Bearing Bar Spacing Distance", new ASProperty("BearingBarSpacingDistance", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Carrier", new ASProperty("Carrier", typeof(string)));
      dictProps.Add("Grating Coating", new ASProperty("Coating", typeof(string)));
      dictProps.Add("Grating Coating Description", new ASProperty("CoatingDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Coating Used For Numbering", new ASProperty("CoatingUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Connector Key", new ASProperty("ConnectorKey", typeof(int)));
      dictProps.Add("Grating Connector Name", new ASProperty("ConnectorName", typeof(string)));
      dictProps.Add("Grating Connector Quantity", new ASProperty("ConnectorQuantity", typeof(int)));
      dictProps.Add("Grating Cross Bar", new ASProperty("CrossBar", typeof(string))); 
      dictProps.Add("Grating Cross Bar Quantity", new ASProperty("CrossBarQuantity", typeof(int), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Cross Bar Spacing", new ASProperty("CrossBarSpacing", typeof(int), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Cross Bar Spacing Distance", new ASProperty("CrossBarSpacingDistance", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Custom Hatch", new ASProperty("CustomHatch", typeof(string)));
      dictProps.Add("Grating EDValue", new ASProperty("EDValue", typeof(double)));
      dictProps.Add("Grating Direction", new ASProperty("Direction", typeof(Vector3d)));
      dictProps.Add("Grating Delivery Date", new ASProperty("DeliveryDate", typeof(string)));
      dictProps.Add("Grating Denotation Used For Numbering", new ASProperty("DennotationUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Denotation", new ASProperty("Denotation", typeof(string)));
      dictProps.Add("Grating Explicit Quantity", new ASProperty("ExplicitQuantity", typeof(int)));
      dictProps.Add("Grating Fabrication Station", new ASProperty("FabricationStation", typeof(string)));
      dictProps.Add("Grating Fabrication Station UsedF or Numbering", new ASProperty("FabricationStationUsedForNumbering", typeof(bool)));
      dictProps.Add("Grating Class", new ASProperty("GratingClass", typeof(string)));
      dictProps.Add("Grating Size", new ASProperty("GratingSize", typeof(string)));
      dictProps.Add("Grating Handle", new ASProperty("Handle", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Heat Number", new ASProperty("HeatNumber", typeof(string)));
      dictProps.Add("Grating Heat Number Used For Numbering", new ASProperty("HeatNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Grating Holes Used For Numbering", new ASProperty("HolesUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Set IsAttached Flag", new ASProperty("IsAttachedPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add("Grating Set IsMainPart Flag", new ASProperty("IsMainPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add("Grating Set IsMatCoating Database Defined", new ASProperty("IsMatCoatDbDefined", typeof(bool)));
      dictProps.Add("Grating Set IsUsing Standard ED Value", new ASProperty("IsUsingStandardED", typeof(bool)));
      dictProps.Add("Grating Set IsUsing Standard Hatch Value", new ASProperty("IsUsingStandardHatch", typeof(bool)));
      dictProps.Add("Grating ItemNumber", new ASProperty("ItemNumber", typeof(string)));
      dictProps.Add("Grating ItemNumber Used For Numbering", new ASProperty("ItemNumberUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Layer", new ASProperty("Layer", typeof(string)));
      dictProps.Add("Grating Length", new ASProperty("Length", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Length Increment", new ASProperty("LengthIncrement", typeof(double)));
      dictProps.Add("Grating Load Number", new ASProperty("LoadNumber", typeof(string)));
      dictProps.Add("Grating MainPart Number", new ASProperty("MainPartNumber", typeof(string)));
      dictProps.Add("Grating MainPart Number Prefix", new ASProperty("MainPartPrefix", typeof(string)));
      dictProps.Add("Grating MainPart Used For BOM", new ASProperty("MainPartUsedForBOM", typeof(int)));
      dictProps.Add("Grating MainPart Used For Collision Check", new ASProperty("MainPartUsedForCollisionCheck", typeof(int)));
      dictProps.Add("Grating MainPart Used For Numbering", new ASProperty("MainPartUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Material", new ASProperty("Material", typeof(string)));
      dictProps.Add("Grating Material Description", new ASProperty("MaterialDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Material Used For Numbering", new ASProperty("MaterialUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Note", new ASProperty("Note", typeof(string)));
      dictProps.Add("Grating Note Used For Numbering", new ASProperty("NoteUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Number Of Holes", new ASProperty("NumberOfHoles", typeof(int), ".", ePropertyDataOperator.Get)); 
      dictProps.Add("Grating Normal", new ASProperty("PlateNormal", typeof(Vector3d), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating OED Value", new ASProperty("OEDValue", typeof(double)));
      dictProps.Add("Grating PONumber", new ASProperty("PONumber", typeof(string)));
      dictProps.Add("Grating PONumber Used For Numbering", new ASProperty("PONumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Grating Face Alignment", new ASProperty("Portioning", typeof(double)));
      dictProps.Add("Grating Preliminary Part Number", new ASProperty("PreliminaryPartNumber", typeof(string)));
      dictProps.Add("Grating Preliminary Part Position Number", new ASProperty("PreliminaryPartPositionNumber", typeof(string)));
      dictProps.Add("Grating Preliminary Part Prefix", new ASProperty("PreliminaryPartPrefix", typeof(string)));
      dictProps.Add("Grating Radius Increment", new ASProperty("RadIncrement", typeof(double)));
      dictProps.Add("Grating Radius", new ASProperty("Radius", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Requisition Number", new ASProperty("RequisitionNumber", typeof(string)));
      dictProps.Add("Grating Requisition Number Used For Numbering", new ASProperty("RequisitionNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Grating Model Role", new ASProperty("Role", typeof(string)));
      dictProps.Add("Grating Model Role Description", new ASProperty("RoleDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Role Used For Numbering", new ASProperty("RoleUsedForNumbering", typeof(int)));
      dictProps.Add("Grating Shipped Date", new ASProperty("ShippedDate", typeof(string)));
      dictProps.Add("Grating Single Part Number", new ASProperty("SinglePartNumber", typeof(string)));
      dictProps.Add("Grating Single Part Prefix", new ASProperty("SinglePartPrefix", typeof(string)));
      dictProps.Add("Grating Single Part Used For BOM", new ASProperty("SinglePartUsedForBOM", typeof(int)));
      dictProps.Add("Grating Single Part Used For CollisionCheck", new ASProperty("SinglePartUsedForCollisionCheck", typeof(int)));
      dictProps.Add("Grating Single Part Used For Numbering", new ASProperty("SinglePartUsedForNumbering", typeof(int)));
      dictProps.Add("Grating SpecificGravity", new ASProperty("SpecificGravity", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Standard Hatch", new ASProperty("StandardHatch", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Supplier", new ASProperty("Supplier", typeof(string)));
      dictProps.Add("Grating SupplierUsedForNumbering", new ASProperty("SupplierUsedForNumbering", typeof(bool)));
      dictProps.Add("Grating Thickness", new ASProperty("Thickness", typeof(double)));
      dictProps.Add("Grating Thickness Of A Bearing Bar", new ASProperty("ThicknessOfABearingBar", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Width Extension Left", new ASProperty("WidthExtensionLeft", typeof(double)));
      dictProps.Add("Grating Width Extension Right", new ASProperty("WidthExtensionRight", typeof(double)));
      dictProps.Add("Grating Volume", new ASProperty("Volume", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Grating Width", new ASProperty("Width", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Change Grating Display Mode", new ASProperty("ReprMode", typeof(int), "Z_PostWriteDB"));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kGrating });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildStriaghtBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Straight Beam Property...", new ASProperty("none", typeof(string)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kUnknown });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildCompoundStraightBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Compound Beam Property...", new ASProperty("none", typeof(string)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kUnknown });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildTaperedBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Tapered Beam Property...", new ASProperty("none", typeof(string)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kUnknown });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildBentBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Bend Beam Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Bend Beam Offset Curve Radius", new ASProperty("OffsetCurveRadius", typeof(double)));
      dictProps.Add("Bend Beam Curve Offset", new ASProperty("CurveOffset", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Bend Beam Systemline Radius", new ASProperty("SystemlineRadius", typeof(double), ".", ePropertyDataOperator.Get));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBentBeam });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildCameraPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Camera Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Camera Description", new ASProperty("Description", typeof(string)));
      dictProps.Add("Camera Scale", new ASProperty("Scale", typeof(double)));
      dictProps.Add("Camera Type", new ASProperty("CameraType", typeof(string)));
      dictProps.Add("Camera 3D Coordinate System", new ASProperty("CameraCS", typeof(Matrix3d)));
      dictProps.Add("Camera SupplierUsedForNumbering", new ASProperty("DetailingFilterEnabled", typeof(bool)));
      dictProps.Add("Camera Type Description", new ASProperty("TypeDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Camera Supports Detailing Disable", new ASProperty("SupportsDetailingDisable", typeof(bool), ".", ePropertyDataOperator.Get));
      dictProps.Add("Camera Detail Style", new ASProperty("DetailStyle", typeof(int)));
      dictProps.Add("Camera Detail Style Location", new ASProperty("DetailStyleLocation", typeof(int)));
      dictProps.Add("Camera Disable Detailing", new ASProperty("DisableDetailing", typeof(bool)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kCamera });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildPlateFeaturePropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Plate Feature Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Plate Feature Boring Out Switch", new ASProperty("BoringOut", typeof(int)));
      dictProps.Add("Plate Feature Center Point", new ASProperty("CenterPoint", typeof(Point3d), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Feature Gap", new ASProperty("Gap", typeof(double)));
      dictProps.Add("Plate Feature Handle", new ASProperty("Handle", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Feature Layer", new ASProperty("Layer", typeof(string)));
      dictProps.Add("Plate Feature Length", new ASProperty("Length", typeof(double)));
      dictProps.Add("Plate Feature LengthIncrement", new ASProperty("LengthIncrement", typeof(double)));
      dictProps.Add("Plate Feature PureRole", new ASProperty("PureRole", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Feature RadIncrement", new ASProperty("RadIncrement", typeof(double)));
      dictProps.Add("Plate Feature Radius", new ASProperty("Radius", typeof(double)));
      dictProps.Add("Plate Feature Display Mode", new ASProperty("ReprMode", typeof(int)));
      dictProps.Add("Plate Feature Role", new ASProperty("Role", typeof(string)));
      dictProps.Add("Plate Feature Role Description", new ASProperty("RoleDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Feature Use Gap Switch", new ASProperty("UseGap", typeof(double)));
      dictProps.Add("Plate Feature Width", new ASProperty("Width", typeof(bool)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPlateFeatContour });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildPlateVertexFilletPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Plate Vertex Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Plate Vertex Center Point", new ASProperty("CenterPoint", typeof(Point3d), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Vertex Contour Index", new ASProperty("ContourIndex", typeof(int)));
      dictProps.Add("Plate Vertex Handle", new ASProperty("Handle", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Vertex Layer", new ASProperty("Layer", typeof(string)));
      dictProps.Add("Plate Vertex Length 1", new ASProperty("Length1", typeof(double)));
      dictProps.Add("Plate Vertex Length 2", new ASProperty("Length2", typeof(double)));
      dictProps.Add("Plate Vertex Object Index", new ASProperty("ObjectIndex", typeof(int)));
      dictProps.Add("Plate Vertex PureRole", new ASProperty("PureRole", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Vertex Radius", new ASProperty("Radius", typeof(double)));
      dictProps.Add("Plate Vertex RadIncrement", new ASProperty("RadIncrement", typeof(double)));
      dictProps.Add("Plate Vertex Display Mode", new ASProperty("ReprMode", typeof(int)));
      dictProps.Add("Plate Vertex Role", new ASProperty("Role", typeof(string)));
      dictProps.Add("Plate Vertex Role Description", new ASProperty("RoleDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Vertex Use Gap", new ASProperty("UseGap", typeof(bool)));
      dictProps.Add("Plate Vertex Vertex Index", new ASProperty("VertexIndex", typeof(short)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPlateFeatVertex });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildBeamCutPlanePropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Beam Cut Plane Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Beam Cut Plane AngleOnY", new ASProperty("AngleOnY", typeof(double)));
      dictProps.Add("Beam Cut Plane AngleOnZ", new ASProperty("AngleOnZ", typeof(double)));
      dictProps.Add("Beam Cut Plane Center Point", new ASProperty("CenterPoint", typeof(Point3d), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Cut Plane Handle", new ASProperty("Handle", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Cut Plane Cut Length", new ASProperty("InsLength", typeof(double)));
      dictProps.Add("Beam Cut Plane Layer", new ASProperty("Layer", typeof(string)));
      dictProps.Add("Beam Cut Plane PureRole", new ASProperty("PureRole", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Cut Plane Display Mode", new ASProperty("ReprMode", typeof(int)));
      dictProps.Add("Beam Cut Plane Role", new ASProperty("Role", typeof(string)));
      dictProps.Add("Beam Cut Plane Role Description", new ASProperty("RoleDescription", typeof(string), ".", ePropertyDataOperator.Get));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBeamShortening });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildBaseBeamNotchPropertyList(int listFilter, string prefix = "")
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add(prefix + "Beam Notch Corner Radius", new ASProperty("CornerRadius", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam Notch Plane Center Point", new ASProperty("CenterPoint", typeof(Point3d), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam Notch Plane Handle", new ASProperty("Handle", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam Notch Plane Cut Depth", new ASProperty("ReferenceDepth", typeof(double)));
      dictProps.Add(prefix + "Beam Notch Plane Cut Length", new ASProperty("ReferenceLength", typeof(double)));
      dictProps.Add(prefix + "Beam Notch Plane Layer", new ASProperty("Layer", typeof(string)));
      dictProps.Add(prefix + "Beam Notch Plane PureRole", new ASProperty("PureRole", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam Notch Plane Display Mode", new ASProperty("ReprMode", typeof(int)));
      dictProps.Add(prefix + "Beam Notch Plane Role", new ASProperty("Role", typeof(string)));
      dictProps.Add(prefix + "Beam Notch Plane Role Description", new ASProperty("RoleDescription", typeof(string), ".", ePropertyDataOperator.Get));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBeamNotch2Ortho, 
                    eObjectType.kBeamNotchEx,
                    eObjectType.kBeamMultiContourNotch});

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildBeamNotchOrthoPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Beam Notch Ortho Property...", new ASProperty("none", typeof(string)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kUnknown });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildBeamMultiNotchPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Beam Multi Notch Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Beam Beam Multi Boring Out", new ASProperty("BoringOut", typeof(int)));
      dictProps.Add("Beam Beam Multi Center Point", new ASProperty("CenterPoint", typeof(Point3d), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Beam Multi Coordinate System", new ASProperty("CS", typeof(Matrix3d)));
      dictProps.Add("Beam Beam Multi Gap", new ASProperty("Gap", typeof(double)));
      dictProps.Add("Beam Beam Multi Handle", new ASProperty("Handle", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Beam Multi Layer", new ASProperty("Layer", typeof(string)));
      dictProps.Add("Beam Beam Multi Length", new ASProperty("Length", typeof(double)));
      dictProps.Add("Beam Beam Multi Length Increment", new ASProperty("LengthIncrement", typeof(double)));
      dictProps.Add("Beam Beam Multi Normal Vector", new ASProperty("Normal", typeof(Vector3d), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Beam Multi Radius Increment", new ASProperty("RadIncrement", typeof(double)));
      dictProps.Add("Beam Beam Multi Radius", new ASProperty("Radius", typeof(double)));
      dictProps.Add("Beam Beam Multi Display Mode", new ASProperty("ReprMode", typeof(int)));
      dictProps.Add("Beam Beam Multi Role", new ASProperty("Role", typeof(string)));
      dictProps.Add("Beam Beam Multi Role Description", new ASProperty("RoleDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Beam Multi PureRole", new ASProperty("PureRole", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Beam Multi Use Gap", new ASProperty("UseGap", typeof(bool)));
      dictProps.Add("Beam Beam Multi Width", new ASProperty("Width", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBeamMultiContourNotch });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildPlateNotchContourPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Plate Notch Contour Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Beam Plate Notch Contour Boring Out", new ASProperty("BoringOut", typeof(int)));
      dictProps.Add("Beam Plate Notch Contour Center Point", new ASProperty("CenterPoint", typeof(Point3d), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Plate Notch Contour Coordinate System", new ASProperty("CS", typeof(Matrix3d)));
      dictProps.Add("Beam Plate Notch Contour Gap", new ASProperty("Gap", typeof(double)));
      dictProps.Add("Beam Plate Notch Contour Handle", new ASProperty("Handle", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Plate Notch Contour Layer", new ASProperty("Layer", typeof(string)));
      dictProps.Add("Beam Plate Notch Contour Length", new ASProperty("Length", typeof(double)));
      dictProps.Add("Beam Plate Notch Contour Length Increment", new ASProperty("LengthIncrement", typeof(double)));
      dictProps.Add("Beam Plate Notch Contour Normal Vector", new ASProperty("Normal", typeof(Vector3d), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Plate Notch Contour Radius Increment", new ASProperty("RadIncrement", typeof(double)));
      dictProps.Add("Beam Plate Notch Contour Radius", new ASProperty("Radius", typeof(double)));
      dictProps.Add("Beam Plate Notch Contour Display Mode", new ASProperty("ReprMode", typeof(int)));
      dictProps.Add("Beam Plate Notch Contour Role", new ASProperty("Role", typeof(string)));
      dictProps.Add("Beam Plate Notch Contour Role Description", new ASProperty("RoleDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Plate Notch Contour PureRole", new ASProperty("PureRole", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Plate Notch Contour Use Gap", new ASProperty("UseGap", typeof(bool)));
      dictProps.Add("Beam Plate Notch Contour Width", new ASProperty("Width", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kPlateFeatContour });
      
      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildBeamNotchRotatedPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Beam Notch Rotated Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Beam Notch Axis Angle", new ASProperty("AxisAngle", typeof(double)));
      dictProps.Add("Beam Notch Z Angle", new ASProperty("ZAngle", typeof(double)));
      dictProps.Add("Beam Notch X Angle", new ASProperty("XAngle", typeof(double)));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kBeamNotchEx });

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildCompundBaseBeamPropertyList(int listFilter, string prefix = "")
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Use " + prefix + "Beam As One Beam", new ASProperty("UseCompoundAsOneBeam", typeof(bool)));
      dictProps.Add(prefix + "Beam ClassName", new ASProperty("CompoundClassName", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add(prefix + "Beam TypeName", new ASProperty("CompoundTypeName", typeof(string), ".", ePropertyDataOperator.Get));

      addElementTypes(dictProps, new List<eObjectType>() {
                    eObjectType.kCompoundStraightBeam });

      return filterDictionary(dictProps, listFilter);
    }

    public static void CheckListUpdateOrAddValue(List<ASProperty> listOfPropertyData, 
                                                  string propName, 
                                                  object propValue, 
                                                  string propLevel = "",
                                                  int propertyDataOp = 6)
    {
      var foundItem = listOfPropertyData.FirstOrDefault<ASProperty>(props => props.PropName == propName);
      if (foundItem != null)
      {
        foundItem.PropValue = propValue;
      }
      else
      {
        listOfPropertyData.Add(new ASProperty(propName, propValue, propValue.GetType(), propLevel, propertyDataOp));
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.CountableScrewBoltPattern objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.PlateContourNotch objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.PlateFeatContour objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }
    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.BeamShortening objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.BeamNotch2Ortho objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.BeamNotchEx objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }
    
    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.BeamMultiContourNotch objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }
    
    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.PlateFeatVertFillet objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.ConstructionHelper.Camera objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }
    
    public static void SetParameters(Autodesk.AdvanceSteel.Arrangement.Arranger objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.ConstructionTypes.AtomicElement objToMod, List<ASProperty> properties)
    {
      if (properties != null) 
      { 
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.AnchorPattern objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }
  }
}
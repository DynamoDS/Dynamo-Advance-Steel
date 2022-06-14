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

    static public Autodesk.AdvanceSteel.Geometry.Point3d ToAstPoint(this Autodesk.DesignScript.Geometry.Point pt)
    {
      return ToAstPoint(pt, true);
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

    static public Double ToInternalUnits(this double value, eUnitType unitType)
    {
      return ToInternalUnits(value, unitType, true);
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

    static public Double FromInternalUnits(this double value, eUnitType unitType)
    {
      return FromInternalUnits(value, unitType, true);
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

    static public Double FromInternalDistanceUnits(this double value)
    {
      return FromInternalDistanceUnits(value, true);
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

    public static Autodesk.DesignScript.Geometry.PolyCurve ToDynPolyCurve(this Autodesk.AdvanceSteel.Geometry.Polyline3d poly)
    {
      var listCurves = ToDynPolyCurves(poly, true);

      return Autodesk.DesignScript.Geometry.PolyCurve.ByJoinedCurves(listCurves);
    }
    static public List<Autodesk.DesignScript.Geometry.Curve> ToDynPolyCurves(Autodesk.AdvanceSteel.Geometry.Polyline3d poly, bool bConvertFromAstUnits)
    {
      List<Autodesk.DesignScript.Geometry.Curve> retData = new List<Autodesk.DesignScript.Geometry.Curve>();
      poly.GetCurves(out Curve3d[] foundPolyCurves);

      foreach (Curve3d curve in foundPolyCurves)
      {
        if (curve is LineSeg3d)
        {
          LineSeg3d line = curve as LineSeg3d;

          line.HasStartPoint(out var lStartPoint);
          line.HasEndPoint(out var lEndPoint);

          retData.Add(Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(Utils.ToDynPoint(lStartPoint, bConvertFromAstUnits), Utils.ToDynPoint(lEndPoint, bConvertFromAstUnits)));
        }
        else if (curve is CircArc3d)
        {
          CircArc3d arc = curve as CircArc3d;
          if (arc != null)
          {
            retData.Add(Autodesk.DesignScript.Geometry.Arc.ByCenterPointStartPointEndPoint(Utils.ToDynPoint(arc.Center, bConvertFromAstUnits), 
              Utils.ToDynPoint(arc.StartPoint, bConvertFromAstUnits), Utils.ToDynPoint(arc.EndPoint, bConvertFromAstUnits)));
          }
        }
      }

      return retData;
    }

    static public Autodesk.AdvanceSteel.Geometry.Polyline3d ToAstPolyline3dOpened(this Autodesk.DesignScript.Geometry.PolyCurve poly)
    {
      return ToAstPolyline3dOpened(poly,  true);
    }

    static public Autodesk.AdvanceSteel.Geometry.Polyline3d ToAstPolyline3dOpened(Autodesk.DesignScript.Geometry.PolyCurve poly, bool bConvertToAstUnits)
    {
      var curves = poly.Curves();

      Polyline3d newReturnPoly = new Polyline3d();
      Point3d[] PolyPoint = new Point3d[poly.NumberOfCurves + 1];
      VertexInfo[] PolyVertexs = new VertexInfo[poly.NumberOfCurves + 1];

      int count = 0;
      foreach (var curve in curves)
      {  
        if (curve is Autodesk.DesignScript.Geometry.Line)
        {
          var line = curve as Autodesk.DesignScript.Geometry.Line;

          PolyPoint[count] = Utils.ToAstPoint(line.StartPoint, bConvertToAstUnits);
          PolyVertexs[count] = new VertexInfo();
        }
        else if (curve is Autodesk.DesignScript.Geometry.Arc)
        {
          var arc = curve as Autodesk.DesignScript.Geometry.Arc;

          PolyPoint[count] = Utils.ToAstPoint(arc.StartPoint, bConvertToAstUnits);
          PolyVertexs[count] = new VertexInfo(Utils.FromInternalDistanceUnits(arc.Radius, bConvertToAstUnits), Utils.ToAstPoint(arc.CenterPoint, bConvertToAstUnits), Utils.ToAstVector3d(arc.Normal, bConvertToAstUnits));
        }

        count++;

        if (count == poly.NumberOfCurves)
        {
          PolyPoint[count] = Utils.ToAstPoint(curve.EndPoint, bConvertToAstUnits);
          PolyVertexs[count] = new VertexInfo();
        }
      }

      newReturnPoly.CreateFrom(PolyPoint, PolyVertexs, false, true);

      return newReturnPoly;
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

    static public Autodesk.DesignScript.Geometry.Point ToDynPoint(this Autodesk.AdvanceSteel.Geometry.Point3d pt)
    {
      return ToDynPoint(pt, true);
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

    static public Autodesk.DesignScript.Geometry.Vector ToDynVector(this Autodesk.AdvanceSteel.Geometry.Vector3d vect)
    {
      return ToDynVector(vect, true);
    }

    static public Autodesk.DesignScript.Geometry.Vector ToDynVector(this Autodesk.AdvanceSteel.Geometry.Vector2d vect)
    {
      return ToDynVector(new Vector3d(vect.x, vect.y, 0), true);
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

    static public Autodesk.DesignScript.Geometry.CoordinateSystem ToDynCoordinateSys(this Autodesk.AdvanceSteel.Geometry.Matrix3d matrix)
    {
      return ToDynCoordinateSys(matrix, true);
    }

    static public Autodesk.DesignScript.Geometry.CoordinateSystem ToDynCoordinateSys(Autodesk.AdvanceSteel.Geometry.Matrix3d matrix, bool bConvertToAstUnits)
    {
      Autodesk.AdvanceSteel.Geometry.Point3d origin = new Point3d();
      Autodesk.AdvanceSteel.Geometry.Vector3d xAxis = new Autodesk.AdvanceSteel.Geometry.Vector3d();
      Autodesk.AdvanceSteel.Geometry.Vector3d yAxis = new Autodesk.AdvanceSteel.Geometry.Vector3d();
      Autodesk.AdvanceSteel.Geometry.Vector3d zAxis = new Autodesk.AdvanceSteel.Geometry.Vector3d();
      matrix.GetCoordSystem(out origin, out xAxis, out yAxis, out zAxis);

      //Try the vectors
      if (xAxis.IsZeroLength() || yAxis.IsZeroLength() || zAxis.IsZeroLength())
      {
        throw new Exception("Error converting Coordinate");
      }

      return Autodesk.DesignScript.Geometry.CoordinateSystem.ByOriginVectors(ToDynPoint(origin, bConvertToAstUnits), ToDynVector(xAxis, bConvertToAstUnits),
        ToDynVector(yAxis, bConvertToAstUnits), ToDynVector(zAxis, bConvertToAstUnits));
    }

    static public Autodesk.AdvanceSteel.Geometry.Vector3d ToAstVector3d(this Autodesk.DesignScript.Geometry.Vector v)
    {
      return ToAstVector3d(v, true);
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

    static public Autodesk.AdvanceSteel.Geometry.Matrix3d ToAstMatrix3d(this Autodesk.DesignScript.Geometry.CoordinateSystem cs)
    {
      return ToAstMatrix3d(cs, true);
    }

    static public Autodesk.AdvanceSteel.Geometry.Matrix3d ToAstMatrix3d(Autodesk.DesignScript.Geometry.CoordinateSystem cs, bool bConvertToAstUnits)
    {
      Autodesk.AdvanceSteel.Geometry.Matrix3d matrix = new Autodesk.AdvanceSteel.Geometry.Matrix3d();
      matrix.SetCoordSystem(ToAstPoint(cs.Origin, bConvertToAstUnits), ToAstVector3d(cs.XAxis, bConvertToAstUnits), ToAstVector3d(cs.YAxis, bConvertToAstUnits), ToAstVector3d(cs.ZAxis, bConvertToAstUnits));
      return matrix;
    }

    static public Autodesk.AdvanceSteel.Geometry.Plane ToAstPlane(this Autodesk.DesignScript.Geometry.Plane dynPlane)
    {
      return ToAstPlane(dynPlane, true);
    }

    static public Autodesk.AdvanceSteel.Geometry.Plane ToAstPlane(Autodesk.DesignScript.Geometry.Plane dynPlane, bool bConvertToAstUnits)
    {
      Point3d planeOrigin = ToAstPoint(dynPlane.Origin, bConvertToAstUnits);
      Vector3d planeNormal = ToAstVector3d(dynPlane.Normal, bConvertToAstUnits);
      Autodesk.AdvanceSteel.Geometry.Plane plane = new Autodesk.AdvanceSteel.Geometry.Plane(planeOrigin, planeNormal);
      return plane;
    }

    static public Autodesk.DesignScript.Geometry.Plane ToDynPlane(this Autodesk.AdvanceSteel.Geometry.Plane astPlane)
    {
      return ToDynPlane(astPlane, true);
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

    public static Dictionary<string, Type> GetASObjectFilters()
    {
      return (from entry in UtilsProperties.SteelObjectPropertySets orderby entry.Value.Description ascending select entry).ToDictionary(x => x.Value.Description, y => y.Key);
    }

    public static List<Type> GetASObjectTypeFilters(List<string> listFilter)
    {
      return UtilsProperties.SteelObjectPropertySets.Where(x => listFilter.Contains(x.Value.Description)).Select(x => x.Key).ToList();
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
            throw new System.Exception("No Advance Steel Object Found");
          }

          if (!UtilsProperties.CheckType(obj.GetType()))
          {
            continue;
          }

          SteelDbObject foundSteelObj = obj.ToDSType();
          retListOfSteelObjects.Add(foundSteelObj);
        }
      }
      return retListOfSteelObjects;
    }

    public static IEnumerable<SteelDbObject> GetDynObjectsByType(string objectSelectionFilter)
    {
      var typeFilter = UtilsProperties.SteelObjectPropertySets.FirstOrDefault(x => objectSelectionFilter.Equals(x.Value.Description)).Value.ASType;

      var retListOfSteelObjects = new List<SteelDbObject>();
      using (var ctx = new DocContext())
      {
        List<string> ret = new List<string>() { };

        ObjectId[] OIDxs = null;

        DatabaseManager.GetModelObjectIds(out OIDxs);
        List<ObjectId> OIDx = OIDxs.ToList<ObjectId>();
        if (OIDx.Count == 0)
        {
          return retListOfSteelObjects;
        }

        for (int i = 0; i < OIDx.Count; i++)
        {
          FilerObject obj = FilerObject.GetFilerObject(OIDx[i]);

          if (obj != null && (obj.GetType().IsSubclassOf(typeFilter) || obj.GetType().IsEquivalentTo(typeFilter)))
          {
            SteelDbObject foundSteelObj = obj.ToDSType();
            retListOfSteelObjects.Add(foundSteelObj);
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

  }
}
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modeler;
using System;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Util
{
  /// <summary>
  /// Custom User attributes for Advance Steel elements
  /// </summary>
  public class Geometry
  {

    /// <summary>
    /// Get line segments of
    /// </summary>
    /// <param name="steelObject"></param>
    /// <param name="bodyResolution"></param>
    /// <param name="intersectionPlane"></param>
    /// <returns></returns>
    public static List<Autodesk.DesignScript.Geometry.Line> CutElementByPlane(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                                                              int bodyResolution,
                                                                              Autodesk.DesignScript.Geometry.Plane intersectionPlane)
    {
      List<Autodesk.DesignScript.Geometry.Line> ret = new List<Autodesk.DesignScript.Geometry.Line>() { };


      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null  || intersectionPlane != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          Plane cutPlane = Utils.ToAstPlane(intersectionPlane, true);
          if (filerObj != null)
          {
            AtomicElement selectedObj = filerObj as AtomicElement;

            ModelerBody modelerTestBody = selectedObj.GetModeler((BodyContext.eBodyContext)bodyResolution);
            LineSeg3d[] segs = null;

            modelerTestBody.IntersectWith(cutPlane, out segs);
            for (int i = 0; i < segs.Length; i++)
            {
              Autodesk.DesignScript.Geometry.Point dynStartPoint = Utils.ToDynPoint(segs[i].StartPoint, true);
              Autodesk.DesignScript.Geometry.Point dynEndPoint = Utils.ToDynPoint(segs[i].EndPoint, true);
              ret.Add(Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(dynStartPoint, dynEndPoint));
            }
          }
        }
        else
          throw new System.Exception("No Steel Object found or Plane is Null");
      }
      return ret;
    }
    public static List<Autodesk.DesignScript.Geometry.Point> IntersectElementByLine(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                                                                    int bodyResolution,
                                                                                    Autodesk.DesignScript.Geometry.Line line)
    {
      List<Autodesk.DesignScript.Geometry.Point> ret = new List<Autodesk.DesignScript.Geometry.Point>() { };


      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null || line != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          Line3d projectedLine = Utils.ToAstLine3D(line, true);
          Point3d originPoint = Utils.ToAstPoint(line.StartPoint, true);
          if (filerObj != null)
          {
            AtomicElement selectedObj = filerObj as AtomicElement;
            Point3d[] foundPoints = null;

            ModelerBody modelerTestBody = selectedObj.GetModeler((BodyContext.eBodyContext)bodyResolution);

            modelerTestBody.IntersectWith(projectedLine, out foundPoints);
            List<MyPoint> sortedPoints = new List<MyPoint>() { };
            for (int i = 0; i < foundPoints.Length; i++)
            {
              sortedPoints.Add(new MyPoint(originPoint, foundPoints[i]));
            }

            List<MyPoint> sortedByDistance = sortedPoints.OrderByDescending(Ptx => Ptx.Distance).ToList();

            for (int i = 0; i < sortedByDistance.Count; i++)
            {
              ret.Add(Utils.ToDynPoint(sortedByDistance[i].Point, true));
            }

          }
        }
        else
          throw new System.Exception("No Steel Object found or Line Object is null");
      }
      return ret;
    }

    private class MyPoint
    {
      private Point3d _Point;
      private Point3d _OriginPoint;
      private double _Distance;
      private double _ZValue;

      public MyPoint(Point3d originPoint, Point3d point)
      {
        _Point = point;
        _OriginPoint = originPoint;
        _Distance = originPoint.DistanceTo(point);
        _ZValue = point.z;
      }

      public Point3d Point
      {
        get
        {
          return _Point;
        }
      }

      public double Distance
      {
        get
        {
          return _Distance;
        }
      }
      public double ZValue
      {
        get
        {
          return _ZValue;
        }
      }
    }
  }

}
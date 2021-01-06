using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modeler;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Util
{
  /// <summary>
  /// Geometric functions to work with steel objects to return construction geometry
  /// </summary>
  public class Geometry
  {

    internal Geometry()
    {
    }
    /// <summary>
    /// Get line segments of steel body that interected with plane
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <param name="bodyResolution"> Set Steel body display resolution</param>
    /// <param name="intersectionPlane"> Dynamo Plane to intersect with Steel body</param>
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
          else
            throw new System.Exception("No Object found via registered handle");
        }
        else
          throw new System.Exception("No Steel Object found or Plane is Null");
      }
      return ret;
    }

    /// <summary>
    /// Get intersection point of Steel object system line with Dynamo plane
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <param name="intersectionPlane"> Dynamo Plane to intersect with Steel body</param>
    /// <returns></returns>
    public static Autodesk.DesignScript.Geometry.Point CutSystemLineByPlane(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                                                          Autodesk.DesignScript.Geometry.Plane intersectionPlane)
    {
      Autodesk.DesignScript.Geometry.Point ret = Autodesk.DesignScript.Geometry.Point.ByCoordinates(0,0,0);
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null || intersectionPlane != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          Plane cutPlane = Utils.ToAstPlane(intersectionPlane, true);
          if (filerObj != null)
          {
            AtomicElement selectedObj = filerObj as AtomicElement;

            if (selectedObj.IsKindOf(FilerObject.eObjectType.kBeam))
            {
              Beam passedBeam = selectedObj as Beam;
              Line3d line = new Line3d(passedBeam.GetPointAtStart(), passedBeam.GetPointAtEnd());
              Point3d[] intPts = new Point3d[] { };
              cutPlane.IntersectWith(line, ref intPts, new Tol());

              if (intPts.Length > 0)
              {
                ret = Utils.ToDynPoint(intPts[0], true);
              }
              else
                throw new System.Exception("No Intersection point found on steel object with current plane");
            }
          }
          else
            throw new System.Exception("No Object found via registered handle");
        }
        else
          throw new System.Exception("No Steel Object found or Plane is Null");
      }
      return ret;
    }

    /// <summary>
    /// Get points on the steel body that interected with line
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <param name="bodyResolution"> Set Steel body display resolution</param>
    /// <param name="line"> Dynamo Line to intersect with Steel body</param>
    /// <returns></returns>
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
            if (modelerTestBody.IntersectWith(projectedLine, out foundPoints))
            {
              foundPoints = foundPoints.OrderByDescending(Ptx => Ptx.DistanceTo(originPoint)).ToArray();
              for (int i = 0; i < foundPoints.Length; i++)
              {
                ret.Add(Utils.ToDynPoint(foundPoints[i], true));
              }
            }
          }
          else
            throw new System.Exception("No Object found via registered handle");
        }
        else
          throw new System.Exception("No Steel Object found or Line Object is null");
      }
      return ret;
    }

    /// <summary>
    /// Get Polycurve from Polybeam
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <returns></returns>
    public static Autodesk.DesignScript.Geometry.PolyCurve GetPolyCurve(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      List<Autodesk.DesignScript.Geometry.Curve> intRet = new List<Autodesk.DesignScript.Geometry.Curve>() { };
      Autodesk.DesignScript.Geometry.PolyCurve ret = null; 
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kPolyBeam))
            {
              PolyBeam selectedObj = filerObj as PolyBeam;
              Polyline3d poly = selectedObj.GetPolyline();
              intRet = Utils.ToDynPolyCurves(poly, true);
              ret = Autodesk.DesignScript.Geometry.PolyCurve.ByJoinedCurves(intRet);
            }
            else
              throw new System.Exception("Wrong type of Steel Object found, must be a Polybeam");
          }
          else
            throw new System.Exception("No Object found via registered handle");
        }
        else
          throw new System.Exception("No Steel Object found");
      }
      return ret;
    }

    /// <summary>
    /// Get Plane from planar objects like - plate, grating, slab or isolated footing
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <returns></returns>
    public static Autodesk.DesignScript.Geometry.Plane GetPlane(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      Autodesk.DesignScript.Geometry.Plane ret = null;
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kPlate) ||
                filerObj.IsKindOf(FilerObject.eObjectType.kSlab) ||
                filerObj.IsKindOf(FilerObject.eObjectType.kFootingIsolated) ||
                filerObj.IsKindOf(FilerObject.eObjectType.kGrating))
            {
              PlateBase selectedObj = filerObj as PlateBase;
              Plane plane = selectedObj.DefinitionPlane;
              ret = Utils.ToDynPlane(plane, true);
            }
            else
              throw new System.Exception("Wrong type of Steel Object found, must be a Plate, Grating, Slab or Footing Isolated - Planner Object");
          }
          else
            throw new System.Exception("No Object found via registered handle");
        }
        else
          throw new System.Exception("No Steel Object found");
      }
      return ret;
    }

    /// <summary>
    /// Set the origin of a plane to a new point
    /// </summary>
    /// <param name="plane"> Input Orginal Dynamo Plane</param>
    /// <param name="point"> Input Dynamo Point to set origin of plane</param>
    /// <returns></returns>
    public static Autodesk.DesignScript.Geometry.Plane SetPlaneOrigin(Autodesk.DesignScript.Geometry.Plane plane,
                                                                      Autodesk.DesignScript.Geometry.Point point)
    {
      Autodesk.DesignScript.Geometry.Plane ret = null;
      using (var ctx = new SteelServices.DocContext())
      {
        if (point != null && plane != null)
        {
          ret = Autodesk.DesignScript.Geometry.Plane.ByOriginNormal(point, plane.Normal);
        }
        else
          throw new System.Exception("No Input objects found");
      }
      return ret;
    }

    /// <summary>
    /// Project point by a value in a direction (Vector)
    /// </summary>
    /// <param name="point"> Input Orginal Dynamo Point</param>
    /// <param name="direction"> Input Dynamo Direction Vector</param>
    /// <param name="distance"> Input value to move point in direction vector</param>
    /// <returns></returns>
    public static Autodesk.DesignScript.Geometry.Point GetPointInDirection(Autodesk.DesignScript.Geometry.Point point, 
                                                                            Autodesk.DesignScript.Geometry.Vector direction,
                                                                            double distance)
    {
      Autodesk.DesignScript.Geometry.Point ret = null;
      using (var ctx = new SteelServices.DocContext())
      {
        Point3d newPoint = Utils.ToAstPoint(point, true);
        Vector3d vector = Utils.ToAstVector3d(direction, true);
        newPoint = newPoint.Add(vector * Utils.ToInternalUnits(distance, true));
        ret = Utils.ToDynPoint(newPoint, true);
      }
      return ret;
    }

    /// <summary>
    /// Project point by value in directional vector to get a new plane
    /// </summary>
    /// <param name="point"> Input Orginal Dynamo Point</param>
    /// <param name="planeNormal"> Input Dynamo Direction Vector and Plane Normal</param>
    /// <param name="distance"> Input value to move point in direction vector</param>
    /// <returns></returns>
    public static Autodesk.DesignScript.Geometry.Plane GetPlaneInDirection(Autodesk.DesignScript.Geometry.Point point,
                                                                          Autodesk.DesignScript.Geometry.Vector planeNormal,
                                                                          double distance)
    {
      Autodesk.DesignScript.Geometry.Plane ret = null;
      using (var ctx = new SteelServices.DocContext())
      {
        Point3d newPoint = Utils.ToAstPoint(point, true);
        Vector3d vector = Utils.ToAstVector3d(planeNormal, true);
        newPoint = newPoint.Add(vector * Utils.ToInternalUnits(distance, true));
        ret = Autodesk.DesignScript.Geometry.Plane.ByOriginNormal(Utils.ToDynPoint(newPoint, true), planeNormal);
      }
      return ret;
    }

    /// <summary>
    /// Creates one plane either side of the centre point in relation to the normal
    /// </summary>
    /// <param name="centrePoint"> Input Dynamo Centre Point between created planes</param>
    /// <param name="planeNormal"> Input Dynamo Direction Vector and Plane Normal</param>
    /// <param name="distance"> Input value to move point in direction vector</param>
    /// <returns></returns>
    public static List<Autodesk.DesignScript.Geometry.Plane> GetPlaneOffsetByCentre(Autodesk.DesignScript.Geometry.Point centrePoint,
                                                                                    Autodesk.DesignScript.Geometry.Vector planeNormal,
                                                                                    double distance)
    {
      List<Autodesk.DesignScript.Geometry.Plane> ret = new List<Autodesk.DesignScript.Geometry.Plane>() { };
      using (var ctx = new SteelServices.DocContext())
      {
        Point3d orginPoint = Utils.ToAstPoint(centrePoint, true);
        Vector3d posVector = Utils.ToAstVector3d(planeNormal, true);
        Vector3d negVector = new Vector3d(posVector).Negate();
        Point3d posPoint = new Point3d(orginPoint).Add(posVector * Utils.ToInternalUnits(distance, true));
        Point3d negPoint = new Point3d(orginPoint).Add(negVector * Utils.ToInternalUnits(distance, true));
        ret.Add(Autodesk.DesignScript.Geometry.Plane.ByOriginNormal(Utils.ToDynPoint(posPoint, true), planeNormal));
        ret.Add(Autodesk.DesignScript.Geometry.Plane.ByOriginNormal(Utils.ToDynPoint(negPoint, true), planeNormal));
      }
      return ret;
    }

    /// <summary>
    /// Ortho Project point to Plane
    /// </summary>
    /// <param name="point"> Input Orginal Dynamo Point</param>
    /// <param name="projectionPlane"> Input Dynamo Plane to project point onto</param>
    /// <returns></returns>
    public static Autodesk.DesignScript.Geometry.Point OrthoProjectPointToPlane(Autodesk.DesignScript.Geometry.Point point,
                                                                                Autodesk.DesignScript.Geometry.Plane projectionPlane)
    {
      Autodesk.DesignScript.Geometry.Point ret = null;
      using (var ctx = new SteelServices.DocContext())
      {
        Point3d newPoint = Utils.ToAstPoint(point, true);
        Plane plane = Utils.ToAstPlane(projectionPlane, true);
        newPoint = newPoint.OrthoProject(plane);
        ret = Utils.ToDynPoint(newPoint, true);
      }
      return ret;
    }

    /// <summary>
    /// Othro Project Point to Line
    /// </summary>
    /// <param name="point"> Input Orginal Dynamo Point</param>
    /// <param name="line"></param>
    /// <returns></returns>
    [MultiReturn(new[] { "FoundPoint", "IsOnline" })]
    public static Dictionary<string, object> OrthoProjectPointToLine(Autodesk.DesignScript.Geometry.Point point,
                                                                                Autodesk.DesignScript.Geometry.Line line)
    {
      Dictionary<string, object> ret = new Dictionary<string, object>();
      using (var ctx = new SteelServices.DocContext())
      {
        Point3d projectionPoint = Utils.ToAstPoint(point, true);
        Point3d sp = Utils.ToAstPoint(line.StartPoint, true);
        Point3d ep = Utils.ToAstPoint(line.EndPoint, true);
        Vector3d vec = projectionPoint.Subtract(sp);
        Vector3d vecLine = ep.Subtract(sp);
        double angle = vec.GetAngleTo(vecLine);
        if (angle > (Math.PI / 2))
        {
          angle = Math.Abs(Math.PI - angle);
        }
        double lineLen = line.Length;
        double distToProjPoint = sp.DistanceTo(projectionPoint);
        double distanceToProjPoint = distToProjPoint * Math.Cos(angle); ;
        double checkDistance = lineLen / Math.Cos(angle);
        Point3d calculatedPoint = new Point3d(sp).Add(distanceToProjPoint * vecLine.Normalize());
        bool isOnLine = false;
        if (Math.Round(distToProjPoint, 6) <= Math.Round(checkDistance, 6))
        {
          isOnLine = true;
        }
        ret["FoundPoint"] = Utils.ToDynPoint(calculatedPoint, true);
        ret["IsOnline"] = isOnLine; 
      }
      return ret;
    }

    /// <summary>
    /// Get point in the middle of two other points
    /// </summary>
    /// <param name="firstPoint"> Input Dynamo First Point</param>
    /// <param name="secondPoint"> Input Dynamo Second Point</param>
    /// <returns></returns>
    public static Autodesk.DesignScript.Geometry.Point GetMidBetweenPoints(Autodesk.DesignScript.Geometry.Point firstPoint,
                                                                            Autodesk.DesignScript.Geometry.Point secondPoint)
    {
      Autodesk.DesignScript.Geometry.Point ret = null;
      using (var ctx = new SteelServices.DocContext())
      {
        Point3d sPoint = Utils.ToAstPoint(firstPoint, true);
        Point3d ePoint = Utils.ToAstPoint(secondPoint, true);
        
        Point3d foundPoint = Utils.GetMidPointBetween(sPoint, ePoint);

        ret = Utils.ToDynPoint(foundPoint, true);
      }
      return ret;
    }
  }
}
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
    /// <returns name="lines"> list of lines that form the section created by passing the steel object through the plane</returns>
    public static List<Autodesk.DesignScript.Geometry.Line> CutElementByPlane(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                                                              int bodyResolution,
                                                                              Autodesk.DesignScript.Geometry.Plane intersectionPlane)
    {
      List<Autodesk.DesignScript.Geometry.Line> ret = new List<Autodesk.DesignScript.Geometry.Line>() { };
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null || intersectionPlane != null)
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
    /// <returns name="point"> point formed by cutting the system line through a plane</returns>
    public static Autodesk.DesignScript.Geometry.Point CutSystemLineByPlane(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                                                          Autodesk.DesignScript.Geometry.Plane intersectionPlane)
    {
      Autodesk.DesignScript.Geometry.Point ret = Autodesk.DesignScript.Geometry.Point.ByCoordinates(0, 0, 0);
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
              Autodesk.AdvanceSteel.Modelling.Beam passedBeam = selectedObj as Autodesk.AdvanceSteel.Modelling.Beam;
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
    /// <returns name="points"> list of points that are returned by intersction a line through a steel object based on the object display mode / resolution</returns>
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
    /// Get Plane from planar objects like - plate, grating, slab or isolated footing
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <returns name="plane"> returns a plane from plannar objects - plate, slab, footing isolated or grating</returns>
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
    /// Creates one plane either side of the centre point in relation to the normal
    /// </summary>
    /// <param name="centrePoint"> Input Dynamo Centre Point between created planes</param>
    /// <param name="planeNormal"> Input Dynamo Direction Vector and Plane Normal</param>
    /// <param name="distance"> Input value to move point in direction vector</param>
    /// <returns name="PlaneNegative"> returns 2 equal spaced planes, one plane plane either side of the centre point by the distance value</returns>
    [MultiReturn(new[] { "PlaneNegative", "PlanePositive" })]
    public static Dictionary<string, Autodesk.DesignScript.Geometry.Plane> GetPlaneOffsetByCentre(Autodesk.DesignScript.Geometry.Point centrePoint,
                                                                                    Autodesk.DesignScript.Geometry.Vector planeNormal,
                                                                                    double distance)
    {
      Dictionary<string, Autodesk.DesignScript.Geometry.Plane> ret = new Dictionary<string, Autodesk.DesignScript.Geometry.Plane>();
      using (var ctx = new SteelServices.DocContext())
      {
        Point3d orginPoint = Utils.ToAstPoint(centrePoint, true);
        Vector3d posVector = Utils.ToAstVector3d(planeNormal, true);
        Vector3d negVector = new Vector3d(posVector).Negate();
        Point3d posPoint = new Point3d(orginPoint).Add(posVector * Utils.ToInternalDistanceUnits(distance, true));
        Point3d negPoint = new Point3d(orginPoint).Add(negVector * Utils.ToInternalDistanceUnits(distance, true));
        ret["PlanePositive"] = (Autodesk.DesignScript.Geometry.Plane.ByOriginNormal(Utils.ToDynPoint(posPoint, true), planeNormal));
        ret["PlaneNegative"] = (Autodesk.DesignScript.Geometry.Plane.ByOriginNormal(Utils.ToDynPoint(negPoint, true), planeNormal));
      }
      return ret;
    }

    /// <summary>
    /// Othro Project Point to Line
    /// </summary>
    /// <param name="point"> Input Orginal Dynamo Point</param>
    /// <param name="line"></param>
    /// <returns name="FoundPoint"> orthographically project point to a line and returns found point and also if the point is on the extents of the line </returns>
    [MultiReturn(new[] { "FoundPoint", "IsOnLine" })]
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
        ret["IsOnLine"] = isOnLine;
      }
      return ret;
    }

    /// <summary>
    /// Get point in the middle of two other points
    /// </summary>
    /// <param name="firstPoint"> Input Dynamo First Point</param>
    /// <param name="secondPoint"> Input Dynamo Second Point</param>
    /// <returns name="point"> get the point midway between two points</returns>
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
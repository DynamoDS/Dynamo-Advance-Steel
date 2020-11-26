using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modeler;
using Autodesk.AdvanceSteel.Modelling;
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
    /// <param name="steelObject">Advance Steel element</param>
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
        }
        else
          throw new System.Exception("No Steel Object found or Plane is Null");
      }
      return ret;
    }

    /// <summary>
    /// Get intersection point of Steel object system line with Dynamo plane
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
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
        }
        else
          throw new System.Exception("No Steel Object found or Plane is Null");
      }
      return ret;
    }

    /// <summary>
    /// Get points on the steel body that interected with line
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
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
        }
        else
          throw new System.Exception("No Steel Object found or Line Object is null");
      }
      return ret;
    }
  }
}
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.AdvanceSteel.Profiles;
using Autodesk.DesignScript.Runtime;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Concrete
{
  /// <summary>
  /// Advance Steel straight beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class ConcStraightBeam : GraphicObject
  {
    internal ConcStraightBeam(string concName, Autodesk.DesignScript.Geometry.Point ptStart, Autodesk.DesignScript.Geometry.Point ptEnd, Autodesk.DesignScript.Geometry.Vector vOrientation)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Point3d beamStart = Utils.ToAstPoint(ptStart, true);
          Point3d beamEnd = Utils.ToAstPoint(ptEnd, true);
          Vector3d refVect = Utils.ToAstVector3d(vOrientation, true);

          Autodesk.AdvanceSteel.Modelling.ConcreteBeam concBeam = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            concBeam = new Autodesk.AdvanceSteel.Modelling.ConcreteBeam(concName, beamStart, beamEnd, refVect);
            concBeam.WriteToDb();
          }
          else
          {
            concBeam = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.ConcreteBeam;

            if (concBeam != null && concBeam.IsKindOf(FilerObject.eObjectType.kConcreteBeam))
            {
              Utils.AdjustBeamEnd(concBeam, beamStart);
              concBeam.SetSysStart(beamStart);
              concBeam.SetSysEnd(beamEnd);
              concBeam.ProfName = concName;

              Utils.SetOrientation(concBeam, refVect);
            }
            else
              throw new System.Exception("Not a Concrete Straight Beam");
          }
          Handle = concBeam.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(concBeam);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel Concrete Straight beam between two points
    /// </summary>
    /// <param name="concName"></param>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    /// <param name="orientation">Section orientation</param>
    /// <returns></returns>
    public static ConcStraightBeam ByStartPointEndPoint(string concName, Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Point end, Autodesk.DesignScript.Geometry.Vector orientation)
    {
      return new ConcStraightBeam(concName, start, end, orientation);
    }

    /// <summary>
    /// Create an Advance Steel Concrete Column
    /// </summary>
    /// <param name="concName"></param>
    /// <param name="start"></param>
    /// <param name="direction"></param>
    /// <param name="orientation"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static ConcStraightBeam ByStartPointDirectionLength(string concName, Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Vector direction, Autodesk.DesignScript.Geometry.Vector orientation, double length)
    {
      Vector3d columnDirection = Utils.ToAstVector3d(direction, true).Normalize();
      Point3d tempPoint = Utils.ToAstPoint(start, true);
      Point3d end = tempPoint.Add(columnDirection * length);
      return new ConcStraightBeam(concName, start, Utils.ToDynPoint(end, true), orientation);
    }

    /// <summary>
    /// Create an Advance Steel Concrete Beam/Column by Dynamo Line Geometry
    /// </summary>
    /// <param name="concName"></param>
    /// <param name="line"></param>
    /// <param name="orientation"></param>
    /// <returns></returns>
    public static ConcStraightBeam ByLine(string concName, Autodesk.DesignScript.Geometry.Line line, Autodesk.DesignScript.Geometry.Vector orientation)
    {
      Autodesk.DesignScript.Geometry.Point start = line.StartPoint;
      Autodesk.DesignScript.Geometry.Point end = line.EndPoint;
      return new ConcStraightBeam(concName, start, end, orientation);
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var beam = Utils.GetObject(Handle) as Beam;

          Point3d asPt1 = beam.GetPointAtStart(0);
          Point3d asPt2 = beam.GetPointAtEnd(0);

          using (var pt1 = Utils.ToDynPoint(beam.GetPointAtStart(0), true))
          using (var pt2 = Utils.ToDynPoint(beam.GetPointAtEnd(0), true))
          {
            var line = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(pt1, pt2);
            return line;
          }
        }
      }
    }
  }
}
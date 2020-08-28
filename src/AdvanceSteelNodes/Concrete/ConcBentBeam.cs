using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Profiles;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

using Autodesk.DesignScript.Runtime;

namespace AdvanceSteel.Nodes.Concrete
{
  /// <summary>
  /// Advance Steel bent beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class ConcBentBeam : GraphicObject
  {
    private Point3d PointOnArc;

    internal ConcBentBeam(string concName, Autodesk.DesignScript.Geometry.Point ptStart, Autodesk.DesignScript.Geometry.Point ptEnd, Autodesk.DesignScript.Geometry.Point ptOnArc, Autodesk.DesignScript.Geometry.Vector vOrientation)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Point3d beamStart = (ptStart == null ? new Point3d() : Utils.ToAstPoint(ptStart, true));
          Point3d beamEnd = (ptEnd == null ? new Point3d() : Utils.ToAstPoint(ptEnd, true));
          Vector3d refVect = Utils.ToAstVector3d(vOrientation, true);
          PointOnArc = Utils.ToAstPoint(ptOnArc, true);

          Autodesk.AdvanceSteel.Modelling.ConcreteBentBeam concBentBeam = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            concBentBeam = new Autodesk.AdvanceSteel.Modelling.ConcreteBentBeam(concName, refVect, beamStart, PointOnArc, beamEnd);
            concBentBeam.WriteToDb();
          }
          else
          {
            concBentBeam = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.ConcreteBentBeam;

            if (concBentBeam != null && concBentBeam.IsKindOf(FilerObject.eObjectType.kConcreteBentBeam))
            {
              concBentBeam.SetSystemline(beamStart, PointOnArc, beamEnd);
              concBentBeam.SetSysStart(beamStart);
              concBentBeam.SetSysEnd(beamEnd);
              concBentBeam.ProfName = concName;
              Utils.SetOrientation(concBentBeam, refVect);
            }
            else
              throw new System.Exception("Not a Bent Concrete Beam");
          }

          Handle = concBentBeam.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(concBentBeam);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel bent beam between two points and a point on arc
    /// </summary>
    /// <param name="concName"></param>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    /// <param name="ptOnArc">Point on arc</param>
    /// <param name="vOrientation">Section orientation</param>
    /// <returns></returns>
    public static ConcBentBeam ByStartPointEndPointOnArc(string concName, Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Point end, Autodesk.DesignScript.Geometry.Point ptOnArc, Autodesk.DesignScript.Geometry.Vector vOrientation)
    {
      return new ConcBentBeam(concName, start, end, ptOnArc, vOrientation);
    }

    /// <summary>
    /// Create an Advance Steel bent beam from arc
    /// </summary>
    /// <param name="concName"></param>
    /// <param name="arc"></param>
    /// <returns></returns>
    public static ConcBentBeam ByArc(string concName, Autodesk.DesignScript.Geometry.Arc arc)
    {
      Autodesk.DesignScript.Geometry.Point start = arc.StartPoint;
      Autodesk.DesignScript.Geometry.Point end = arc.EndPoint;
      Autodesk.DesignScript.Geometry.Point ptOnArc = arc.PointAtChordLength();

      return new ConcBentBeam(concName, start, end, ptOnArc, arc.Normal);
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var beam = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.ConcreteBentBeam;

          using (var start = Utils.ToDynPoint(beam.GetPointAtStart(0), true))
          using (var end = Utils.ToDynPoint(beam.GetPointAtEnd(0), true))
          using (var ptOnArc = Utils.ToDynPoint(PointOnArc, true))
          {
            var arc = Autodesk.DesignScript.Geometry.Arc.ByThreePoints(start, ptOnArc, end);
            return arc;
          }
        }
      }
    }

  }
}
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Profiles;

using Autodesk.DesignScript.Runtime;

namespace AdvanceSteel.Nodes.Beams
{
  /// <summary>
  /// An AdvanceSteel Bent Beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class BentBeam : GraphicObject
  {
    private Point3d _ptOnArc;

    internal BentBeam(Autodesk.DesignScript.Geometry.Point ptStart, Autodesk.DesignScript.Geometry.Point ptEnd, Autodesk.DesignScript.Geometry.Point ptOnArc, Autodesk.DesignScript.Geometry.Vector vOrientation)
    {
      //use lock just to be safe
      //AutoCAD does not support multithreaded access
      lock (myLock)
      {
        //lock the document and start transaction
        using (var _CADAccess = new AdvanceSteel.Services.ObjectAccess.CADContext())
        {
          string handle = AdvanceSteel.Services.ElementBinder.GetHandleFromTrace();
          var beamStart = (ptStart == null ? new Point3d() : Utils.ToAstPoint(ptStart, true));
          var beamEnd = (ptEnd == null ? new Point3d() : Utils.ToAstPoint(ptEnd, true));
          _ptOnArc = Utils.ToAstPoint(ptOnArc, true);

          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            ProfileName profName = new ProfileName();
            ProfilesManager.GetProfTypeAsDefault("I", out profName);

            var myBeam = new Autodesk.AdvanceSteel.Modelling.BentBeam(profName.Name, Vector3d.kZAxis, beamStart, _ptOnArc, beamEnd);

            myBeam.WriteToDb();
            handle = myBeam.Handle;
          }

          var beam = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.BentBeam;

          if (beam != null && beam.IsKindOf(FilerObject.eObjectType.kBentBeam))
          {
            beam.SetSystemline(beamStart, _ptOnArc, beamEnd);
            Utils.SetOrientation(beam, Utils.ToAstVector3d(vOrientation, true));
          }
          else
            throw new System.Exception("Not a BentBeam");

          Handle = handle;

          AdvanceSteel.Services.ElementBinder.CleanupAndSetElementForTrace(beam);
        }
      }
    }

    /// <summary>
    /// Create an advance steel bent beam between two points and a point on arc
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="ptOnArc"></param>
    /// <param name="section orientation"></param>
    /// <returns></returns>
    public static BentBeam ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Point end, Autodesk.DesignScript.Geometry.Point ptOnArc, Autodesk.DesignScript.Geometry.Vector vOrientation)
    {
      return new BentBeam(start, end, ptOnArc, vOrientation);
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve Curve
    {
      get
      {
        //use lock just to be safe
        //AutoCAD does not support multithreaded access
        lock (myLock)
        {
          using (var _CADAccess = new AdvanceSteel.Services.ObjectAccess.CADContext())
          {
            var beam = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.BentBeam;

            var start = Utils.ToDynPoint(beam.GetPointAtStart(0), true);
            var end = Utils.ToDynPoint(beam.GetPointAtEnd(0), true);

            var arc = Autodesk.DesignScript.Geometry.Arc.ByThreePoints(start, Utils.ToDynPoint(_ptOnArc, true), end);
            return arc;
          }
        }
      }
    }
  }
}
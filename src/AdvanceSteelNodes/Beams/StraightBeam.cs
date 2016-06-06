using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.AdvanceSteel.Profiles;
using Autodesk.DesignScript.Runtime;

namespace AdvanceSteel.Nodes.Beams
{
  /// <summary>DynamoServices
  /// An AdvanceSteel Straight Beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class StraightBeam : GraphicObject
  {
    internal StraightBeam(Autodesk.DesignScript.Geometry.Point ptStart, Autodesk.DesignScript.Geometry.Point ptEnd, Autodesk.DesignScript.Geometry.Vector vOrientation)
    {
      //use lock just to be safe
      //AutoCAD does not support multithreaded access
      lock (myLock)
      {
        //lock the document and start transaction
        using (var _CADAccess = new AdvanceSteel.Services.ObjectAccess.CADContext())
        {
          string handle = AdvanceSteel.Services.ElementBinder.GetHandleFromTrace();

          Point3d beamStart = Utils.ToAstPoint(ptStart, true);
          Point3d beamEnd = Utils.ToAstPoint(ptEnd, true);

          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            ProfileName profName = new ProfileName();
            ProfilesManager.GetProfTypeAsDefault("I", out profName);
            var myBeam = new Autodesk.AdvanceSteel.Modelling.StraightBeam(profName.Name, beamStart, beamEnd, Vector3d.kXAxis);

            myBeam.WriteToDb();
            handle = myBeam.Handle;
          }

          Beam beam = Utils.GetObject(handle) as Beam;

          if (beam != null && beam.IsKindOf(FilerObject.eObjectType.kStraightBeam))
          {
            Utils.AdjustBeamEnd(beam, beamStart);
            beam.SetSysStart(beamStart);
            beam.SetSysEnd(beamEnd);

            Utils.SetOrientation(beam, Utils.ToAstVector3d(vOrientation, true));
          }
          else
            throw new System.Exception("Not a StraightBeam");

          this.Handle = handle;

          AdvanceSteel.Services.ElementBinder.CleanupAndSetElementForTrace(beam);
        }
      }
    }

    /// <summary>
    /// Create an advance steel beam between two end points
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="Section orientation"></param>
    /// <returns></returns>
    public static StraightBeam ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Point end, Autodesk.DesignScript.Geometry.Vector vOrientation)
    {
      return new StraightBeam(start, end, vOrientation);
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
            var beam = Utils.GetObject(Handle) as Beam;

            Point3d asPt1 = beam.GetPointAtStart(0);
            Point3d asPt2 = beam.GetPointAtEnd(0);

            var pt1 = Utils.ToDynPoint(beam.GetPointAtStart(0), true);
            var pt2 = Utils.ToDynPoint(beam.GetPointAtEnd(0), true);

            var line = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(pt1, pt2);
            return line;
          }
        }
      }
    }
  }
}
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.AdvanceSteel.Profiles;
using Autodesk.DesignScript.Runtime;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Beams
{
  /// <summary>
  /// Advance Steel straight beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class StraightBeam : GraphicObject
  {
    internal StraightBeam(Autodesk.DesignScript.Geometry.Point ptStart, Autodesk.DesignScript.Geometry.Point ptEnd, Autodesk.DesignScript.Geometry.Vector vOrientation)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Point3d beamStart = Utils.ToAstPoint(ptStart, true);
          Point3d beamEnd = Utils.ToAstPoint(ptEnd, true);
          Vector3d refVect = Utils.ToAstVector3d(vOrientation, true);

          Autodesk.AdvanceSteel.Modelling.StraightBeam beam = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            ProfileName profName = new ProfileName();
            ProfilesManager.GetProfTypeAsDefault("I", out profName);
            beam = new Autodesk.AdvanceSteel.Modelling.StraightBeam(profName.Name, beamStart, beamEnd, refVect);
            beam.WriteToDb();

          }
          else
          {
            beam = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.StraightBeam;

            if (beam != null && beam.IsKindOf(FilerObject.eObjectType.kStraightBeam))
            {
              Utils.AdjustBeamEnd(beam, beamStart);
              beam.SetSysStart(beamStart);
              beam.SetSysEnd(beamEnd);

              Utils.SetOrientation(beam, refVect);
            }
            else
              throw new System.Exception("Not a straight Beam");
          }
          Handle = beam.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(beam);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel straight beam between two points
    /// </summary>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    /// <param name="vOrientation">Section orientation</param>
    /// <returns></returns>
    public static StraightBeam ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Point end, Autodesk.DesignScript.Geometry.Vector vOrientation)
    {
      return new StraightBeam(start, end, vOrientation);
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
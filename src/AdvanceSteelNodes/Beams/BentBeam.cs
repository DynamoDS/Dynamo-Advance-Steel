using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Profiles;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

using Autodesk.DesignScript.Runtime;

namespace AdvanceSteel.Nodes.Beams
{
  /// <summary>
  /// Advance Steel bent beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class BentBeam : GraphicObject
  {
    private Point3d _ptOnArc;

    internal BentBeam(Autodesk.DesignScript.Geometry.Point ptStart, Autodesk.DesignScript.Geometry.Point ptEnd, Autodesk.DesignScript.Geometry.Point ptOnArc, Autodesk.DesignScript.Geometry.Vector vOrientation)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();
          Point3d beamStart = (ptStart == null ? new Point3d() : Utils.ToAstPoint(ptStart, true));
          Point3d beamEnd = (ptEnd == null ? new Point3d() : Utils.ToAstPoint(ptEnd, true));
          Vector3d refVect = Utils.ToAstVector3d(vOrientation, true);
          _ptOnArc = Utils.ToAstPoint(ptOnArc, true);

          Autodesk.AdvanceSteel.Modelling.BentBeam beam = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            ProfileName profName = new ProfileName();
            ProfilesManager.GetProfTypeAsDefault("I", out profName);

            beam = new Autodesk.AdvanceSteel.Modelling.BentBeam(profName.Name, refVect, beamStart, _ptOnArc, beamEnd);
            beam.WriteToDb();
          }
          else
          {
            beam = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.BentBeam;

            if (beam != null && beam.IsKindOf(FilerObject.eObjectType.kBentBeam))
            {
              beam.SetSystemline(beamStart, _ptOnArc, beamEnd);
              Utils.SetOrientation(beam, refVect);
            }
            else
              throw new System.Exception("Not a bent Beam");
          }

          Handle = beam.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(beam);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel bent beam between two points and a point on arc
    /// </summary>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    /// <param name="ptOnArc">Point on arc</param>
    /// <param name="vOrientation">Section orientation</param>
    /// <returns></returns>
    public static BentBeam ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Point end, Autodesk.DesignScript.Geometry.Point ptOnArc, Autodesk.DesignScript.Geometry.Vector vOrientation)
    {
      return new BentBeam(start, end, ptOnArc, vOrientation);
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var beam = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.BentBeam;

          using (var start = Utils.ToDynPoint(beam.GetPointAtStart(0), true))
          using (var end = Utils.ToDynPoint(beam.GetPointAtEnd(0), true))
          using (var ptOnArc = Utils.ToDynPoint(_ptOnArc, true))
          {
            var arc = Autodesk.DesignScript.Geometry.Arc.ByThreePoints(start, ptOnArc, end);
            return arc;
          }
        }
      }
    }

  }
}
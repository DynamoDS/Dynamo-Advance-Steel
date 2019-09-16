using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.DesignScript.Runtime;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Beams
{
  /// <summary>
  /// Advance Steel tapered beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class TaperedBeam : GraphicObject
  {
    internal TaperedBeam(Autodesk.DesignScript.Geometry.Point ptStart, Autodesk.DesignScript.Geometry.Point ptEnd, Autodesk.DesignScript.Geometry.Vector vOrientation, double startHeight = 100, double endHeight = 100, double webThickness = 100)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Point3d beamStart = Utils.ToAstPoint(ptStart, true);
          Point3d beamEnd = Utils.ToAstPoint(ptEnd, true);
          Vector3d refVect = Utils.ToAstVector3d(vOrientation, true);

          Autodesk.AdvanceSteel.Modelling.BeamTapered beam = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            beam = new Autodesk.AdvanceSteel.Modelling.BeamTapered(beamStart, beamEnd, refVect, startHeight, endHeight, webThickness);
            beam.CreateComponents();
            beam.WriteToDb();
          }
          else
          {
            beam = Utils.GetObject(handle) as BeamTapered;

            if (beam != null && beam.IsKindOf(FilerObject.eObjectType.kBeamTapered))
            {
              Utils.AdjustBeamEnd(beam, beamStart);
              beam.SetSysStart(beamStart);
              beam.SetSysEnd(beamEnd);

              Utils.SetOrientation(beam, refVect);
            }
            else
              throw new System.Exception("Not a tapered beam");
          }

          Handle = beam.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(beam);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel tapered beam
    /// </summary>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    /// <param name="vOrientation">Section orientation</param>
    /// <returns></returns>
    public static TaperedBeam ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Point end, Autodesk.DesignScript.Geometry.Vector vOrientation)
    {
      return new TaperedBeam(start, end, vOrientation);
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
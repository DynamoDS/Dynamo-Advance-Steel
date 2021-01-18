using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Profiles;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace AdvanceSteel.Nodes.Beams
{
  /// <summary>
  /// Advance Steel bent beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class BentBeam : GraphicObject
  {
    internal BentBeam()
    {
    }

    internal BentBeam(Autodesk.DesignScript.Geometry.Point ptStart,
                      Autodesk.DesignScript.Geometry.Point ptEnd,
                      Autodesk.DesignScript.Geometry.Point ptOnArc,
                      Autodesk.DesignScript.Geometry.Vector vOrientation,
                      int refAxis, bool crossSectionMirror,
                      List<ASProperty> beamProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          List<ASProperty> defaultData = beamProperties.Where(x => x.Level == ".").ToList<ASProperty>();
          List<ASProperty> postWriteDBData = beamProperties.Where(x => x.Level == "Z_PostWriteDB").ToList<ASProperty>();
          ASProperty foundProfName = beamProperties.FirstOrDefault<ASProperty>(x => x.Name == "ProfName");
          string sectionName = "";
          if (foundProfName != null)
          {
            sectionName = (string)foundProfName.Value;
          }

          string handle = SteelServices.ElementBinder.GetHandleFromTrace();
          Point3d beamStart = (ptStart == null ? new Point3d() : Utils.ToAstPoint(ptStart, true));
          Point3d beamEnd = (ptEnd == null ? new Point3d() : Utils.ToAstPoint(ptEnd, true));
          Vector3d refVect = Utils.ToAstVector3d(vOrientation, true);
          Point3d pointOnArc = Utils.ToAstPoint(ptOnArc, true);

          if (string.IsNullOrEmpty(sectionName))
          {
            ProfileName profName = new ProfileName();
            ProfilesManager.GetProfTypeAsDefault("I", out profName);
            sectionName = profName.Name;
          }

          Autodesk.AdvanceSteel.Modelling.BentBeam beam = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            beam = new Autodesk.AdvanceSteel.Modelling.BentBeam(sectionName, refVect, beamStart, pointOnArc, beamEnd);
            if (Beam.eRefAxis.IsDefined(typeof(Beam.eRefAxis), refAxis) == true)
            {
              beam.RefAxis = (Beam.eRefAxis)refAxis;
            }
            beam.SetCrossSectionMirrored(crossSectionMirror, false);

            if (defaultData != null)
            {
              Utils.SetParameters(beam, defaultData);
            }

            beam.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(beam, postWriteDBData);
            }
          }
          else
          {
            beam = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.BentBeam;

            if (beam != null && beam.IsKindOf(FilerObject.eObjectType.kBentBeam))
            {
              string sectionType = Utils.SplitSectionName(sectionName)[0];
              string sectionSize = Utils.SplitSectionName(sectionName)[1];
              beam.SetSystemline(beamStart, pointOnArc, beamEnd);
              beam.ChangeProfile(sectionType, sectionSize);
              if (Beam.eRefAxis.IsDefined(typeof(Beam.eRefAxis), refAxis) == true)
              {
                beam.RefAxis = (Beam.eRefAxis)refAxis;
              }

              if (defaultData != null)
              {
                Utils.SetParameters(beam, defaultData);
              }

              beam.SetCrossSectionMirrored(crossSectionMirror, false);
              Utils.SetOrientation(beam, refVect);

              if (postWriteDBData != null)
              {
                Utils.SetParameters(beam, postWriteDBData);
              }

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
    /// <param name="orientation">Section orientation</param>
    /// <param name="additionalBeamParameters"> Optional Input Beam Build Properties </param>
    /// <returns></returns>
    public static BentBeam ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point start,
                                                Autodesk.DesignScript.Geometry.Point end,
                                                Autodesk.DesignScript.Geometry.Point ptOnArc,
                                                Autodesk.DesignScript.Geometry.Vector orientation,
                                                [DefaultArgument("null")] List<ASProperty> additionalBeamParameters)
    {
      additionalBeamParameters = PreSetDefaults(additionalBeamParameters);
      return new BentBeam(start, end, ptOnArc, orientation, -1, false, additionalBeamParameters);
    }

    /// <summary>
    /// Create an Advance Steel bent beam between two points and a point on arc - inc ModelRole and Section Size
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="ptOnArc"></param>
    /// <param name="orientation">Section orientation</param>
    /// <param name="refAxis">Input Beam reference axis UpperLeft = 0, UpperSys = 1, UpperRight = 2, MidLeft = 3, SysSys = 4, MidRight = 5, LowerLeft = 6, LowerSys = 7, LowerRight = 8, ContourCenter = 9</param>
    /// <param name="crossSectionMirror">Input Beam Mirror Option</param>
    /// <param name="additionalBeamParameters"> Optional Input Beam Build Properties </param>
    /// <returns></returns>
    public static BentBeam ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point start,
                                                Autodesk.DesignScript.Geometry.Point end,
                                                Autodesk.DesignScript.Geometry.Point ptOnArc,
                                                Autodesk.DesignScript.Geometry.Vector orientation,
                                                [DefaultArgument("5;")] int refAxis,
                                                [DefaultArgument("false;")] bool crossSectionMirror,
                                                [DefaultArgument("null")] List<ASProperty> additionalBeamParameters)
    {
      var arc = Autodesk.DesignScript.Geometry.Arc.ByThreePoints(start, ptOnArc, end);
      Autodesk.DesignScript.Geometry.Point[] cvs = arc.PointsAtEqualSegmentLength(2);
      additionalBeamParameters = PreSetDefaults(additionalBeamParameters);
      return new BentBeam(arc.StartPoint, arc.EndPoint, cvs[0], orientation, refAxis, crossSectionMirror, additionalBeamParameters);

      //return new BentBeam(start, end, ptOnArc, orientation, modelRole, sectionName, Utils.ToInternalAngleUnits(rotation, true), refAxis, crossSectionMirror);
    }

    /// <summary>
    /// Create an Advance Steel bent beam on a Dynamo Arc- inc ModelRole and Section Size
    /// </summary>
    /// <param name="arc">Input Dynamo Node</param>
    /// <param name="orientation">Section orientation</param>
    /// <param name="refAxis">Input Beam reference axis UpperLeft = 0, UpperSys = 1, UpperRight = 2, MidLeft = 3, SysSys = 4, MidRight = 5, LowerLeft = 6, LowerSys = 7, LowerRight = 8, ContourCenter = 9</param>
    /// <param name="crossSectionMirror">Input Beam Mirror Option</param>
    /// <param name="additionalBeamParameters"> Optional Input Beam Build Properties </param>
    /// <returns></returns>
    public static BentBeam ByArc(Autodesk.DesignScript.Geometry.Arc arc,
                                [DefaultArgument("Autodesk.DesignScript.Geometry.Vector.ZAxis();")] Autodesk.DesignScript.Geometry.Vector orientation,
                                [DefaultArgument("5;")] int refAxis,
                                [DefaultArgument("false;")] bool crossSectionMirror,
                                [DefaultArgument("null")] List<ASProperty> additionalBeamParameters)
    {
      Autodesk.DesignScript.Geometry.Point[] cvs = arc.PointsAtEqualSegmentLength(2);
      additionalBeamParameters = PreSetDefaults(additionalBeamParameters);
      return new BentBeam(arc.StartPoint, arc.EndPoint, cvs[0], orientation, refAxis, crossSectionMirror, additionalBeamParameters);
    }

    private static List<ASProperty> PreSetDefaults(List<ASProperty> listBeamData)
    {
      if (listBeamData == null)
      {
        listBeamData = new List<ASProperty>() { };
      }
      return listBeamData;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var beam = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.BentBeam;
          var midPointOnArc = beam.CenterPoint;

          using (var start = Utils.ToDynPoint(beam.GetPointAtStart(0), true))
          using (var end = Utils.ToDynPoint(beam.GetPointAtEnd(0), true))
          using (var ptOnArc = Utils.ToDynPoint(midPointOnArc, true))
          {
            var arc = Autodesk.DesignScript.Geometry.Arc.ByThreePoints(start, ptOnArc, end);
            return arc;
          }
        }
      }
    }

  }
}
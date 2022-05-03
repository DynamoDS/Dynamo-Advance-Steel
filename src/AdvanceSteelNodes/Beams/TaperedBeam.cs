using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using ASBeamTapered = Autodesk.AdvanceSteel.Modelling.BeamTapered;

namespace AdvanceSteel.Nodes.Beams
{
  /// <summary>
  /// Advance Steel tapered beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class TaperedBeam : GraphicObject
  {
    private TaperedBeam(Autodesk.DesignScript.Geometry.Point ptStart,
                          Autodesk.DesignScript.Geometry.Point ptEnd,
                          Autodesk.DesignScript.Geometry.Vector vOrientation,
                          double startHeight,
                          double endHeight,
                          double webThickness,
                          List<Property> beamProperties)
    {
      SafeInit(() => InitTaperedBeam(ptStart, ptEnd, vOrientation, startHeight, endHeight, webThickness, beamProperties));
    }

    private TaperedBeam(ASBeamTapered beam)
    {
      SafeInit(() => SetHandle(beam));
    }

    internal static TaperedBeam FromExisting(ASBeamTapered beam)
    {
      return new TaperedBeam(beam)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitTaperedBeam(Autodesk.DesignScript.Geometry.Point ptStart,
                          Autodesk.DesignScript.Geometry.Point ptEnd,
                          Autodesk.DesignScript.Geometry.Vector vOrientation,
                          double startHeight,
                          double endHeight,
                          double webThickness,
                          List<Property> beamProperties)
    {
      List<Property> defaultData = beamProperties.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> postWriteDBData = beamProperties.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();

      Point3d beamStart = Utils.ToAstPoint(ptStart, true);
      Point3d beamEnd = Utils.ToAstPoint(ptEnd, true);
      Vector3d refVect = Utils.ToAstVector3d(vOrientation, true);

      ASBeamTapered beam = SteelServices.ElementBinder.GetObjectASFromTrace<ASBeamTapered>();
      if (beam == null)
      {
        beam = new ASBeamTapered(beamStart, beamEnd, refVect, startHeight, endHeight, webThickness);
        beam.CreateComponents();

        if (defaultData != null)
        {
          Utils.SetParameters(beam, defaultData);
        }

        beam.WriteToDb();
      }
      else
      {
        if (!beam.IsKindOf(FilerObject.eObjectType.kBeamTapered))
          throw new System.Exception("Not a tapered beam");

        Utils.AdjustBeamEnd(beam, beamStart);
        beam.SetSysStart(beamStart);
        beam.SetSysEnd(beamEnd);

        if (defaultData != null)
        {
          Utils.SetParameters(beam, defaultData);
        }

        Utils.SetOrientation(beam, refVect);
      }

      SetHandle(beam);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(beam, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(beam);
    }

    /// <summary>
    /// Create an Advance Steel tapered beam
    /// </summary>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    /// <param name="vOrientation">Section orientation</param>
    /// <param name="additionalBeamParameters"> Optional Input Beam Build Properties </param>
    /// <returns name="taperedBeam"> beam</returns>
    public static TaperedBeam ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point start,
                                                    Autodesk.DesignScript.Geometry.Point end,
                                                    Autodesk.DesignScript.Geometry.Vector vOrientation,
                                                    [DefaultArgument("null")] List<Property> additionalBeamParameters)
    {
      additionalBeamParameters = PreSetDefaults(additionalBeamParameters);
      return new TaperedBeam(start, end, vOrientation, 100, 100, 100, additionalBeamParameters);
    }

    /// <summary>
    /// Create an Advance Steel tapered beam with start and end heights and Web Thickness
    /// </summary>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    /// <param name="vOrientation">Section orientation</param>
    /// <param name="startHeight"> Input starting Height</param>
    /// <param name="endHeight"> Input end Height</param>
    /// <param name="webThickness"> Input Web Thickness</param>
    /// <param name="additionalBeamParameters"> Optional Input Beam Build Properties </param>
    /// <returns name="taperedBeam"> beam</returns>
    public static TaperedBeam ByStartPointEndPointHeights(Autodesk.DesignScript.Geometry.Point start,
                                                          Autodesk.DesignScript.Geometry.Point end,
                                                          Autodesk.DesignScript.Geometry.Vector vOrientation,
                                                          [DefaultArgument("100")] double startHeight,
                                                          [DefaultArgument("100")] double endHeight,
                                                          [DefaultArgument("100")] double webThickness,
                                                          [DefaultArgument("null")] List<Property> additionalBeamParameters)
    {
      additionalBeamParameters = PreSetDefaults(additionalBeamParameters);
      return new TaperedBeam(start, end, vOrientation,
                              Utils.ToInternalDistanceUnits(startHeight, true),
                              Utils.ToInternalDistanceUnits(endHeight, true),
                              Utils.ToInternalDistanceUnits(webThickness, true), additionalBeamParameters);
    }

    private static List<Property> PreSetDefaults(List<Property> listBeamData)
    {
      if (listBeamData == null)
      {
        listBeamData = new List<Property>() { };
      }
      return listBeamData;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
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
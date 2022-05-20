using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using ASCompoundStraightBeam = Autodesk.AdvanceSteel.Modelling.CompoundStraightBeam;

namespace AdvanceSteel.Nodes.Beams
{
  /// <summary>
  /// Advance Steel compound beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class CompoundBeam : GraphicObject
  {
    private CompoundBeam(Autodesk.DesignScript.Geometry.Point ptStart,
                          Autodesk.DesignScript.Geometry.Point ptEnd,
                          Autodesk.DesignScript.Geometry.Vector vOrientation,
                          List<Property> beamProperties)
    {
      SafeInit(() => InitCompoundBeam(ptStart, ptEnd, vOrientation, beamProperties));
    }

    private CompoundBeam(ASCompoundStraightBeam beam)
    {
      SafeInit(() => SetHandle(beam));
    }

    internal static CompoundBeam FromExisting(ASCompoundStraightBeam beam)
    {
      return new CompoundBeam(beam)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitCompoundBeam(Autodesk.DesignScript.Geometry.Point ptStart,
                          Autodesk.DesignScript.Geometry.Point ptEnd,
                          Autodesk.DesignScript.Geometry.Vector vOrientation,
                          List<Property> beamProperties)
    {
      List<Property> defaultData = beamProperties.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> postWriteDBData = beamProperties.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();
      Property foundProfName = beamProperties.FirstOrDefault<Property>(x => x.Name == "ProfName");
      string sectionProfileName = "";
      if (foundProfName != null)
      {
        sectionProfileName = (string)foundProfName.InternalValue;
      }

      Point3d beamStart = Utils.ToAstPoint(ptStart, true);
      Point3d beamEnd = Utils.ToAstPoint(ptEnd, true);
      Vector3d refVect = Utils.ToAstVector3d(vOrientation, true);

      string sectionType = Utils.SplitSectionName(sectionProfileName)[0];
      string sectionName = Utils.SplitSectionName(sectionProfileName)[1];

      ASCompoundStraightBeam beam = SteelServices.ElementBinder.GetObjectASFromTrace<ASCompoundStraightBeam>();
      if (beam == null)
      {
        beam = new ASCompoundStraightBeam(beamStart, beamEnd, refVect);
        beam.CreateComponents(sectionType, sectionName);

        if (defaultData != null)
        {
          Utils.SetParameters(beam, defaultData);
        }

        beam.WriteToDb();
      }
      else
      {
        if (!beam.IsKindOf(FilerObject.eObjectType.kCompoundStraightBeam))
          throw new System.Exception("Not a compound beam");

        Utils.AdjustBeamEnd(beam, beamStart);
        beam.SetSysStart(beamStart);
        beam.SetSysEnd(beamEnd);

        if (defaultData != null)
        {
          Utils.SetParameters(beam, defaultData);
        }

        Utils.SetOrientation(beam, refVect);

        if (Utils.CompareCompoundSectionTypes(sectionType, beam.ProfSectionType))
        {
          if (beam.ProfSectionName != sectionName)
          {
            beam.ChangeProfile(sectionType, sectionName);
          }
        }
        else
        {
          throw new System.Exception("Failed to change section as compound section type is different than the one created the beam was created with");
        }
      }

      SetHandle(beam);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(beam, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(beam);
    }


    /// <summary>
    /// Create an Advance Steel compound beam
    /// </summary>
    /// <param name="start"> Input Start point</param>
    /// <param name="end"> Input End point</param>
    /// <param name="orientation"> Input Section orientation</param>
    /// <param name="sectionName"> Input Section name</param>
    /// <param name="additionalBeamParameters"> Optional Input Beam Build Properties </param>
    /// <returns name="compoundBeam"> beam</returns>
    public static CompoundBeam ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point start,
                                                    Autodesk.DesignScript.Geometry.Point end,
                                                    Autodesk.DesignScript.Geometry.Vector orientation,
                                                    string sectionName,
                                                    [DefaultArgument("null")] List<Property> additionalBeamParameters)
    {
      additionalBeamParameters = PreSetDefaults(additionalBeamParameters, sectionName);
      return new CompoundBeam(start, end, orientation, additionalBeamParameters);
    }

    private static List<Property> PreSetDefaults(List<Property> listBeamData, string sectionName)
    {
      if (listBeamData == null)
      {
        listBeamData = new List<Property>() { };
      }
      Utils.CheckListUpdateOrAddValue(listBeamData, "ProfName", sectionName);

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
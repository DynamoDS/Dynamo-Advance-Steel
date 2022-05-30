using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.AdvanceSteel.Profiles;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using System.Linq;
using ASPolyBeam = Autodesk.AdvanceSteel.Modelling.PolyBeam;

namespace AdvanceSteel.Nodes.Beams
{
  /// <summary>
  /// Advance Steel Polybeam Beams
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class PolyBeam : GraphicObject
  {
    private PolyBeam(Polyline3d poly,
                         Autodesk.DesignScript.Geometry.Vector vOrientation,
                         List<Property> beamProperties)
    {
      SafeInit(() => InitPolyBeam(poly, vOrientation, beamProperties));
    }

    private PolyBeam(ASPolyBeam beam)
    {
      SafeInit(() => SetHandle(beam));
    }

    internal static PolyBeam FromExisting(ASPolyBeam beam)
    {
      return new PolyBeam(beam)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitPolyBeam(Polyline3d poly,
                      Autodesk.DesignScript.Geometry.Vector vOrientation,
                      List<Property> beamProperties)
    {
      List<Property> defaultData = beamProperties.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> postWriteDBData = beamProperties.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();
      Property foundProfName = beamProperties.FirstOrDefault<Property>(x => x.MemberName == nameof(Beam.ProfName));
      string sectionName = "";
      if (foundProfName != null)
      {
        sectionName = (string)foundProfName.InternalValue;
      }

      if (string.IsNullOrEmpty(sectionName))
      {
        ProfileName profName = new ProfileName();
        ProfilesManager.GetProfTypeAsDefault("I", out profName);
        sectionName = profName.Name;
      }

      Vector3d refVect = Utils.ToAstVector3d(vOrientation, true);

      ASPolyBeam beam = SteelServices.ElementBinder.GetObjectASFromTrace<ASPolyBeam>();
      if (beam == null)
      {
        beam = new ASPolyBeam(sectionName, poly, refVect);

        if (defaultData != null)
        {
          UtilsProperties.SetParameters(beam, defaultData);
        }

        beam.WriteToDb();
      }
      else
      {
        if (!beam.IsKindOf(FilerObject.eObjectType.kPolyBeam))
          throw new System.Exception("Not an UnFolded Straight Beam");

        beam.SetPolyline(poly);

        if (defaultData != null)
        {
          UtilsProperties.SetParameters(beam, defaultData);
        }

        Utils.SetOrientation(beam, refVect);
      }

      SetHandle(beam);

      if (postWriteDBData != null)
      {
        UtilsProperties.SetParameters(beam, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(beam);
    }

    /// <summary>
    /// Create an Advance Steel poly beam between from a dynamo polycurve
    /// </summary>
    /// <param name="polyCurve"> Input Dynamo Polycurve</param>
    /// <param name="orientation">Section orientation</param>
    /// <param name="additionalBeamParameters"> Optional Input Beam Build Properties </param>
    /// <returns name="polyBeam"> beam</returns>
    public static PolyBeam ByPolyCurve(Autodesk.DesignScript.Geometry.PolyCurve polyCurve,
                                        Autodesk.DesignScript.Geometry.Vector orientation,
                                        [DefaultArgument("null")] List<Property> additionalBeamParameters)
    {
      additionalBeamParameters = PreSetDefaults(additionalBeamParameters);
      Polyline3d poly = Utils.ToAstPolyline3d(polyCurve, true);
      if (poly == null)
        throw new System.Exception("No Valid Poly");
      return new PolyBeam(poly, orientation, additionalBeamParameters);
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
      var beam = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.PolyBeam;

      Polyline3d poly = beam.GetPolyline(true);
      Autodesk.DesignScript.Geometry.PolyCurve pCurve = Autodesk.DesignScript.Geometry.PolyCurve.ByJoinedCurves(Utils.ToDynPolyCurves(poly, true));
      return pCurve;
    }

  }
}
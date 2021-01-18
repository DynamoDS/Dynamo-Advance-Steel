using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.AdvanceSteel.Profiles;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using System.Linq;

namespace AdvanceSteel.Nodes.Beams
{
  /// <summary>
  /// Advance Steel Polybeam Beams
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class PolyBeam : GraphicObject
  {

    internal PolyBeam()
    {
    }

    internal PolyBeam(Polyline3d poly,
                      Autodesk.DesignScript.Geometry.Vector vOrientation,
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

          if (string.IsNullOrEmpty(sectionName))
          {
            ProfileName profName = new ProfileName();
            ProfilesManager.GetProfTypeAsDefault("I", out profName);
            sectionName = profName.Name;
          }

          Vector3d refVect = Utils.ToAstVector3d(vOrientation, true);

          Autodesk.AdvanceSteel.Modelling.PolyBeam beam = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            beam = new Autodesk.AdvanceSteel.Modelling.PolyBeam(sectionName, poly, refVect);

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
            beam = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.PolyBeam;

            if (beam != null && beam.IsKindOf(FilerObject.eObjectType.kPolyBeam))
            {
              beam.SetPolyline(poly);

              if (defaultData != null)
              {
                Utils.SetParameters(beam, defaultData);
              }

              Utils.SetOrientation(beam, refVect);

              if (postWriteDBData != null)
              {
                Utils.SetParameters(beam, postWriteDBData);
              }
            }
            else
              throw new System.Exception("Not an UnFolded Straight Beam");
          }
          Handle = beam.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(beam);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel poly beam between from a dynamo polycurve
    /// </summary>
    /// <param name="polyCurve"> Input Dynamo Polycurve</param>
    /// <param name="orientation">Section orientation</param>
    /// <param name="additionalBeamParameters"> Optional Input Beam Build Properties </param>
    /// <returns></returns>
    public static PolyBeam ByPolyCurve(Autodesk.DesignScript.Geometry.PolyCurve polyCurve,
                                        Autodesk.DesignScript.Geometry.Vector orientation,
                                        [DefaultArgument("null")] List<ASProperty> additionalBeamParameters)
    {
      additionalBeamParameters = PreSetDefaults(additionalBeamParameters);
      Polyline3d poly = Utils.ToAstPolyline3d(polyCurve, true);
      if (poly == null)
        throw new System.Exception("No Valid Poly");
      return new PolyBeam(poly, orientation, additionalBeamParameters);
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
          var beam = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.PolyBeam;

          Polyline3d poly = beam.GetPolyline();
          Autodesk.DesignScript.Geometry.PolyCurve pCurve = Autodesk.DesignScript.Geometry.PolyCurve.ByJoinedCurves(Utils.ToDynPolyCurves(poly, true));
          return pCurve;
        }
      }
    }
  }
}
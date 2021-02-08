using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Modelling;
using System;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Util
{
  /// <summary>
  /// This node can be used to assign Section to Advance Steel beams from Dynamo
  /// </summary>
  public class BeamSection
  {
    internal BeamSection()
    {
    }

    /// <summary>
    /// This node can set the Section for Advance Steel beams from Dynamo.
    /// For the Section use the following format: "HEA  DIN18800-1#@§@#HEA100"
    /// </summary>
    /// <param name="beamElement">Advance Steel beam</param>
    /// <param name="sectionName">Section</param>
    /// <returns></returns>
    [Obsolete]
    public static void SetSection(AdvanceSteel.Nodes.SteelDbObject beamElement, string sectionName)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        string handle = beamElement.Handle;

        FilerObject obj = Utils.GetObject(handle);

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kBeam))
        {

          string sectionType = Utils.SplitSectionName(sectionName)[0];
          string sectionSize = Utils.SplitSectionName(sectionName)[1];

          Beam beam = obj as Beam;
          if (obj.IsKindOf(FilerObject.eObjectType.kCompoundBeam) && !Utils.CompareCompoundSectionTypes(beam.ProfSectionType, sectionType))
          {
            throw new System.Exception("Failed to change section as compound section type is different");
          }
          beam.ChangeProfile(sectionType, sectionSize);
        }
        else
          throw new System.Exception("Failed to change section");
      }
    }

    /// <summary>
    /// Returns a concatenated string containing the SectionType, a fixed string separator "#@§@#" and the SectionSize.
    /// </summary>
    /// <param name="sectionType">SectionType for a beam section</param>
    /// <param name="sectionSize">SectionSize for a beam section</param>
    /// <returns name="sectionName">Beam section name</returns>
    [Obsolete]
    public static string CreateSectionString(string sectionType, string sectionSize)
    {
      return sectionType + Utils.Separator + sectionSize;
    }
  }
}
using Autodesk.AdvanceSteel.Arrangement;
using Autodesk.AdvanceSteel.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;
using static Autodesk.AdvanceSteel.DotNetRoots.Units.Unit;

namespace AdvanceSteel.Nodes
{
  public class ScrewBoltPatternProperties : BaseProperties<ScrewBoltPattern>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Top Tool Diameter", nameof(ScrewBoltPattern.TopToolDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Bottom Tool Diameter", nameof(ScrewBoltPattern.BottomToolDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Bottom Tool Height", nameof(ScrewBoltPattern.BottomToolHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Head Number of Edges", nameof(ScrewBoltPattern.BoltHeadNumEdges), LevelEnum.Default);
      InsertProperty(dictionary, "Head Diameter", nameof(ScrewBoltPattern.BoltHeadDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Head Height", nameof(ScrewBoltPattern.BoltHeadHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Top Tool Height", nameof(ScrewBoltPattern.TopToolHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Bolt Assembly", nameof(ScrewBoltPattern.BoltAssembly));
      InsertProperty(dictionary, "Grade", nameof(ScrewBoltPattern.Grade));
      InsertProperty(dictionary, "Standard", nameof(ScrewBoltPattern.Standard));
      InsertProperty(dictionary, "Hole Tolerance", nameof(ScrewBoltPattern.HoleTolerance), eUnitType.kDistance);
      InsertProperty(dictionary, "Binding Length Addition", nameof(ScrewBoltPattern.BindingLengthAddition), eUnitType.kDistance);
      InsertProperty(dictionary, "Annotation", nameof(ScrewBoltPattern.Annotation), LevelEnum.Default);
      InsertProperty(dictionary, "Screw Length", nameof(ScrewBoltPattern.ScrewLength), eUnitType.kDistance);
      InsertProperty(dictionary, "Sum Top Height", nameof(ScrewBoltPattern.SumTopHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Sum Top Set Height", nameof(ScrewBoltPattern.SumTopSetHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Sum Bottom Set Height", nameof(ScrewBoltPattern.SumBottomSetHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Sum Bottom Height", nameof(ScrewBoltPattern.SumBottomHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Max Top Diameter", nameof(ScrewBoltPattern.MaxTopDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Max Bottom Diameter", nameof(ScrewBoltPattern.MaxBottomDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Nut Height", nameof(ScrewBoltPattern.NutHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Nut Diameter", nameof(ScrewBoltPattern.NutDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Screw Diameter", nameof(ScrewBoltPattern.ScrewDiameter), eUnitType.kDistance);
      InsertProperty(dictionary, "Binding Length", nameof(ScrewBoltPattern.BindingLength), eUnitType.kDistance);
      InsertProperty(dictionary, "Ignore Max Gap", nameof(ScrewBoltPattern.IgnoreMaxGap));

      InsertProperty(dictionary, "Weight", nameof(ScrewBoltPattern.GetWeight), eUnitType.kWeight);

      InsertCustomProperty(dictionary, "Screw Bolt Type", nameof(ScrewBoltPatternProperties.GetScrewBoltType), null);
      InsertCustomProperty(dictionary, "Assembly Location", nameof(ScrewBoltPatternProperties.GetAssemblyLocation), null);

      return dictionary;
    }

    private string GetScrewBoltType(ScrewBoltPattern screwBoltPattern)
    {
      return screwBoltPattern.ScrewBoltType.ToString();
    }

    private string GetAssemblyLocation(ScrewBoltPattern screwBoltPattern)
    {
      return screwBoltPattern.AssemblyLocation.ToString();
    }
  }
}
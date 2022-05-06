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
  public class ScrewBoltPatternProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kScrewBoltPattern;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Top Tool Diameter", nameof(ScrewBoltPattern.TopToolDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Bottom Tool Diameter", nameof(ScrewBoltPattern.BottomToolDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Bottom Tool Height", nameof(ScrewBoltPattern.BottomToolHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Head Number of Edges", nameof(ScrewBoltPattern.BoltHeadNumEdges), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Head Diameter", nameof(ScrewBoltPattern.BoltHeadDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Head Height", nameof(ScrewBoltPattern.BoltHeadHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Top Tool Height", nameof(ScrewBoltPattern.TopToolHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Assembly", nameof(ScrewBoltPattern.BoltAssembly));
      InsertItem(dictionary, objectASType, "Grade", nameof(ScrewBoltPattern.Grade));
      InsertItem(dictionary, objectASType, "Standard", nameof(ScrewBoltPattern.Standard));
      InsertItem(dictionary, objectASType, "Hole Tolerance", nameof(ScrewBoltPattern.HoleTolerance), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Binding Length Addition", nameof(ScrewBoltPattern.BindingLengthAddition), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Annotation", nameof(ScrewBoltPattern.Annotation), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Screw Length", nameof(ScrewBoltPattern.ScrewLength), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Sum Top Height", nameof(ScrewBoltPattern.SumTopHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Sum Top Set Height", nameof(ScrewBoltPattern.SumTopSetHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Sum Bottom Set Height", nameof(ScrewBoltPattern.SumBottomSetHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Sum Bottom Height", nameof(ScrewBoltPattern.SumBottomHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Max Top Diameter", nameof(ScrewBoltPattern.MaxTopDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Max Bottom Diameter", nameof(ScrewBoltPattern.MaxBottomDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Nut Height", nameof(ScrewBoltPattern.NutHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Nut Diameter", nameof(ScrewBoltPattern.NutDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Screw Diameter", nameof(ScrewBoltPattern.ScrewDiameter), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Binding Length", nameof(ScrewBoltPattern.BindingLength), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Ignore Max Gap", nameof(ScrewBoltPattern.IgnoreMaxGap));

      InsertItem(dictionary, objectASType, "Weight", nameof(ScrewBoltPattern.GetWeight), eUnitType.kWeight);

      InsertItem(dictionary, "Screw Bolt Type", GetScrewBoltType);
      InsertItem(dictionary, "Assembly Location", GetAssemblyLocation);

      return dictionary;
    }

    private object GetScrewBoltType(object screwBoltPattern)
    {
      ScrewBoltPattern asScrewBoltPattern = screwBoltPattern as ScrewBoltPattern;

      return asScrewBoltPattern.ScrewBoltType.ToString();
    }

    private object GetAssemblyLocation(object screwBoltPattern)
    {
      ScrewBoltPattern asScrewBoltPattern = screwBoltPattern as ScrewBoltPattern;

      return asScrewBoltPattern.AssemblyLocation.ToString();
    }
  }
}
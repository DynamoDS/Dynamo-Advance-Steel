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
  public class WeldPatternProperties : BaseProperties<WeldPattern>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Thickness", nameof(WeldPattern.Thickness), eUnitType.kDistance);
      InsertProperty(dictionary, "Main Effective Throat", nameof(WeldPattern.MainEffectiveThroat), eUnitType.kDistance);
      InsertProperty(dictionary, "Double Effective Throat", nameof(WeldPattern.DoubleEffectiveThroat), eUnitType.kDistance);
      InsertProperty(dictionary, "Main Preparation Depth", nameof(WeldPattern.MainPreparationDepth), eUnitType.kDistance);
      InsertProperty(dictionary, "Double Preparation Depth", nameof(WeldPattern.DoublePreparationDepth), eUnitType.kDistance);
      InsertProperty(dictionary, "Prefix", nameof(WeldPattern.Prefix));
      InsertProperty(dictionary, "Closed", nameof(WeldPattern.IsClosed));
      InsertProperty(dictionary, "Length", nameof(WeldPattern.Length), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Double Weld Text", nameof(WeldPattern.DoubleWeldText));
      InsertProperty(dictionary, "Double Root Opening", nameof(WeldPattern.DoubleRootOpening), eUnitType.kDistance);
      InsertProperty(dictionary, "Main Weld Text", nameof(WeldPattern.MainWeldText));
      InsertProperty(dictionary, "Pitch", nameof(WeldPattern.Pitch), eUnitType.kDistance);
      InsertProperty(dictionary, "Additional Data", nameof(WeldPattern.AdditionalData));
      InsertProperty(dictionary, "Single Seam Length", nameof(WeldPattern.SingleSeamLength), eUnitType.kDistance);
      InsertProperty(dictionary, "Seam Distance", nameof(WeldPattern.SeamDistance), eUnitType.kDistance);
      InsertProperty(dictionary, "Main Root Opening", nameof(WeldPattern.MainRootOpening), eUnitType.kDistance);

      InsertProperty(dictionary, "Combining Welding", nameof(WeldPattern.IsCombiWelding));
      InsertProperty(dictionary, "Weld Center Point", nameof(WeldPattern.GetCenterPoint));

      return dictionary;
    }

  }
}
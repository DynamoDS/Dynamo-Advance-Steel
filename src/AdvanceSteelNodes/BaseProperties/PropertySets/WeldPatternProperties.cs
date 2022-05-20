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
  public class WeldPatternProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(WeldPattern);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Thickness", nameof(WeldPattern.Thickness), eUnitType.kDistance);
      InsertItem(dictionary, "Main Effective Throat", nameof(WeldPattern.MainEffectiveThroat), eUnitType.kDistance);
      InsertItem(dictionary, "Double Effective Throat", nameof(WeldPattern.DoubleEffectiveThroat), eUnitType.kDistance);
      InsertItem(dictionary, "Main Preparation Depth", nameof(WeldPattern.MainPreparationDepth), eUnitType.kDistance);
      InsertItem(dictionary, "Double Preparation Depth", nameof(WeldPattern.DoublePreparationDepth), eUnitType.kDistance);
      InsertItem(dictionary, "Prefix", nameof(WeldPattern.Prefix));
      InsertItem(dictionary, "Closed", nameof(WeldPattern.IsClosed));
      InsertItem(dictionary, "Length", nameof(WeldPattern.Length), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Double Weld Text", nameof(WeldPattern.DoubleWeldText));
      InsertItem(dictionary, "Double Root Opening", nameof(WeldPattern.DoubleRootOpening), eUnitType.kDistance);
      InsertItem(dictionary, "Main Weld Text", nameof(WeldPattern.MainWeldText));
      InsertItem(dictionary, "Pitch", nameof(WeldPattern.Pitch), eUnitType.kDistance);
      InsertItem(dictionary, "Additional Data", nameof(WeldPattern.AdditionalData));
      InsertItem(dictionary, "Single Seam Length", nameof(WeldPattern.SingleSeamLength), eUnitType.kDistance);
      InsertItem(dictionary, "Seam Distance", nameof(WeldPattern.SeamDistance), eUnitType.kDistance);
      InsertItem(dictionary, "Main Root Opening", nameof(WeldPattern.MainRootOpening), eUnitType.kDistance);

      InsertItem(dictionary, "Combining Welding", nameof(WeldPattern.IsCombiWelding));
      InsertItem(dictionary, "Weld Center Point", nameof(WeldPattern.GetCenterPoint));

      return dictionary;
    }

  }
}
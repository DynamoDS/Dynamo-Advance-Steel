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
    public override eObjectType GetObjectType => eObjectType.kWeldPattern;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Thickness", nameof(WeldPattern.Thickness), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Main Effective Throat", nameof(WeldPattern.MainEffectiveThroat), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Double Effective Throat", nameof(WeldPattern.DoubleEffectiveThroat), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Main Preparation Depth", nameof(WeldPattern.MainPreparationDepth), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Double Preparation Depth", nameof(WeldPattern.DoublePreparationDepth), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Prefix", nameof(WeldPattern.Prefix));
      InsertItem(dictionary, objectASType, "Closed", nameof(WeldPattern.IsClosed));
      InsertItem(dictionary, objectASType, "Length", nameof(WeldPattern.Length), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Double Weld Text", nameof(WeldPattern.DoubleWeldText));
      InsertItem(dictionary, objectASType, "Double Root Opening", nameof(WeldPattern.DoubleRootOpening), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Main Weld Text", nameof(WeldPattern.MainWeldText));
      InsertItem(dictionary, objectASType, "Pitch", nameof(WeldPattern.Pitch), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Additional Data", nameof(WeldPattern.AdditionalData));
      InsertItem(dictionary, objectASType, "Single Seam Length", nameof(WeldPattern.SingleSeamLength), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Seam Distance", nameof(WeldPattern.SeamDistance), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Main Root Opening", nameof(WeldPattern.MainRootOpening), eUnitType.kDistance);

      InsertItem(dictionary, objectASType, "Combining Welding", nameof(WeldPattern.IsCombiWelding));
      InsertItem(dictionary, objectASType, "Center Point", nameof(WeldPattern.GetCenterPoint));

      return dictionary;
    }

  }
}
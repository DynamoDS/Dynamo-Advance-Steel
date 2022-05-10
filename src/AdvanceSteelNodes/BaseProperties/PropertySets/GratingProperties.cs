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
  public class GratingProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kGrating;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Bearing Bar Spacing", nameof(Grating.BearingBarSpacing), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Cross Bar Quantity", nameof(Grating.CrossBarQuantity), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Bearing Bar Quantity", nameof(Grating.BearingBarQuantity), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Connector Key", nameof(Grating.ConnectorKey));
      InsertItem(dictionary, objectASType, "OED Value", nameof(Grating.OEDValue), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "ED Value", nameof(Grating.EDValue));
      InsertItem(dictionary, objectASType, "Is Using Standard ED", nameof(Grating.IsUsingStandardED));
      InsertItem(dictionary, objectASType, "Width Extension Right", nameof(Grating.WidthExtensionRight), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Width Extension Left", nameof(Grating.WidthExtensionLeft), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Grating Class", nameof(Grating.GratingClass));
      InsertItem(dictionary, objectASType, "Grating Size", nameof(Grating.GratingSize), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "BearingBar Spacing Distance", nameof(Grating.BearingBarSpacingDistance), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Is MatCoatDbDefined", nameof(Grating.IsMatCoatDbDefined));
      InsertItem(dictionary, objectASType, "Direction", nameof(Grating.Direction));
      InsertItem(dictionary, objectASType, "Is Top On Upper Plane", nameof(Grating.IsTopOnUpperPlane));
      InsertItem(dictionary, objectASType, "Connector Quantity", nameof(Grating.ConnectorQuantity));
      InsertItem(dictionary, objectASType, "Connector Name", nameof(Grating.ConnectorName));
      InsertItem(dictionary, objectASType, "CrossBar", nameof(Grating.CrossBar));
      InsertItem(dictionary, objectASType, "CrossBar Spacing Distance", nameof(Grating.CrossBarSpacingDistance), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Bearing Bar Thickness", nameof(Grating.ThicknessOfABearingBar), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "CrossBar Spacing", nameof(Grating.CrossBarSpacing), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Using Standard Hatch", nameof(Grating.IsUsingStandardHatch));
      InsertItem(dictionary, objectASType, "Standard Hatch", nameof(Grating.StandardHatch), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Thickness of CrossBar", nameof(Grating.ThicknessOfACrossBar), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Custom Hatch", nameof(Grating.CustomHatch));

      InsertItem(dictionary, objectASType, "Bar Grating Product Name", nameof(Grating.GetBarGratingProductName));
      InsertItem(dictionary, objectASType, "Center Point on Top", nameof(Grating.GetCenterOnTop));
      InsertItem(dictionary, objectASType, "Exact Paint Area", nameof(Grating.GetExactPaintArea), eUnitType.kArea);
      InsertItem(dictionary, objectASType, "Top Normal", nameof(Grating.GetTopNormal));
      InsertItem(dictionary, objectASType, "top Plane", nameof(Grating.GetTopPlane));
      InsertItem(dictionary, objectASType, "Y Direction", nameof(Grating.GetYDir));

      InsertItem(dictionary, "Connector Type", GetConnectorType);
      InsertItem(dictionary, "Grating Type", GetGratingType);

      return dictionary;
    }

    private object GetConnectorType(object grating)
    {
      Grating asGrating = grating as Grating;

      return asGrating.ConnectorType.ToString();
    }

    private object GetGratingType(object grating)
    {
      Grating asGrating = grating as Grating;

      return asGrating.GratingType.ToString();
    }
  }
}
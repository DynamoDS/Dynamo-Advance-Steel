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
    public override Type GetObjectType => typeof(Grating);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Bearing Bar Spacing", nameof(Grating.BearingBarSpacing), LevelEnum.Default);
      InsertItem(dictionary, "Cross Bar Quantity", nameof(Grating.CrossBarQuantity), LevelEnum.Default);
      InsertItem(dictionary, "Bearing Bar Quantity", nameof(Grating.BearingBarQuantity), LevelEnum.Default);
      InsertItem(dictionary, "Connector Key", nameof(Grating.ConnectorKey));
      InsertItem(dictionary, "OED Value", nameof(Grating.OEDValue), LevelEnum.Default);
      InsertItem(dictionary, "ED Value", nameof(Grating.EDValue));
      InsertItem(dictionary, "Is Using Standard ED", nameof(Grating.IsUsingStandardED));
      InsertItem(dictionary, "Width Extension Right", nameof(Grating.WidthExtensionRight), eUnitType.kDistance);
      InsertItem(dictionary, "Width Extension Left", nameof(Grating.WidthExtensionLeft), eUnitType.kDistance);
      InsertItem(dictionary, "Grating Class", nameof(Grating.GratingClass));
      InsertItem(dictionary, "Grating Size", nameof(Grating.GratingSize), eUnitType.kDistance);
      InsertItem(dictionary, "BearingBar Spacing Distance", nameof(Grating.BearingBarSpacingDistance), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Is MatCoatDbDefined", nameof(Grating.IsMatCoatDbDefined));
      InsertItem(dictionary, "Direction", nameof(Grating.Direction));
      InsertItem(dictionary, "Is Top On Upper Plane", nameof(Grating.IsTopOnUpperPlane));
      InsertItem(dictionary, "Connector Quantity", nameof(Grating.ConnectorQuantity));
      InsertItem(dictionary, "Connector Name", nameof(Grating.ConnectorName));
      InsertItem(dictionary, "CrossBar", nameof(Grating.CrossBar));
      InsertItem(dictionary, "CrossBar Spacing Distance", nameof(Grating.CrossBarSpacingDistance), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Bearing Bar Thickness", nameof(Grating.ThicknessOfABearingBar), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "CrossBar Spacing", nameof(Grating.CrossBarSpacing), LevelEnum.Default);
      InsertItem(dictionary, "Using Standard Hatch", nameof(Grating.IsUsingStandardHatch));
      InsertItem(dictionary, "Standard Hatch", nameof(Grating.StandardHatch), LevelEnum.Default);
      InsertItem(dictionary, "Thickness of CrossBar", nameof(Grating.ThicknessOfACrossBar), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Custom Hatch", nameof(Grating.CustomHatch));

      InsertItem(dictionary, "Bar Grating Product Name", nameof(Grating.GetBarGratingProductName));
      InsertItem(dictionary, "Center Point on Top", nameof(Grating.GetCenterOnTop));
      InsertItem(dictionary, "Exact Paint Area", nameof(Grating.GetExactPaintArea), eUnitType.kArea);
      InsertItem(dictionary, "Top Normal", nameof(Grating.GetTopNormal));
      InsertItem(dictionary, "top Plane", nameof(Grating.GetTopPlane));
      InsertItem(dictionary, "Y Direction", nameof(Grating.GetYDir));

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
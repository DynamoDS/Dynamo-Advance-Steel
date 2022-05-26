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
  public class GratingProperties : BaseProperties<Grating>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Bearing Bar Spacing", nameof(Grating.BearingBarSpacing), LevelEnum.Default);
      InsertProperty(dictionary, "Cross Bar Quantity", nameof(Grating.CrossBarQuantity), LevelEnum.Default);
      InsertProperty(dictionary, "Bearing Bar Quantity", nameof(Grating.BearingBarQuantity), LevelEnum.Default);
      InsertProperty(dictionary, "Connector Key", nameof(Grating.ConnectorKey));
      InsertProperty(dictionary, "OED Value", nameof(Grating.OEDValue), LevelEnum.Default);
      InsertProperty(dictionary, "ED Value", nameof(Grating.EDValue));
      InsertProperty(dictionary, "Is Using Standard ED", nameof(Grating.IsUsingStandardED));
      InsertProperty(dictionary, "Width Extension Right", nameof(Grating.WidthExtensionRight), eUnitType.kDistance);
      InsertProperty(dictionary, "Width Extension Left", nameof(Grating.WidthExtensionLeft), eUnitType.kDistance);
      InsertProperty(dictionary, "Grating Class", nameof(Grating.GratingClass));
      InsertProperty(dictionary, "Grating Size", nameof(Grating.GratingSize), eUnitType.kDistance);
      InsertProperty(dictionary, "BearingBar Spacing Distance", nameof(Grating.BearingBarSpacingDistance), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Is MatCoatDbDefined", nameof(Grating.IsMatCoatDbDefined));
      InsertProperty(dictionary, "Direction", nameof(Grating.Direction));
      InsertProperty(dictionary, "Is Top On Upper Plane", nameof(Grating.IsTopOnUpperPlane));
      InsertProperty(dictionary, "Connector Quantity", nameof(Grating.ConnectorQuantity));
      InsertProperty(dictionary, "Connector Name", nameof(Grating.ConnectorName));
      InsertProperty(dictionary, "CrossBar", nameof(Grating.CrossBar));
      InsertProperty(dictionary, "CrossBar Spacing Distance", nameof(Grating.CrossBarSpacingDistance), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Bearing Bar Thickness", nameof(Grating.ThicknessOfABearingBar), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "CrossBar Spacing", nameof(Grating.CrossBarSpacing), LevelEnum.Default);
      InsertProperty(dictionary, "Using Standard Hatch", nameof(Grating.IsUsingStandardHatch));
      InsertProperty(dictionary, "Standard Hatch", nameof(Grating.StandardHatch), LevelEnum.Default);
      InsertProperty(dictionary, "Thickness of CrossBar", nameof(Grating.ThicknessOfACrossBar), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Custom Hatch", nameof(Grating.CustomHatch));

      InsertProperty(dictionary, "Bar Grating Product Name", nameof(Grating.GetBarGratingProductName));
      InsertProperty(dictionary, "Center Point on Top", nameof(Grating.GetCenterOnTop));
      InsertProperty(dictionary, "Exact Paint Area", nameof(Grating.GetExactPaintArea), eUnitType.kArea);
      InsertProperty(dictionary, "Top Normal", nameof(Grating.GetTopNormal));
      InsertProperty(dictionary, "top Plane", nameof(Grating.GetTopPlane));
      InsertProperty(dictionary, "Y Direction", nameof(Grating.GetYDir));

      InsertCustomProperty(dictionary, "Connector Type", nameof(GratingProperties.GetConnectorType), null);
      InsertCustomProperty(dictionary, "Grating Type", nameof(GratingProperties.GetGratingType), null);

      return dictionary;
    }

    private static string GetConnectorType(Grating grating)
    {
      return grating.ConnectorType.ToString();
    }

    private static string GetGratingType(Grating grating)
    {
      return grating.GratingType.ToString();
    }
  }
}
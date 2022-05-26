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
  public class AnchorPatternProperties : BaseProperties<AnchorPattern>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Size Y Direction", nameof(AnchorPattern.Wy), eUnitType.kDistance);
      InsertProperty(dictionary, "Size X Direction", nameof(AnchorPattern.Wx), eUnitType.kDistance);
      InsertProperty(dictionary, "Screws Number", nameof(AnchorPattern.NumberOfScrews));
      InsertProperty(dictionary, "Spacing Y Direction", nameof(AnchorPattern.Dy), eUnitType.kDistance);
      InsertProperty(dictionary, "Spacing X Direction", nameof(AnchorPattern.Dx), eUnitType.kDistance);
      InsertProperty(dictionary, "Count X Direction", nameof(AnchorPattern.Nx));
      InsertProperty(dictionary, "Count Y Direction", nameof(AnchorPattern.Ny));
      InsertProperty(dictionary, "Unfolded Length", nameof(AnchorPattern.AnchorUnfoldedLength), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Thread Length", nameof(AnchorPattern.ThreadLength), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Midpoint On Upper Right", nameof(AnchorPattern.MidpointOnUpperRight), LevelEnum.Default);
      InsertProperty(dictionary, "Midpoint On Lower Left", nameof(AnchorPattern.MidpointOnLowerLeft), LevelEnum.Default);
      InsertProperty(dictionary, "Midpoint On Lower Right", nameof(AnchorPattern.MidpointOnLowerRight), LevelEnum.Default);
      InsertProperty(dictionary, "Midpoint On Upper Left", nameof(AnchorPattern.MidpointOnUpperLeft), LevelEnum.Default);
      InsertProperty(dictionary, "Radius", nameof(AnchorPattern.Radius), eUnitType.kDistance);
      InsertProperty(dictionary, "Part Name", nameof(AnchorPattern.AnchorPartName), LevelEnum.Default);

      InsertCustomProperty(dictionary, "Orientation Type", nameof(AnchorPatternProperties.GetOrientationType), null);

      return dictionary;
    }

    private string GetOrientationType(AnchorPattern anchorPattern)
    {
      return anchorPattern.OrientationType.ToString();
    }
  }
}
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
  public class AnchorPatternProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(AnchorPattern);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Size Y Direction", nameof(AnchorPattern.Wy), eUnitType.kDistance);
      InsertItem(dictionary, "Size X Direction", nameof(AnchorPattern.Wx), eUnitType.kDistance);
      InsertItem(dictionary, "Screws Number", nameof(AnchorPattern.NumberOfScrews));
      InsertItem(dictionary, "Spacing Y Direction", nameof(AnchorPattern.Dy), eUnitType.kDistance);
      InsertItem(dictionary, "Spacing X Direction", nameof(AnchorPattern.Dx), eUnitType.kDistance);
      InsertItem(dictionary, "Count X Direction", nameof(AnchorPattern.Nx));
      InsertItem(dictionary, "Count Y Direction", nameof(AnchorPattern.Ny));
      InsertItem(dictionary, "Unfolded Length", nameof(AnchorPattern.AnchorUnfoldedLength), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Thread Length", nameof(AnchorPattern.ThreadLength), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Midpoint On Upper Right", nameof(AnchorPattern.MidpointOnUpperRight), LevelEnum.Default);
      InsertItem(dictionary, "Midpoint On Lower Left", nameof(AnchorPattern.MidpointOnLowerLeft), LevelEnum.Default);
      InsertItem(dictionary, "Midpoint On Lower Right", nameof(AnchorPattern.MidpointOnLowerRight), LevelEnum.Default);
      InsertItem(dictionary, "Midpoint On Upper Left", nameof(AnchorPattern.MidpointOnUpperLeft), LevelEnum.Default);
      InsertItem(dictionary, "Radius", nameof(AnchorPattern.Radius), eUnitType.kDistance);
      InsertItem(dictionary, "Part Name", nameof(AnchorPattern.AnchorPartName), LevelEnum.Default);
      InsertItem(dictionary, "Height", nameof(AnchorPattern.Height), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Max Top Diameter", nameof(AnchorPattern.MaxTopDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Max Bottom Diameter", nameof(AnchorPattern.MaxBottomDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Length", nameof(AnchorPattern.Length), LevelEnum.Default, eUnitType.kDistance);

      InsertItem(dictionary, "Weight", nameof(AnchorPattern.GetWeight), eUnitType.kWeight);

      InsertItem(dictionary, "Orientation Type", GetOrientationType);

      return dictionary;
    }

    private object GetOrientationType(object anchorPattern)
    {
      return ((AnchorPattern)anchorPattern).OrientationType.ToString();
    }
  }
}
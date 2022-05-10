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
    public override eObjectType GetObjectType => eObjectType.kAnchorPattern;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Size Y Direction", nameof(AnchorPattern.Wy), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Size X Direction", nameof(AnchorPattern.Wx), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Screws Number", nameof(AnchorPattern.NumberOfScrews));
      InsertItem(dictionary, objectASType, "Spacing Y Direction", nameof(AnchorPattern.Dy), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Spacing X Direction", nameof(AnchorPattern.Dx), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Count X Direction", nameof(AnchorPattern.Nx));
      InsertItem(dictionary, objectASType, "Count Y Direction", nameof(AnchorPattern.Ny));
      InsertItem(dictionary, objectASType, "Unfolded Length", nameof(AnchorPattern.AnchorUnfoldedLength), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Thread Length", nameof(AnchorPattern.ThreadLength), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Midpoint On Upper Right", nameof(AnchorPattern.MidpointOnUpperRight), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Midpoint On Lower Left", nameof(AnchorPattern.MidpointOnLowerLeft), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Midpoint On Lower Right", nameof(AnchorPattern.MidpointOnLowerRight), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Midpoint On Upper Left", nameof(AnchorPattern.MidpointOnUpperLeft), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Radius", nameof(AnchorPattern.Radius), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Part Name", nameof(AnchorPattern.AnchorPartName), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Height", nameof(AnchorPattern.Height), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Max Top Diameter", nameof(AnchorPattern.MaxTopDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Max Bottom Diameter", nameof(AnchorPattern.MaxBottomDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Length", nameof(AnchorPattern.Length), LevelEnum.Default, eUnitType.kDistance);

      InsertItem(dictionary, objectASType, "Weight", nameof(AnchorPattern.GetWeight), eUnitType.kWeight);

      InsertItem(dictionary, "Orientation Type", GetOrientationType);

      return dictionary;
    }

    private object GetOrientationType(object anchorPattern)
    {
      return ((AnchorPattern)anchorPattern).OrientationType.ToString();
    }
  }
}
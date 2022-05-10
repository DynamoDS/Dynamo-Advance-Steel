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
  public class PlateContourNotchProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kPlateContourNotch;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Gap", nameof(PlateContourNotch.Gap), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Length", nameof(PlateContourNotch.Length), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Length Increment", nameof(PlateContourNotch.LengthIncrement), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Radius", nameof(PlateContourNotch.Radius), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Radius Increment", nameof(PlateContourNotch.RadIncrement), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Width", nameof(PlateContourNotch.Width), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Boring Out Option", nameof(PlateContourNotch.BoringOut));
      InsertItem(dictionary, objectASType, "Normal", nameof(PlateContourNotch.Normal), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Straight Cut", nameof(PlateContourNotch.IsStraightCut));
      InsertItem(dictionary, objectASType, "Offset", nameof(PlateContourNotch.Offset));

      InsertItem(dictionary, "Contour Type", ContourType);
      InsertItem(dictionary, "Clip Type", ClipType);

      return dictionary;
    }

    private object ContourType(object plateContourNotch)
    {
      PlateContourNotch asPlateContourNotch = plateContourNotch as PlateContourNotch;
      return asPlateContourNotch.ContourType.ToString();
    }

    private object ClipType(object plateContourNotch)
    {
      PlateContourNotch asPlateContourNotch = plateContourNotch as PlateContourNotch;
      return asPlateContourNotch.ClipType.ToString();
    }
  }
}
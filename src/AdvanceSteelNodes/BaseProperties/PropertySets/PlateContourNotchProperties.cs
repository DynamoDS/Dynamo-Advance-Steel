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
    public override Type GetObjectType => typeof(PlateContourNotch);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Gap", nameof(PlateContourNotch.Gap), eUnitType.kDistance);
      InsertItem(dictionary, "Length", nameof(PlateContourNotch.Length), eUnitType.kDistance);
      InsertItem(dictionary, "Length Increment", nameof(PlateContourNotch.LengthIncrement), eUnitType.kDistance);
      InsertItem(dictionary, "Radius", nameof(PlateContourNotch.Radius), eUnitType.kDistance);
      InsertItem(dictionary, "Radius Increment", nameof(PlateContourNotch.RadIncrement), eUnitType.kDistance);
      InsertItem(dictionary, "Width", nameof(PlateContourNotch.Width), eUnitType.kDistance);
      InsertItem(dictionary, "Boring Out Option", nameof(PlateContourNotch.BoringOut));
      InsertItem(dictionary, "Normal", nameof(PlateContourNotch.Normal), LevelEnum.Default);
      InsertItem(dictionary, "Straight Cut", nameof(PlateContourNotch.IsStraightCut));
      InsertItem(dictionary, "Offset", nameof(PlateContourNotch.Offset));

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
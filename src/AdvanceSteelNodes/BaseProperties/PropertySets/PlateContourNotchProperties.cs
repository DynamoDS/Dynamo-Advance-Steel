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
  public class PlateContourNotchProperties : BaseProperties<PlateContourNotch>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Gap", nameof(PlateContourNotch.Gap), eUnitType.kDistance);
      InsertProperty(dictionary, "Length", nameof(PlateContourNotch.Length), eUnitType.kDistance);
      InsertProperty(dictionary, "Length Increment", nameof(PlateContourNotch.LengthIncrement), eUnitType.kDistance);
      InsertProperty(dictionary, "Radius", nameof(PlateContourNotch.Radius), eUnitType.kDistance);
      InsertProperty(dictionary, "Radius Increment", nameof(PlateContourNotch.RadIncrement), eUnitType.kDistance);
      InsertProperty(dictionary, "Width", nameof(PlateContourNotch.Width), eUnitType.kDistance);
      InsertProperty(dictionary, "Boring Out Option", nameof(PlateContourNotch.BoringOut));
      InsertProperty(dictionary, "Normal", nameof(PlateContourNotch.Normal), LevelEnum.Default);
      InsertProperty(dictionary, "Straight Cut", nameof(PlateContourNotch.IsStraightCut));
      InsertProperty(dictionary, "Offset", nameof(PlateContourNotch.Offset));

      InsertCustomProperty(dictionary, "Contour Type", nameof(PlateContourNotchProperties.ContourType), null);
      InsertCustomProperty(dictionary, "Clip Type", nameof(PlateContourNotchProperties.ClipType), null);

      return dictionary;
    }

    private static string ContourType(PlateContourNotch plateContourNotch)
    {
      return plateContourNotch.ContourType.ToString();
    }

    private static string ClipType(PlateContourNotch plateContourNotch)
    {
      return plateContourNotch.ClipType.ToString();
    }
  }
}
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
  public class BeamMultiContourNotchProperties : BaseProperties<BeamMultiContourNotch>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Gap", nameof(BeamMultiContourNotch.Gap), eUnitType.kDistance);
      InsertProperty(dictionary, "Radius Increment", nameof(BeamMultiContourNotch.RadIncrement), eUnitType.kDistance);
      InsertProperty(dictionary, "BoringOut", nameof(BeamMultiContourNotch.BoringOut));
      InsertProperty(dictionary, "Normal", nameof(BeamMultiContourNotch.Normal), LevelEnum.Default);
      InsertProperty(dictionary, "Length", nameof(BeamMultiContourNotch.Length), eUnitType.kDistance);
      InsertProperty(dictionary, "Width", nameof(BeamMultiContourNotch.Width), eUnitType.kDistance);
      InsertProperty(dictionary, "Length Increment", nameof(BeamMultiContourNotch.LengthIncrement), eUnitType.kDistance);
      InsertProperty(dictionary, "Radius", nameof(BeamMultiContourNotch.Radius), eUnitType.kDistance);

      InsertProperty(dictionary, "Offset", nameof(BeamMultiContourNotch.Offset));
      InsertProperty(dictionary, "Lower Clip", nameof(BeamMultiContourNotch.GetLowerClip));

      InsertCustomProperty(dictionary, "Contour Type", nameof(BeamMultiContourNotchProperties.GetContourType), null);
      InsertCustomProperty(dictionary, "End", nameof(BeamMultiContourNotchProperties.GetEnd), null);
      InsertCustomProperty(dictionary, "Clip Type", nameof(BeamMultiContourNotchProperties.GetClipType), null);

      return dictionary;
    }

    private string GetContourType(BeamMultiContourNotch beamNotch)
    {
      return beamNotch.ContourType.ToString();
    }

    private string GetEnd(BeamMultiContourNotch beamNotch)
    {
      return beamNotch.End.ToString();
    }

    private string GetClipType(BeamMultiContourNotch beamNotch)
    {
      return beamNotch.ClipType.ToString();
    }
  }
}

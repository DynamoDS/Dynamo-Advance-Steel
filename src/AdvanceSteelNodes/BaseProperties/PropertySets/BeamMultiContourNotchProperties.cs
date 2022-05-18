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
  public class BeamMultiContourNotchProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(BeamMultiContourNotch);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Gap", nameof(BeamMultiContourNotch.Gap), eUnitType.kDistance);
      InsertItem(dictionary, "Radius Increment", nameof(BeamMultiContourNotch.RadIncrement), eUnitType.kDistance);
      InsertItem(dictionary, "BoringOut", nameof(BeamMultiContourNotch.BoringOut));
      InsertItem(dictionary, "Normal", nameof(BeamMultiContourNotch.Normal), LevelEnum.Default);
      InsertItem(dictionary, "Length", nameof(BeamMultiContourNotch.Length), eUnitType.kDistance);
      InsertItem(dictionary, "Width", nameof(BeamMultiContourNotch.Width), eUnitType.kDistance);
      InsertItem(dictionary, "Length Increment", nameof(BeamMultiContourNotch.LengthIncrement), eUnitType.kDistance);
      InsertItem(dictionary, "Radius", nameof(BeamMultiContourNotch.Radius), eUnitType.kDistance);

      InsertItem(dictionary, "Offset", nameof(BeamMultiContourNotch.Offset));
      InsertItem(dictionary, "Lower Clip", nameof(BeamMultiContourNotch.GetLowerClip));

      InsertItem(dictionary, "Contour Type", GetContourType);
      InsertItem(dictionary, "End", GetEnd);
      InsertItem(dictionary, "Clip Type", GetClipType);

      return dictionary;
    }

    private object GetContourType(object beamNotch)
    {
      BeamMultiContourNotch asBeamNotch = beamNotch as BeamMultiContourNotch;

      return asBeamNotch.ContourType.ToString();
    }

    private object GetEnd(object beamNotch)
    {
      BeamMultiContourNotch asBeamNotch = beamNotch as BeamMultiContourNotch;

      return asBeamNotch.End.ToString();
    }

    private object GetClipType(object beamNotch)
    {
      BeamMultiContourNotch asBeamNotch = beamNotch as BeamMultiContourNotch;

      return asBeamNotch.ClipType.ToString();
    }
  }
}

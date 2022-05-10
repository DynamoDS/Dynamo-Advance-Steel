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
    public override eObjectType GetObjectType => eObjectType.kBeamMultiContourNotch;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Gap", nameof(BeamMultiContourNotch.Gap), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Radius Increment", nameof(BeamMultiContourNotch.RadIncrement), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "BoringOut", nameof(BeamMultiContourNotch.BoringOut));
      InsertItem(dictionary, objectASType, "Normal", nameof(BeamMultiContourNotch.Normal), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Length", nameof(BeamMultiContourNotch.Length), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Width", nameof(BeamMultiContourNotch.Width), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Length Increment", nameof(BeamMultiContourNotch.LengthIncrement), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Radius", nameof(BeamMultiContourNotch.Radius), eUnitType.kDistance);

      InsertItem(dictionary, objectASType, "Offset", nameof(BeamMultiContourNotch.Offset));
      InsertItem(dictionary, objectASType, "Lower Clip", nameof(BeamMultiContourNotch.GetLowerClip));

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

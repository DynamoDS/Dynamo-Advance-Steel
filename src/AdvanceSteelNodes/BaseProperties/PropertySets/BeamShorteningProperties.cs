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
  public class BeamShorteningProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kBeamShortening;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Ins Length", nameof(BeamShortening.InsLength), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Cut Straight Relative Offset", nameof(BeamShortening.CutStraightRelativeOffset), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Cut Straight", nameof(BeamShortening.CutStraight), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Cut Straight Type", nameof(BeamShortening.CutStraightType), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Angle On Y", nameof(BeamShortening.AngleOnY), eUnitType.kAngle);
      InsertItem(dictionary, objectASType, "Cut Straight Offset", nameof(BeamShortening.CutStraightOffset), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Angle On Z", nameof(BeamShortening.AngleOnZ), eUnitType.kAngle);

      InsertItem(dictionary, "End", GetEnd);

      return dictionary;
    }

    private object GetEnd(object beamShortening)
    {
      BeamShortening asBeamShortening = beamShortening as BeamShortening;

      return asBeamShortening.End.ToString();
    }
  }
}
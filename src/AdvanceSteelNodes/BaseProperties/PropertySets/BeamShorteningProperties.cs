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
    public override Type GetObjectType => typeof(BeamShortening);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Ins Length", nameof(BeamShortening.InsLength), eUnitType.kDistance);
      InsertItem(dictionary, "Cut Straight Relative Offset", nameof(BeamShortening.CutStraightRelativeOffset), eUnitType.kDistance);
      InsertItem(dictionary, "Cut Straight", nameof(BeamShortening.CutStraight), eUnitType.kDistance);
      InsertItem(dictionary, "Cut Straight Type", nameof(BeamShortening.CutStraightType), eUnitType.kDistance);
      InsertItem(dictionary, "Angle On Y", nameof(BeamShortening.AngleOnY), eUnitType.kAngle);
      InsertItem(dictionary, "Cut Straight Offset", nameof(BeamShortening.CutStraightOffset), eUnitType.kDistance);
      InsertItem(dictionary, "Angle On Z", nameof(BeamShortening.AngleOnZ), eUnitType.kAngle);

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
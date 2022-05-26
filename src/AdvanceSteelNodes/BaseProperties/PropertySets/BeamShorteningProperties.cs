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
  public class BeamShorteningProperties : BaseProperties<BeamShortening>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Ins Length", nameof(BeamShortening.InsLength), eUnitType.kDistance);
      InsertProperty(dictionary, "Cut Straight Relative Offset", nameof(BeamShortening.CutStraightRelativeOffset), eUnitType.kDistance);
      InsertProperty(dictionary, "Cut Straight", nameof(BeamShortening.CutStraight), eUnitType.kDistance);
      InsertProperty(dictionary, "Cut Straight Type", nameof(BeamShortening.CutStraightType), eUnitType.kDistance);
      InsertProperty(dictionary, "Angle On Y", nameof(BeamShortening.AngleOnY), eUnitType.kAngle);
      InsertProperty(dictionary, "Cut Straight Offset", nameof(BeamShortening.CutStraightOffset), eUnitType.kDistance);
      InsertProperty(dictionary, "Angle On Z", nameof(BeamShortening.AngleOnZ), eUnitType.kAngle);

      InsertCustomProperty(dictionary, "End", nameof(BeamShorteningProperties.GetEnd), null);

      return dictionary;
    }

    private string GetEnd(BeamShortening beamShortening)
    {
      return beamShortening.End.ToString();
    }
  }
}
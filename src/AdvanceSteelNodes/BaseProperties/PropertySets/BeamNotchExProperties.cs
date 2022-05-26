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
  public class BeamNotchExProperties : BaseProperties<BeamNotchEx>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Axis Angle", nameof(BeamNotchEx.AxisAngle), eUnitType.kAngle);
      InsertProperty(dictionary, "Z Angle", nameof(BeamNotchEx.ZAngle), eUnitType.kAngle);
      InsertProperty(dictionary, "X Angle", nameof(BeamNotchEx.XAngle), eUnitType.kAngle);

      InsertCustomProperty(dictionary, "X Axis Rotation", nameof(BeamNotchExProperties.GetXRotation), null);

      return dictionary;
    }

    private string GetXRotation(BeamNotchEx beamNotch)
    {
      return beamNotch.XRotation.ToString();
    }
  }
}
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
  public class BeamNotchExProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(BeamNotchEx);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Axis Angle", nameof(BeamNotchEx.AxisAngle), eUnitType.kAngle);
      InsertItem(dictionary, "Z Angle", nameof(BeamNotchEx.ZAngle), eUnitType.kAngle);
      InsertItem(dictionary, "X Angle", nameof(BeamNotchEx.XAngle), eUnitType.kAngle);

      InsertItem(dictionary, "X Axis Rotation", GetXRotation);

      return dictionary;
    }

    private object GetXRotation(object beamNotch)
    {
      BeamNotchEx asBeamNotch = beamNotch as BeamNotchEx;

      return asBeamNotch.XRotation.ToString();
    }
  }
}
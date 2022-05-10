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
    public override eObjectType GetObjectType => eObjectType.kBeamNotchEx;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Axis Angle", nameof(BeamNotchEx.AxisAngle), eUnitType.kAngle);
      InsertItem(dictionary, objectASType, "Z Angle", nameof(BeamNotchEx.ZAngle), eUnitType.kAngle);
      InsertItem(dictionary, objectASType, "X Angle", nameof(BeamNotchEx.XAngle), eUnitType.kAngle);

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
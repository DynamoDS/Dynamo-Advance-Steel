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
  public class BentBeamProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(BentBeam);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Offset Curve Radius", nameof(BentBeamBase.OffsetCurveRadius), eUnitType.kDistance);
      InsertItem(dictionary, "Curve Offset", nameof(BentBeamBase.CurveOffset), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Definition Plane Coordinate System", nameof(BentBeamBase.DefinitionPlane), LevelEnum.Default);
      InsertItem(dictionary, "Systemline Radius", nameof(BentBeamBase.SystemlineRadius), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Physical Length", nameof(BentBeamBase.GetPhysLength), eUnitType.kDistance);

      InsertItem(dictionary, "Arc Center", GetArcCenter);
      InsertItem(dictionary, "Arc Normal", GetArcNormal);

      return dictionary;
    }

    private object GetArcCenter(object beam)
    {
      ((BentBeamBase)beam).GetArcCenter(out var point, true);
      return point;
    }

    private object GetArcNormal(object beam)
    {
      ((BentBeamBase)beam).GetArcNormal(out var normal, true);
      return normal;
    }
  }
}

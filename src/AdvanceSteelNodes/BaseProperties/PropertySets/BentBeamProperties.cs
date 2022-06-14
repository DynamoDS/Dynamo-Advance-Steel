using Autodesk.AdvanceSteel.Geometry;
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
  public class BentBeamProperties : BaseProperties<BentBeam>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Offset Curve Radius", nameof(BentBeamBase.OffsetCurveRadius), eUnitType.kDistance);
      InsertProperty(dictionary, "Curve Offset", nameof(BentBeamBase.CurveOffset), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Definition Plane Coordinate System", nameof(BentBeamBase.DefinitionPlane), LevelEnum.Default);
      InsertProperty(dictionary, "Systemline Radius", nameof(BentBeamBase.SystemlineRadius), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Physical Length", nameof(BentBeamBase.GetPhysLength), eUnitType.kDistance);

      InsertCustomProperty(dictionary, "Arc Center", nameof(BentBeamProperties.GetArcCenter), null);
      InsertCustomProperty(dictionary, "Arc Normal", nameof(BentBeamProperties.GetArcNormal), null);

      return dictionary;
    }

    private static Point3d GetArcCenter(BentBeamBase beam)
    {
      beam.GetArcCenter(out var point, true);
      return point;
    }

    private static Vector3d GetArcNormal(BentBeamBase beam)
    {
      beam.GetArcNormal(out var normal, true);
      return normal;
    }
  }
}

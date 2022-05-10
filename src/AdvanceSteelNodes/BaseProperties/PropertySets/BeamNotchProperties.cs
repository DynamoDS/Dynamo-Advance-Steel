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
  public class BeamNotchProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kBeamNotch;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Radius", nameof(BeamNotch.CornerRadius), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Depth", nameof(BeamNotch.ReferenceDepth), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Length", nameof(BeamNotch.ReferenceLength), eUnitType.kDistance);

      InsertItem(dictionary, "Side", GetSide);
      InsertItem(dictionary, "End", GetEnd);
      InsertItem(dictionary, "Corner Type", GetCornerType);

      InsertItem(dictionary, "Plane 1", GetPlane1);
      InsertItem(dictionary, "Plane 2", GetPlane2);

      InsertItem(dictionary, "Axis Beam Intersection Point 1", GetAxisBeamIntersectionPoint1);
      InsertItem(dictionary, "Axis Beam Intersection Point 2", GetAxisBeamIntersectionPoint2);

      return dictionary;
    }

    private object GetSide(object beamNotch)
    {
      BeamNotch asBeamNotch = beamNotch as BeamNotch;

      return asBeamNotch.Side.ToString();
    }

    private object GetEnd(object beamNotch)
    {
      BeamNotch asBeamNotch = beamNotch as BeamNotch;

      return asBeamNotch.End.ToString();
    }

    private object GetCornerType(object beamNotch)
    {
      BeamNotch asBeamNotch = beamNotch as BeamNotch;

      return asBeamNotch.CornerType.ToString();
    }

    private object GetPlane1(object beamNotch)
    {
      BeamNotch asBeamNotch = beamNotch as BeamNotch;

      asBeamNotch.getPlanes(out var plane1, out var plane2);
      return plane1;
    }

    private object GetPlane2(object beamNotch)
    {
      BeamNotch asBeamNotch = beamNotch as BeamNotch;

      asBeamNotch.getPlanes(out var plane1, out var plane2);
      return plane2;
    }

    private object GetAxisBeamIntersectionPoint1(object beamNotch)
    {
      BeamNotch asBeamNotch = beamNotch as BeamNotch;

      asBeamNotch.getAxisBeamIntersection(out var point1, out var point2);
      return point1;
    }

    private object GetAxisBeamIntersectionPoint2(object beamNotch)
    {
      BeamNotch asBeamNotch = beamNotch as BeamNotch;

      asBeamNotch.getAxisBeamIntersection(out var point1, out var point2);
      return point2;
    }
  }
}
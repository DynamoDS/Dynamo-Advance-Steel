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
  public class BeamNotchProperties : BaseProperties<BeamNotch>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Radius", nameof(BeamNotch.CornerRadius), eUnitType.kDistance);
      InsertProperty(dictionary, "Depth", nameof(BeamNotch.ReferenceDepth), eUnitType.kDistance);
      InsertProperty(dictionary, "Length", nameof(BeamNotch.ReferenceLength), eUnitType.kDistance);

      InsertCustomProperty(dictionary, "Side", nameof(BeamNotchProperties.GetSide), null);
      InsertCustomProperty(dictionary, "End", nameof(BeamNotchProperties.GetEnd), null);
      InsertCustomProperty(dictionary, "Corner Type", nameof(BeamNotchProperties.GetCornerType), null);

      InsertCustomProperty(dictionary, "Plane 1", nameof(BeamNotchProperties.GetPlane1), null);
      InsertCustomProperty(dictionary, "Plane 2", nameof(BeamNotchProperties.GetPlane2), null);

      InsertCustomProperty(dictionary, "Axis Beam Intersection Point 1", nameof(BeamNotchProperties.GetAxisBeamIntersectionPoint1), null);
      InsertCustomProperty(dictionary, "Axis Beam Intersection Point 2", nameof(BeamNotchProperties.GetAxisBeamIntersectionPoint2), null);

      return dictionary;
    }

    private static string GetSide(BeamNotch beamNotch)
    {
      return beamNotch.Side.ToString();
    }

    private static string GetEnd(BeamNotch beamNotch)
    {
      return beamNotch.End.ToString();
    }

    private static string GetCornerType(BeamNotch beamNotch)
    {
      return beamNotch.CornerType.ToString();
    }

    private static Plane GetPlane1(BeamNotch beamNotch)
    {
      beamNotch.getPlanes(out var plane1, out var plane2);
      return plane1;
    }

    private static Plane GetPlane2(BeamNotch beamNotch)
    {
      beamNotch.getPlanes(out var plane1, out var plane2);
      return plane2;
    }

    private static Point3d GetAxisBeamIntersectionPoint1(BeamNotch beamNotch)
    {
      beamNotch.getAxisBeamIntersection(out var point1, out var point2);
      return point1;
    }

    private static Point3d GetAxisBeamIntersectionPoint2(BeamNotch beamNotch)
    {
      beamNotch.getAxisBeamIntersection(out var point1, out var point2);
      return point2;
    }
  }
}
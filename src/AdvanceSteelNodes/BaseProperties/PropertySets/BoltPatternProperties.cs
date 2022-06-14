using Autodesk.AdvanceSteel.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;
using static Autodesk.AdvanceSteel.DotNetRoots.Units.Unit;
using DSCoordinateSystem = Autodesk.DesignScript.Geometry.CoordinateSystem;

namespace AdvanceSteel.Nodes
{
  public class BoltPatternProperties : BaseProperties<BoltPattern>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Reference Point", nameof(BoltPattern.RefPoint));
      InsertProperty(dictionary, "Number of Screws", nameof(BoltPattern.NumberOfScrews), LevelEnum.Default);
      InsertProperty(dictionary, "Is Inverted", nameof(BoltPattern.IsInverted));
      InsertProperty(dictionary, "Center", nameof(BoltPattern.Center), LevelEnum.Default);
      InsertProperty(dictionary, "X Direction", nameof(BoltPattern.XDirection));
      InsertProperty(dictionary, "Bolt Normal", nameof(BoltPattern.BoltNormal), LevelEnum.Default);
      InsertProperty(dictionary, "Normal", nameof(BoltPattern.Normal), LevelEnum.Default);
      InsertProperty(dictionary, "Y Direction", nameof(BoltPattern.YDirection));

      InsertCustomProperty(dictionary, "Middle Points", nameof(BoltPatternProperties.GetMidPoints), null);

      InsertCustomProperty(dictionary, "Bolt Coordinate System", nameof(BoltPatternProperties.GetBoltCoordinateSystem), null);

      return dictionary;
    }

    private static IEnumerable<Autodesk.DesignScript.Geometry.Point> GetMidPoints(BoltPattern boltPattern)
    {
      boltPattern.GetMidpoints(out var points);
      return points.Select(x => x.ToDynPoint());
    }

    private static DSCoordinateSystem GetBoltCoordinateSystem(BoltPattern boltPattern)
    {
      return DSCoordinateSystem.ByOriginVectors(boltPattern.RefPoint.ToDynPoint(), boltPattern.XDirection.ToDynVector(), boltPattern.YDirection.ToDynVector());
    }

  }
}
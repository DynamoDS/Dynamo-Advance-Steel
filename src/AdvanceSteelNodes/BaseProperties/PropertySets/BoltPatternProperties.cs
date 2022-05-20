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
  public class BoltPatternProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(BoltPattern);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Reference Point", nameof(BoltPattern.RefPoint));
      InsertItem(dictionary, "Number of Screws", nameof(BoltPattern.NumberOfScrews), LevelEnum.Default);
      InsertItem(dictionary, "Is Inverted", nameof(BoltPattern.IsInverted));
      InsertItem(dictionary, "Center", nameof(BoltPattern.Center), LevelEnum.Default);
      InsertItem(dictionary, "X Direction", nameof(BoltPattern.XDirection));
      InsertItem(dictionary, "Bolt Normal", nameof(BoltPattern.BoltNormal), LevelEnum.Default);
      InsertItem(dictionary, "Normal", nameof(BoltPattern.Normal), LevelEnum.Default);
      InsertItem(dictionary, "Y Direction", nameof(BoltPattern.YDirection));

      InsertItem(dictionary, "Middle Points", GetMidPoints);

      InsertItem(dictionary, "Bolt Coordinate System", GetBoltCoordinateSystem);

      return dictionary;
    }

    private object GetMidPoints(object boltPattern)
    {
      ((BoltPattern)boltPattern).GetMidpoints(out var points);
      return points.Select(x => x.ToDynPoint());
    }

    private object GetBoltCoordinateSystem(object boltPattern)
    {
      var bolt = (BoltPattern)boltPattern;
      return DSCoordinateSystem.ByOriginVectors(bolt.RefPoint.ToDynPoint(), bolt.XDirection.ToDynVector(), bolt.YDirection.ToDynVector());
    }

  }
}
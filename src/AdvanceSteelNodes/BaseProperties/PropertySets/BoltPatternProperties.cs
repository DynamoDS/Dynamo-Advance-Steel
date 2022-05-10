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
    public override eObjectType GetObjectType => eObjectType.kBoltPattern;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Reference Point", nameof(BoltPattern.RefPoint));
      InsertItem(dictionary, objectASType, "Number of Screws", nameof(BoltPattern.NumberOfScrews), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Is Inverted", nameof(BoltPattern.IsInverted));
      InsertItem(dictionary, objectASType, "Center", nameof(BoltPattern.Center), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "X Direction", nameof(BoltPattern.XDirection));
      InsertItem(dictionary, objectASType, "Bolt Normal", nameof(BoltPattern.BoltNormal), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Normal", nameof(BoltPattern.Normal), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Y Direction", nameof(BoltPattern.YDirection));

      InsertItem(dictionary, "Middle Points", GetMidPoints);

      InsertItem(dictionary, "Coordinate System", GetBoltCoordinateSystem);

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
using Autodesk.AdvanceSteel.Connection;
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
  public class ConnectionHoleProperties : BaseProperties<ConnectionHoleFeature>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Depth", nameof(ConnectionHoleFeature.Depth), eUnitType.kDistance);
      InsertProperty(dictionary, "Angle", nameof(ConnectionHoleFeature.Angle), eUnitType.kAngle);
      InsertProperty(dictionary, "Use Hole Definition for Numbering", nameof(ConnectionHoleFeature.UsedForNumbering));
      InsertProperty(dictionary, "Hole Center Point", nameof(ConnectionHoleFeature.GetCenterPoint));
      InsertProperty(dictionary, "Exact Coordinate System", nameof(ConnectionHoleFeature.CSExact));
      InsertProperty(dictionary, "Local Coordinate System", nameof(ConnectionHoleFeature.CSLocal));

      InsertCustomProperty(dictionary, "Diameter", nameof(ConnectionHoleProperties.GetDiameter), null);

      return dictionary;
    }

    private double GetDiameter(ConnectionHoleFeature hole)
    {
      return hole.Hole.Diameter;
    }
  }
}
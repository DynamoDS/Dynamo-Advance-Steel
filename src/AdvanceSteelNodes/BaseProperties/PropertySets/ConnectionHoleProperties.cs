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

      InsertItem(dictionary, "Depth", nameof(ConnectionHoleFeature.Depth), eUnitType.kDistance);
      InsertItem(dictionary, "Angle", nameof(ConnectionHoleFeature.Angle), eUnitType.kAngle);
      InsertItem(dictionary, "Use Hole Definition for Numbering", nameof(ConnectionHoleFeature.UsedForNumbering));
      InsertItem(dictionary, "Hole Center Point", nameof(ConnectionHoleFeature.GetCenterPoint));
      InsertItem(dictionary, "Exact Coordinate System", nameof(ConnectionHoleFeature.CSExact));
      InsertItem(dictionary, "Local Coordinate System", nameof(ConnectionHoleFeature.CSLocal));

      InsertItem(dictionary, "Diameter", GetDiameter);

      return dictionary;
    }

    private object GetDiameter(object hole)
    {
      ConnectionHoleFeature asHole = hole as ConnectionHoleFeature;

      return asHole.Hole.Diameter;
    }
  }
}
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
  public class ConnectionHoleProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kConnectionHoleFeature;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Depth", nameof(ConnectionHoleFeature.Depth), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Angle", nameof(ConnectionHoleFeature.Angle), eUnitType.kAngle);
      InsertItem(dictionary, objectASType, "Use Hole Definition for Numbering", nameof(ConnectionHoleFeature.UsedForNumbering));
      InsertItem(dictionary, objectASType, "Center Point", nameof(ConnectionHoleFeature.GetCenterPoint));
      InsertItem(dictionary, objectASType, "Exact Coordinate System", nameof(ConnectionHoleFeature.CSExact));
      InsertItem(dictionary, objectASType, "Local Coordinate System", nameof(ConnectionHoleFeature.CSLocal));

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
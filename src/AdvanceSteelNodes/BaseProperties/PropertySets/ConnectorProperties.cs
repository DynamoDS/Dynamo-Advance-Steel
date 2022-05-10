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
  public class ConnectorProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kConnector;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Head Height", nameof(Connector.HeadHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Head Diameter", nameof(Connector.HeadDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Length", nameof(Connector.Length), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Diameter", nameof(Connector.Diameter), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Grade", nameof(Connector.Grade));
      InsertItem(dictionary, objectASType, "Standard", nameof(Connector.Standard));
      InsertItem(dictionary, objectASType, "Normal", nameof(Connector.Normal), LevelEnum.Default);

      return dictionary;
    }
  }
}
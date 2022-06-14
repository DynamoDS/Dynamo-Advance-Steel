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
  public class ConnectorProperties : BaseProperties<Connector>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Head Height", nameof(Connector.HeadHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Head Diameter", nameof(Connector.HeadDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Length", nameof(Connector.Length), eUnitType.kDistance);
      InsertProperty(dictionary, "Diameter", nameof(Connector.Diameter), eUnitType.kDistance);
      InsertProperty(dictionary, "Grade", nameof(Connector.Grade));
      InsertProperty(dictionary, "Standard", nameof(Connector.Standard));
      InsertProperty(dictionary, "Normal", nameof(Connector.Normal), LevelEnum.Default);
      InsertProperty(dictionary, "Weight", nameof(Connector.GetWeight), eUnitType.kWeight);

      return dictionary;
    }
  }
}
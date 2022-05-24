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

      InsertItem(dictionary, "Head Height", nameof(Connector.HeadHeight), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Head Diameter", nameof(Connector.HeadDiameter), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Length", nameof(Connector.Length), eUnitType.kDistance);
      InsertItem(dictionary, "Diameter", nameof(Connector.Diameter), eUnitType.kDistance);
      InsertItem(dictionary, "Grade", nameof(Connector.Grade));
      InsertItem(dictionary, "Standard", nameof(Connector.Standard));
      InsertItem(dictionary, "Normal", nameof(Connector.Normal), LevelEnum.Default);

      return dictionary;
    }
  }
}
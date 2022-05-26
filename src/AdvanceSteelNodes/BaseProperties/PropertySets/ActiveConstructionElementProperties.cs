using Autodesk.AdvanceSteel.ConstructionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;

namespace AdvanceSteel.Nodes
{
  public class ActiveConstructionElementProperties : BaseProperties<ActiveConstructionElement>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Coordinate System", nameof(ActiveConstructionElement.CS), LevelEnum.Default);
      InsertProperty(dictionary, "Connection Number", nameof(ActiveConstructionElement.NumberOfDrivenConObj));

      return dictionary;
    }
  }
}

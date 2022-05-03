using Autodesk.AdvanceSteel.ConstructionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;

namespace AdvanceSteel.Nodes
{
  public class ActiveConstructionElementProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kActConstructionElem;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Coordinate System", nameof(ActiveConstructionElement.CS), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Connection Number", nameof(ActiveConstructionElement.NumberOfDrivenConObj));

      return dictionary;
    }
  }
}

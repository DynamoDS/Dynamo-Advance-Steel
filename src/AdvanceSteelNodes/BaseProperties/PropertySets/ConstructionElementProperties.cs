using Autodesk.AdvanceSteel.ConstructionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;

namespace AdvanceSteel.Nodes
{
  public class ConstructionElementProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kConstructionElem;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Role Description", nameof(ConstructionElement.RoleDescription), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Pure Role", nameof(ConstructionElement.PureRole), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Center Point", nameof(ConstructionElement.CenterPoint), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Role", nameof(ConstructionElement.Role));
      InsertItem(dictionary, objectASType, "Display Modes Number", nameof(ConstructionElement.NumberOfReprModes), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Display Mode", nameof(ConstructionElement.ReprMode));

      return dictionary;
    }
  }
}

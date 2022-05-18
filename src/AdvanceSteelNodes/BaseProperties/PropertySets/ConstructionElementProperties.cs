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
    public override Type GetObjectType => typeof(ConstructionElement);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Role Description", nameof(ConstructionElement.RoleDescription), LevelEnum.Default);
      InsertItem(dictionary, "Pure Role", nameof(ConstructionElement.PureRole), LevelEnum.Default);
      InsertItem(dictionary, "Center Point", nameof(ConstructionElement.CenterPoint), LevelEnum.Default);
      InsertItem(dictionary, "Role", nameof(ConstructionElement.Role));
      InsertItem(dictionary, "Display Modes Number", nameof(ConstructionElement.NumberOfReprModes), LevelEnum.Default);
      InsertItem(dictionary, "Display Mode", nameof(ConstructionElement.ReprMode));

      return dictionary;
    }
  }
}

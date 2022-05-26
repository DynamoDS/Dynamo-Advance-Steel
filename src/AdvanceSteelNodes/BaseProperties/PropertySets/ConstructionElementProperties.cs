using Autodesk.AdvanceSteel.ConstructionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;

namespace AdvanceSteel.Nodes
{
  public class ConstructionElementProperties : BaseProperties<ConstructionElement>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Role Description", nameof(ConstructionElement.RoleDescription), LevelEnum.Default);
      InsertProperty(dictionary, "Pure Role", nameof(ConstructionElement.PureRole), LevelEnum.Default);
      InsertProperty(dictionary, "Center Point", nameof(ConstructionElement.CenterPoint), LevelEnum.Default);
      InsertProperty(dictionary, "Role", nameof(ConstructionElement.Role));
      InsertProperty(dictionary, "Display Modes Number", nameof(ConstructionElement.NumberOfReprModes), LevelEnum.Default);
      InsertProperty(dictionary, "Display Mode", nameof(ConstructionElement.ReprMode));

      return dictionary;
    }
  }
}

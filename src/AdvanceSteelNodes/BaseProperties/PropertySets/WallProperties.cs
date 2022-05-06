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
  public class WallProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kWall;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Height", nameof(Wall.Height), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Free", nameof(Wall.IsFree));

      return dictionary;
    }
  }
}

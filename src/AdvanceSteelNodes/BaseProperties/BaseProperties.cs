using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AdvanceSteel.CADAccess;
using static Autodesk.AdvanceSteel.DotNetRoots.Units.Unit;

namespace AdvanceSteel.Nodes
{
  public abstract class BaseProperties : IASProperties
  {
    public abstract FilerObject.eObjectType GetObjectType { get; }

    public abstract Dictionary<string, Property> BuildPropertyList(Type objectASType);

    protected void InsertItem(Dictionary<string, Property> dictionary, Type objectASType, string description, string memberName, LevelEnum level = LevelEnum.NoDefinition, eUnitType? unitType = null)
    {
      dictionary.Add(description, new Property(objectASType, GetObjectType, description, memberName, level, unitType));
    }

    protected void InsertItem(Dictionary<string, Property> dictionary, Type objectASType, string description, string memberName, eUnitType unitType)
    {
      InsertItem(dictionary, objectASType, description, memberName, LevelEnum.NoDefinition, unitType);
    }

    protected void InsertItem(Dictionary<string, Property> dictionary, string description, Func<object, object> funcGetValue, LevelEnum level = LevelEnum.NoDefinition, eUnitType? unitType = null)
    {
      dictionary.Add(description, new Property(GetObjectType, description, funcGetValue, level, unitType));
    }

    protected void InsertItem(Dictionary<string, Property> dictionary, string description, Func<object, object> funcGetValue, eUnitType? unitType)
    {
      InsertItem(dictionary, description, funcGetValue, LevelEnum.NoDefinition, unitType);
    }
  }
}

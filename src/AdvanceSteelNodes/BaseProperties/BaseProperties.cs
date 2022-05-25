using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AdvanceSteel.CADAccess;
using static Autodesk.AdvanceSteel.DotNetRoots.Units.Unit;

namespace AdvanceSteel.Nodes
{
  public abstract class BaseProperties<T> : IASProperties where T:class
  {
    public Type GetObjectType { get => typeof(T); }

    public abstract Dictionary<string, Property> BuildPropertyList();

    /// <summary>
    /// Insert item at properties dictionary
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="description"></param>
    /// <param name="memberName">Member name of AS Object - May be Get/Set property or Get Method(without parameter)</param>
    /// <param name="level"></param>
    /// <param name="unitType"></param>
    protected void InsertItem(Dictionary<string, Property> dictionary, string description, string memberName, LevelEnum level = LevelEnum.NoDefinition, eUnitType? unitType = null)
    {
      dictionary.Add(description, new Property(GetObjectType, description, memberName, level, unitType));
    }

    /// <summary>
    /// Insert item at properties dictionary
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="description"></param>
    /// <param name="memberName">Member name of AS Object - May be Get/Set property or Get Method(without parameter)</param>
    /// <param name="unitType"></param>
    protected void InsertItem(Dictionary<string, Property> dictionary, string description, string memberName, eUnitType unitType)
    {
      InsertItem(dictionary, description, memberName, LevelEnum.NoDefinition, unitType);
    }

    /// <summary>
    /// Insert item at properties dictionary
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="description"></param>
    /// <param name="methodInfoGet">Method name of Get Custom Function on properties class</param>
    /// <param name="methodInfoSet">Method name of Set Custom Function on properties class</param>
    /// <param name="level"></param>
    /// <param name="unitType"></param>
    protected void InsertItem(Dictionary<string, Property> dictionary, string description, string methodInfoGet, string methodInfoSet, LevelEnum level = LevelEnum.NoDefinition, eUnitType? unitType = null)
    {
      PropertyMethods propertyMethods = new PropertyMethods(this.GetType(), methodInfoGet, methodInfoSet);
      dictionary.Add(description, new Property(GetObjectType, description, propertyMethods, level, unitType));
    }

    /// <summary>
    /// Insert item at properties dictionary
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="description"></param>
    /// <param name="methodInfoGet">Method name of Get Custom Function on properties class</param>
    /// <param name="methodInfoSet">Method name of Set Custom Function on properties class</param>
    /// <param name="unitType"></param>
    protected void InsertItem(Dictionary<string, Property> dictionary, string description, string methodInfoGet, string methodInfoSet, eUnitType? unitType)
    {
      InsertItem(dictionary, description, methodInfoGet, methodInfoSet, LevelEnum.NoDefinition, unitType);
    }

  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;

namespace AdvanceSteel.Nodes
{
  public class SteelTypeData
  {
    internal Type ASType { get; set; }
    internal string Description { get; private set; }

    internal Dictionary<string, Property> PropertiesSpecific { get; private set; }

    internal Dictionary<string, Property> PropertiesAll { get; private set; }

    internal SteelTypeData(string description)
    {
      Description = description;

      PropertiesAll = new Dictionary<string, Property>();
    }

    internal void SetPropertiesSpecific(Dictionary<string, Property> properties)
    {
      PropertiesSpecific = properties;
    }
    internal void AddPropertiesAll(Dictionary<string, Property> properties)
    {
      foreach (var item in properties)
      {
        if (PropertiesAll.ContainsKey(item.Key))
        {
          throw new Exception(string.Format("Property '{0}' already added", item.Key));
        }

        PropertiesAll.Add(item.Key, new Property(item.Value));
      }
    }

    internal void OrderDictionaryPropertiesAll()
    {
      PropertiesAll = (from entry in PropertiesAll orderby entry.Key ascending select entry).ToDictionary(x => x.Key, y => y.Value);
    }
  }
}

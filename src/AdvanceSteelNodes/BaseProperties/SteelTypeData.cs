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
    public Type ASType { get; private set; }
    public string Description { get; private set; }

    public Dictionary<string, Property> Properties { get; private set; }

    public SteelTypeData(Type asType, string description)
    {
      ASType = asType;
      Description = description;
    }

    public bool IsFromThisType(Type type)
    {
      return this.ASType.IsSubclassOf(type) || this.ASType.IsEquivalentTo(type);
    }

    public void SetProperties(Dictionary<string, Property> properties)
    {
      Properties = properties;
    }
  }
}

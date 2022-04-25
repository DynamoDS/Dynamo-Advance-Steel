using Autodesk.AdvanceSteel.CADAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;

namespace AdvanceSteel.Nodes
{
  public class FilerObjectProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kFilerObject;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Layer", nameof(FilerObject.Layer));
      InsertItem(dictionary, objectASType, "Handle", nameof(FilerObject.Handle), LevelEnum.Default);
      InsertItem(dictionary, "Type", GetFormatedType);

      return dictionary;
    }

    private object GetFormatedType(object filerObject)
    {
      return UtilsProperties.SteelObjectPropertySets[((FilerObject)filerObject).Type()].Description;
    }
  }
}

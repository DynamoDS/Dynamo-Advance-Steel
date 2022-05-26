using Autodesk.AdvanceSteel.CADAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;

namespace AdvanceSteel.Nodes
{
  public class FilerObjectProperties : BaseProperties<FilerObject>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Layer", nameof(FilerObject.Layer));
      InsertProperty(dictionary, "Handle", nameof(FilerObject.Handle), LevelEnum.Default);
      InsertCustomProperty(dictionary, "Object Type", nameof(FilerObjectProperties.GetFormatedType), null);

      return dictionary;
    }

    private static string GetFormatedType(object filerObject)
    {
      return Utils.GetDescriptionObject(filerObject.GetType());
    }
  }
}

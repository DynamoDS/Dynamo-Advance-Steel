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

      InsertItem(dictionary, "Layer", nameof(FilerObject.Layer));
      InsertItem(dictionary, "Handle", nameof(FilerObject.Handle), LevelEnum.Default);
      InsertItem(dictionary, "Object Type", GetFormatedType);

      return dictionary;
    }

    private object GetFormatedType(object filerObject)
    {
      return Utils.GetDescriptionObject(filerObject.GetType());
    }
  }
}

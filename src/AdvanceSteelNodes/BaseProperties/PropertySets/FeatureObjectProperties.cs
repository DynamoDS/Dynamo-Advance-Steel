using Autodesk.AdvanceSteel.ConstructionTypes;
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
  public class FeatureObjectProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kFeatureObject;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Is From Fitter", nameof(FeatureObject.IsFromFitter), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Coordinate System", nameof(FeatureObject.CS));
      InsertItem(dictionary, objectASType, "Use Gap", nameof(FeatureObject.UseGap));
      InsertItem(dictionary, objectASType, "Object Index", nameof(FeatureObject.ObjectIndex));

      return dictionary;
    }
  }
}
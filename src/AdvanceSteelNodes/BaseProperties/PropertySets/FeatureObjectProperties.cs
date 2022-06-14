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
  public class FeatureObjectProperties : BaseProperties<FeatureObject>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Is From Fitter", nameof(FeatureObject.IsFromFitter), LevelEnum.Default);
      InsertProperty(dictionary, "Coordinate System", nameof(FeatureObject.CS));
      InsertProperty(dictionary, "Use Gap", nameof(FeatureObject.UseGap));
      InsertProperty(dictionary, "Object Index", nameof(FeatureObject.ObjectIndex));

      return dictionary;
    }
  }
}
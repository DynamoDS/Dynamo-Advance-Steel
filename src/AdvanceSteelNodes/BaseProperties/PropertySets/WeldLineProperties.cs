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
  public class WeldLineProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(WeldLine);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Points Lower", GetWeldPointsLower);
      InsertItem(dictionary, "Points Upper", GetWeldPointsUpper);

      return dictionary;
    }
    private object GetWeldPointsLower(object weldLine)
    {
      ((WeldLine)weldLine).GetWeldPoints(out var points, WeldPattern.eSeamPosition.kLower);
      return points.Select(x => x.ToDynPoint());
    }

    private object GetWeldPointsUpper(object weldLine)
    {
      ((WeldLine)weldLine).GetWeldPoints(out var points, WeldPattern.eSeamPosition.kUpper);
      return points.Select(x => x.ToDynPoint());
    }

  }
}

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
  public class WeldLineProperties : BaseProperties<WeldLine>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertCustomProperty(dictionary, "Points Lower", nameof(WeldLineProperties.GetWeldPointsLower), null);
      InsertCustomProperty(dictionary, "Points Upper", nameof(WeldLineProperties.GetWeldPointsUpper), null);

      return dictionary;
    }
    private IEnumerable<Autodesk.DesignScript.Geometry.Point> GetWeldPointsLower(WeldLine weldLine)
    {
      weldLine.GetWeldPoints(out var points, WeldPattern.eSeamPosition.kLower);
      return points.Select(x => x.ToDynPoint());
    }

    private IEnumerable<Autodesk.DesignScript.Geometry.Point> GetWeldPointsUpper(WeldLine weldLine)
    {
      weldLine.GetWeldPoints(out var points, WeldPattern.eSeamPosition.kUpper);
      return points.Select(x => x.ToDynPoint());
    }

  }
}

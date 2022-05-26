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
  public class PlateFeatEdgeProperties : BaseProperties<PlateFeatEdge>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Polygon", nameof(PlateFeatEdge.ContextPolygon), eUnitType.kDistance);
      InsertProperty(dictionary, "Edge Index", nameof(PlateFeatEdge.EdgeIndex));
      InsertProperty(dictionary, "Contour Index", nameof(PlateFeatEdge.ContourIndex));

      return dictionary;
    }

  }
}

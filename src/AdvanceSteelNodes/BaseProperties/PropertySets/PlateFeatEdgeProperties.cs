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
  public class PlateFeatEdgeProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(PlateFeatEdge);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Polygon", nameof(PlateFeatEdge.ContextPolygon), eUnitType.kDistance);
      InsertItem(dictionary, "Edge Index", nameof(PlateFeatEdge.EdgeIndex));
      InsertItem(dictionary, "Contour Index", nameof(PlateFeatEdge.ContourIndex));

      return dictionary;
    }

  }
}

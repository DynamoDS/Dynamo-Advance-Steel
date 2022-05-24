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
  public class PlateFeatVertexProperties : BaseProperties<PlateFeatVertex>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Vertex Index", nameof(PlateFeatVertex.VertexIndex));
      InsertItem(dictionary, "Contour Index", nameof(PlateFeatVertex.ContourIndex));

      InsertItem(dictionary, "Polygon", GetPolygon);

      return dictionary;
    }

    private object GetPolygon(object plateFeatVertex)
    {
      PlateFeatVertex asPlateFeatVertex = plateFeatVertex as PlateFeatVertex;
      asPlateFeatVertex.GetPolygon(out var polygon);

      return polygon.Vertices.Select(x => x.ToDynPoint());
    }
  }
}
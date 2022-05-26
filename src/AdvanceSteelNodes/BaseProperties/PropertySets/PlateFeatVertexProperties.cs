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

      InsertProperty(dictionary, "Vertex Index", nameof(PlateFeatVertex.VertexIndex));
      InsertProperty(dictionary, "Contour Index", nameof(PlateFeatVertex.ContourIndex));

      InsertCustomProperty(dictionary, "Polygon", nameof(PlateFeatVertexProperties.GetPolygon), null);

      return dictionary;
    }

    private IEnumerable<Autodesk.DesignScript.Geometry.Point> GetPolygon(PlateFeatVertex plateFeatVertex)
    {
      plateFeatVertex.GetPolygon(out var polygon);

      return polygon.Vertices.Select(x => x.ToDynPoint());
    }
  }
}
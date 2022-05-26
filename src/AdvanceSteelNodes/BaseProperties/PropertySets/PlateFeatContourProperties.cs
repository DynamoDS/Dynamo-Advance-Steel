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
  public class PlateFeatContourProperties : BaseProperties<PlateFeatContour>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Gap", nameof(PlateFeatContour.Gap), eUnitType.kDistance);
      InsertProperty(dictionary, "Length", nameof(PlateFeatContour.Length), eUnitType.kDistance);
      InsertProperty(dictionary, "Length Increment", nameof(PlateFeatContour.LengthIncrement), eUnitType.kDistance);
      InsertProperty(dictionary, "Radius", nameof(PlateFeatContour.Radius), eUnitType.kDistance);
      InsertProperty(dictionary, "Radius Increment", nameof(PlateFeatContour.RadIncrement), eUnitType.kDistance);
      InsertProperty(dictionary, "Width", nameof(PlateFeatContour.Width), eUnitType.kDistance);
      InsertProperty(dictionary, "Boring Out Option", nameof(PlateFeatContour.BoringOut));
      InsertProperty(dictionary, "Offset", nameof(PlateFeatContour.Offset));
      InsertProperty(dictionary, "Vertex", nameof(PlateFeatContour.VertexInformation));

      InsertCustomProperty(dictionary, "Contour Type", nameof(PlateFeatContourProperties.GetContourType), null);
      InsertCustomProperty(dictionary, "Contour Polygon", nameof(PlateFeatContourProperties.GetContourPolygon), null);

      return dictionary;
    }

    private string GetContourType(PlateFeatContour plateFeatContour)
    {
      return plateFeatContour.ContourType.ToString();
    }

    private IEnumerable<Autodesk.DesignScript.Geometry.Point> GetContourPolygon(PlateFeatContour plateFeatContour)
    {
      return plateFeatContour.GetContourPolygon(0).Select(x => x.ToDynPoint());
    }
  }
}
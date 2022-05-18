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
  public class PlateFeatContourProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(PlateFeatContour);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Gap", nameof(PlateFeatContour.Gap), eUnitType.kDistance);
      InsertItem(dictionary, "Length", nameof(PlateFeatContour.Length), eUnitType.kDistance);
      InsertItem(dictionary, "Length Increment", nameof(PlateFeatContour.LengthIncrement), eUnitType.kDistance);
      InsertItem(dictionary, "Radius", nameof(PlateFeatContour.Radius), eUnitType.kDistance);
      InsertItem(dictionary, "Radius Increment", nameof(PlateFeatContour.RadIncrement), eUnitType.kDistance);
      InsertItem(dictionary, "Width", nameof(PlateFeatContour.Width), eUnitType.kDistance);
      InsertItem(dictionary, "Boring Out Option", nameof(PlateFeatContour.BoringOut));
      InsertItem(dictionary, "Offset", nameof(PlateFeatContour.Offset));
      InsertItem(dictionary, "Vertex", nameof(PlateFeatContour.VertexInformation));

      InsertItem(dictionary, "Contour Type", GetContourType);
      InsertItem(dictionary, "Contour Polygon", GetContourPolygon);

      return dictionary;
    }

    private object GetContourType(object plateFeatContour)
    {
      PlateFeatContour asPlateFeatContour = plateFeatContour as PlateFeatContour;

      return asPlateFeatContour.ContourType.ToString();
    }

    private object GetContourPolygon(object plateFeatContour)
    {
      PlateFeatContour asPlateFeatContour = plateFeatContour as PlateFeatContour;

      return asPlateFeatContour.GetContourPolygon(0).Select(x => x.ToDynPoint());
    }
  }
}
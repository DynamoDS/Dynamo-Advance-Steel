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
    public override eObjectType GetObjectType => eObjectType.kPlateFeatContour;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Gap", nameof(PlateFeatContour.Gap), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Length", nameof(PlateFeatContour.Length), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Length Increment", nameof(PlateFeatContour.LengthIncrement), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Radius", nameof(PlateFeatContour.Radius), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Radius Increment", nameof(PlateFeatContour.RadIncrement), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Width", nameof(PlateFeatContour.Width), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Boring Out Option", nameof(PlateFeatContour.BoringOut));
      InsertItem(dictionary, objectASType, "Offset", nameof(PlateFeatContour.Offset));
      InsertItem(dictionary, objectASType, "Vertex", nameof(PlateFeatContour.VertexInformation));

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
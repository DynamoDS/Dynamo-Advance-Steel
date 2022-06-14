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
  public class PlateFeatVertFilletProperties : BaseProperties<PlateFeatVertFillet>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Length 1", nameof(PlateFeatVertFillet.Length1), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Length 2", nameof(PlateFeatVertFillet.Length2), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Radius Increment", nameof(PlateFeatVertFillet.RadiusIncrement), eUnitType.kDistance);
      InsertProperty(dictionary, "Radius", nameof(PlateFeatVertFillet.Radius), LevelEnum.Default, eUnitType.kDistance);

      InsertCustomProperty(dictionary, "Fillet Type", nameof(PlateFeatVertFilletProperties.GetFilletType), null);
      InsertCustomProperty(dictionary, "Contour Polygon", nameof(PlateFeatVertFilletProperties.GetContourPolygon), null);

      return dictionary;
    }

    private static string GetFilletType(PlateFeatVertFillet plateFeatVertFillet)
    {
      return plateFeatVertFillet.FilletType.ToString();
    }

    private static IEnumerable<Autodesk.DesignScript.Geometry.Point> GetContourPolygon(PlateFeatVertFillet plateFeatVertFillet)
    {
      return plateFeatVertFillet.GetBaseContourPolygon(0).Select(x => x.ToDynPoint());
    }
  }
}
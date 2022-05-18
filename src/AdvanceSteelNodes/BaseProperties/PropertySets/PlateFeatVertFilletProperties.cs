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
  public class PlateFeatVertFilletProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(PlateFeatVertFillet);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Length 1", nameof(PlateFeatVertFillet.Length1), eUnitType.kDistance);
      InsertItem(dictionary, "Length 2", nameof(PlateFeatVertFillet.Length2), eUnitType.kDistance);
      InsertItem(dictionary, "Radius Increment", nameof(PlateFeatVertFillet.RadiusIncrement), eUnitType.kDistance);
      InsertItem(dictionary, "Radius", nameof(PlateFeatVertFillet.Radius), eUnitType.kDistance);

      InsertItem(dictionary, "Fillet Type", GetFilletType);
      InsertItem(dictionary, "Contour Polygon", GetContourPolygon);

      return dictionary;
    }

    private object GetFilletType(object plateFeatVertFillet)
    {
      PlateFeatVertFillet asPlateFeatVertFillet = plateFeatVertFillet as PlateFeatVertFillet;

      return asPlateFeatVertFillet.FilletType.ToString();
    }

    private object GetContourPolygon(object plateFeatVertFillet)
    {
      PlateFeatVertFillet asPlateFeatVertFillet = plateFeatVertFillet as PlateFeatVertFillet;

      return asPlateFeatVertFillet.GetBaseContourPolygon(0).Select(x => x.ToDynPoint());
    }
  }
}
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
    public override eObjectType GetObjectType => eObjectType.kPlateFeatVertFillet;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Length 1", nameof(PlateFeatVertFillet.Length1), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Length 2", nameof(PlateFeatVertFillet.Length2), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Radius Increment", nameof(PlateFeatVertFillet.RadiusIncrement), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Radius", nameof(PlateFeatVertFillet.Radius), eUnitType.kDistance);

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
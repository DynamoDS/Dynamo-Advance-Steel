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
  public class SpecialPartProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kSpecialPart;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Block Name", nameof(SpecialPart.BlockName), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Depth", nameof(SpecialPart.Depth), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Width", nameof(SpecialPart.Width), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Length", nameof(SpecialPart.Length), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Paint Area", nameof(SpecialPart.PaintArea), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Scale", nameof(SpecialPart.Scale), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Weight", nameof(SpecialPart.Weight), eUnitType.kDistance);

      return dictionary;
    }
  }
}
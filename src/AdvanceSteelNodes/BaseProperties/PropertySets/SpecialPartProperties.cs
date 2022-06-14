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
  public class SpecialPartProperties : BaseProperties<SpecialPart>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Block Name", nameof(SpecialPart.BlockName), LevelEnum.Default);
      InsertProperty(dictionary, "Depth", nameof(SpecialPart.Depth), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Width", nameof(SpecialPart.Width), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Length", nameof(SpecialPart.Length), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Paint Area", nameof(SpecialPart.PaintArea), eUnitType.kDistance);
      InsertProperty(dictionary, "Scale", nameof(SpecialPart.Scale), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Weight", nameof(SpecialPart.Weight), eUnitType.kDistance);

      return dictionary;
    }
  }
}
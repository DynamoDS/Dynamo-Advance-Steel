using Autodesk.AdvanceSteel.Arrangement;
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
  public class CountableScrewBoltPatternProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kCountableScrewBoltPattern;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();
      
      InsertItem(dictionary, objectASType, "Height", nameof(CountableScrewBoltPattern.Height), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Length", nameof(CountableScrewBoltPattern.Length), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Holes spacing in X Direction", nameof(CountableScrewBoltPattern.Dx), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Holes spacing in Y Direction", nameof(CountableScrewBoltPattern.Dy), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Holes in Y Direction", nameof(CountableScrewBoltPattern.Ny));
      InsertItem(dictionary, objectASType, "Holes in X Direction", nameof(CountableScrewBoltPattern.Nx));

      return dictionary;
    }
  }
}
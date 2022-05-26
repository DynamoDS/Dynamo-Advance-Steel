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
  public class CountableScrewBoltPatternProperties : BaseProperties<CountableScrewBoltPattern>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();
      
      InsertProperty(dictionary, "Height", nameof(CountableScrewBoltPattern.Height), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Length", nameof(CountableScrewBoltPattern.Length), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Spacing of holes in X Direction", nameof(CountableScrewBoltPattern.Dx), eUnitType.kDistance);
      InsertProperty(dictionary, "Spacing of holes in Y Direction", nameof(CountableScrewBoltPattern.Dy), eUnitType.kDistance);
      InsertProperty(dictionary, "Holes in Y Direction", nameof(CountableScrewBoltPattern.Ny));
      InsertProperty(dictionary, "Holes in X Direction", nameof(CountableScrewBoltPattern.Nx));

      return dictionary;
    }
  }
}
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
    public override Type GetObjectType => typeof(CountableScrewBoltPattern);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();
      
      InsertItem(dictionary, "Height", nameof(CountableScrewBoltPattern.Height), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Length", nameof(CountableScrewBoltPattern.Length), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Holes spacing in X Direction", nameof(CountableScrewBoltPattern.Dx), eUnitType.kDistance);
      InsertItem(dictionary, "Holes spacing in Y Direction", nameof(CountableScrewBoltPattern.Dy), eUnitType.kDistance);
      InsertItem(dictionary, "Holes in Y Direction", nameof(CountableScrewBoltPattern.Ny));
      InsertItem(dictionary, "Holes in X Direction", nameof(CountableScrewBoltPattern.Nx));

      return dictionary;
    }
  }
}
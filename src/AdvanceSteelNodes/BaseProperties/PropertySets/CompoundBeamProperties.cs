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
  public class CompoundBeamProperties : BaseProperties<CompoundStraightBeam>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "CompoundTypeName", nameof(CompoundStraightBeam.CompoundTypeName), LevelEnum.Default);
      InsertItem(dictionary, "CompoundClassName", nameof(CompoundStraightBeam.CompoundClassName), LevelEnum.Default);
      InsertItem(dictionary, "Use Compound As One Beam", nameof(CompoundStraightBeam.UseCompoundAsOneBeam));

      return dictionary;
    }
  }
}

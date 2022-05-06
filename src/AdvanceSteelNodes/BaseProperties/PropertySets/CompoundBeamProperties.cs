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
  public class CompoundBeamProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kCompoundBeam;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "CompoundTypeName", nameof(CompoundStraightBeam.CompoundTypeName), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "CompoundClassName", nameof(CompoundStraightBeam.CompoundClassName), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Use Compound As One Beam", nameof(CompoundStraightBeam.UseCompoundAsOneBeam));

      return dictionary;
    }
  }
}

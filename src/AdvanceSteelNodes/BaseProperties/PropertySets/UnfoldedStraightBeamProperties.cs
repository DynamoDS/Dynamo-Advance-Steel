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
  public class UnfoldedStraightBeamProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kUnfoldedStraightBeam;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Unfolded Bem Thickness", nameof(UnfoldedStraightBeam.Thickness), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Unfolded Bem Portioning", nameof(UnfoldedStraightBeam.Portioning), eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Closed", nameof(UnfoldedStraightBeam.IsClosed));

      return dictionary;
    }
  }
}
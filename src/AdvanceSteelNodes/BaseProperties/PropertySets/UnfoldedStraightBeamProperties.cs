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
  public class UnfoldedStraightBeamProperties : BaseProperties<UnfoldedStraightBeam>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Unfolded Bem Thickness", nameof(UnfoldedStraightBeam.Thickness), eUnitType.kDistance);
      InsertProperty(dictionary, "Unfolded Bem Portioning", nameof(UnfoldedStraightBeam.Portioning), eUnitType.kDistance);
      InsertProperty(dictionary, "Closed", nameof(UnfoldedStraightBeam.IsClosed));

      return dictionary;
    }
  }
}
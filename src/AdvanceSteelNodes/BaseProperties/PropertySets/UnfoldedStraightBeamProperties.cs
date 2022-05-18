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
    public override Type GetObjectType => typeof(UnfoldedStraightBeam);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Unfolded Bem Thickness", nameof(UnfoldedStraightBeam.Thickness), eUnitType.kDistance);
      InsertItem(dictionary, "Unfolded Bem Portioning", nameof(UnfoldedStraightBeam.Portioning), eUnitType.kDistance);
      InsertItem(dictionary, "Closed", nameof(UnfoldedStraightBeam.IsClosed));

      return dictionary;
    }
  }
}
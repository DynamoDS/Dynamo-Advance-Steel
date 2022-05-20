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
  public class FinitRectScrewBoltPatternProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(FinitRectScrewBoltPattern);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Midpoint On Upper Left", nameof(FinitRectScrewBoltPattern.MidpointOnUpperLeft));
      InsertItem(dictionary, "Midpoint On Lower Left", nameof(FinitRectScrewBoltPattern.MidpointOnLowerRight));
      InsertItem(dictionary, "Size Y Direction", nameof(FinitRectScrewBoltPattern.Wy), eUnitType.kDistance);
      InsertItem(dictionary, "Size X Direction", nameof(FinitRectScrewBoltPattern.Wx), eUnitType.kDistance);

      return dictionary;
    }
  }
}

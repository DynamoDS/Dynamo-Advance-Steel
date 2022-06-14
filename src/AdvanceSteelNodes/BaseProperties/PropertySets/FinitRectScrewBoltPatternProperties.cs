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
  public class FinitRectScrewBoltPatternProperties : BaseProperties<FinitRectScrewBoltPattern>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Midpoint On Upper Left", nameof(FinitRectScrewBoltPattern.MidpointOnUpperLeft));
      InsertProperty(dictionary, "Midpoint On Lower Left", nameof(FinitRectScrewBoltPattern.MidpointOnLowerRight));
      InsertProperty(dictionary, "Size Y Direction", nameof(FinitRectScrewBoltPattern.Wy), eUnitType.kDistance);
      InsertProperty(dictionary, "Size X Direction", nameof(FinitRectScrewBoltPattern.Wx), eUnitType.kDistance);

      return dictionary;
    }
  }
}

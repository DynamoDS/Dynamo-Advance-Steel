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
  public class InfinitRectScrewBoltPatternProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(InfinitRectScrewBoltPattern);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Midpoint On Lower Right", nameof(InfinitRectScrewBoltPattern.MidpointOnUpperRight));
      InsertItem(dictionary, "Midpoint On Lower Left", nameof(InfinitRectScrewBoltPattern.MidpointOnLowerLeft));
      InsertItem(dictionary, "Size Y Direction", nameof(InfinitRectScrewBoltPattern.Wy), eUnitType.kDistance);
      InsertItem(dictionary, "Size X Direction", nameof(InfinitRectScrewBoltPattern.Wx), eUnitType.kDistance);

      return dictionary;
    }
  }
}
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
  public class InfinitMidScrewBoltPatternProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(InfinitMidScrewBoltPattern);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Midpoint On Lower Left", nameof(InfinitMidScrewBoltPattern.MidpointOnLowerLeft), LevelEnum.Default);
      InsertItem(dictionary, "Midpoint On Lower Right", nameof(InfinitMidScrewBoltPattern.MidpointOnLowerRight), LevelEnum.Default);
      InsertItem(dictionary, "Midpoint On Upper Left", nameof(InfinitMidScrewBoltPattern.MidpointOnUpperLeft), LevelEnum.Default);

      return dictionary;
    }
  }
}
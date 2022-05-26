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
  public class CircleScrewBoltPatternProperties : BaseProperties<CircleScrewBoltPattern>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Bolts Number", nameof(CircleScrewBoltPattern.NumberOfScrews));
      InsertProperty(dictionary, "Radius", nameof(CircleScrewBoltPattern.Radius), eUnitType.kDistance);

      return dictionary;
    }
  }
}
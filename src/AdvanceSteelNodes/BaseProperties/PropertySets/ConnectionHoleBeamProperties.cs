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
  public class ConnectionHoleBeamProperties : BaseProperties<ConnectionHoleBeam>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertCustomProperty(dictionary, "Beam", nameof(ConnectionHoleBeamProperties.GetBeam), null);
      InsertCustomProperty(dictionary, "End", nameof(ConnectionHoleBeamProperties.GetEnd), null);

      return dictionary;
    }

    private static SteelDbObject GetBeam(ConnectionHoleBeam connectionHoleBeam)
    {
      return connectionHoleBeam.GetBeam().ToDSType();
    }

    private static string GetEnd(ConnectionHoleBeam connectionHoleBeam)
    {
      return connectionHoleBeam.GetEnd().ToString();
    }
  }
}
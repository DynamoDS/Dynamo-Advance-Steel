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
  public class ConnectionHoleBeamProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(ConnectionHoleBeam);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Beam", GetBeam);
      InsertItem(dictionary, "End", GetEnd);

      return dictionary;
    }

    private object GetBeam(object connectionHoleBeam)
    {
      ConnectionHoleBeam asConnectionHoleBeam = connectionHoleBeam as ConnectionHoleBeam;

      return asConnectionHoleBeam.GetBeam().ToDSType();
    }

    private object GetEnd(object connectionHoleBeam)
    {
      ConnectionHoleBeam asConnectionHoleBeam = connectionHoleBeam as ConnectionHoleBeam;

      return asConnectionHoleBeam.GetEnd().ToString();
    }
  }
}
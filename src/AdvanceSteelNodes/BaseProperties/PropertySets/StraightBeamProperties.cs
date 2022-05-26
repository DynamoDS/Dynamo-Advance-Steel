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
  public class StraightBeamProperties : BaseProperties<StraightBeam>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Has Valid Saw Cut", nameof(StraightBeam.HasValidSawCut));

      InsertCustomProperty(dictionary, "Camber Height", nameof(StraightBeamProperties.GetCamberHeight), null);
      InsertCustomProperty(dictionary, "Camber Position", nameof(StraightBeamProperties.GetCamberPosition), null);
      InsertCustomProperty(dictionary, "Camber Direction YZ", nameof(StraightBeamProperties.GetCamberDirectionYZStatus), null);
      InsertCustomProperty(dictionary, "Camber Start Offset", nameof(StraightBeamProperties.GetCamberStartOffset), null);
      InsertCustomProperty(dictionary, "Camber End Offset", nameof(StraightBeamProperties.GetCamberEndOffset), null);

      return dictionary;
    }

    private double GetCamberHeight(StraightBeam straightBeam)
    {
      return straightBeam.Camber.Height;
    }

    private double GetCamberPosition(StraightBeam straightBeam)
    {
      return straightBeam.Camber.Position;
    }

    private bool GetCamberDirectionYZStatus(StraightBeam straightBeam)
    {
      return straightBeam.Camber.DirectionYZStatus;
    }

    private double GetCamberStartOffset(StraightBeam straightBeam)
    {
      return straightBeam.Camber.StartOffset;
    }

    private double GetCamberEndOffset(StraightBeam straightBeam)
    {
      return straightBeam.Camber.EndOffset;
    }
  }
}

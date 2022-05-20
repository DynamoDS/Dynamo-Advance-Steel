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
  public class StraightBeamProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(StraightBeam);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Has Valid Saw Cut", nameof(StraightBeam.HasValidSawCut));

      InsertItem(dictionary, "Camber Height", GetCamberHeight);
      InsertItem(dictionary, "Camber Position", GetCamberPosition);
      InsertItem(dictionary, "Camber Direction YZ", GetCamberDirectionYZStatus);
      InsertItem(dictionary, "Camber Start Offset", GetCamberStartOffset);
      InsertItem(dictionary, "Camber End Offset", GetCamberEndOffset);

      return dictionary;
    }

    private object GetCamberHeight(object straightBeam)
    {
      StraightBeam asStraightBeam = straightBeam as StraightBeam;

      return asStraightBeam.Camber.Height;
    }

    private object GetCamberPosition(object straightBeam)
    {
      StraightBeam asStraightBeam = straightBeam as StraightBeam;

      return asStraightBeam.Camber.Position;
    }

    private object GetCamberDirectionYZStatus(object straightBeam)
    {
      StraightBeam asStraightBeam = straightBeam as StraightBeam;

      return asStraightBeam.Camber.DirectionYZStatus;
    }

    private object GetCamberStartOffset(object straightBeam)
    {
      StraightBeam asStraightBeam = straightBeam as StraightBeam;

      return asStraightBeam.Camber.StartOffset;
    }

    private object GetCamberEndOffset(object straightBeam)
    {
      StraightBeam asStraightBeam = straightBeam as StraightBeam;

      return asStraightBeam.Camber.EndOffset;
    }
  }
}

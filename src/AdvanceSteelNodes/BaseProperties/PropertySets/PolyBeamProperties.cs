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
  public class PolyBeamProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kPolyBeam;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Vector Reference Orientation", nameof(PolyBeam.VecRefOrientation), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Continuous", nameof(PolyBeam.IsContinuous));
      InsertItem(dictionary, objectASType, "Polyline", nameof(PolyBeam.GetPolyline));
      InsertItem(dictionary, "Orientation", GetOrientation);

      return dictionary;
    }

    private object GetOrientation(object beam)
    {
      return ((PolyBeam)beam).Orientation.ToString();
    }
  }
}

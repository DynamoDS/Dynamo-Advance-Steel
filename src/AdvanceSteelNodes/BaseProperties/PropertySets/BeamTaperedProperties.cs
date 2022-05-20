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
  public class BeamTaperedProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(BeamTapered);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Distances", GetDistances);
      InsertItem(dictionary, "Heigths", GetHeigths);

      return dictionary;
    }

    private object GetDistances(object beam)
    {
      ((BeamTapered)beam).GetSegmentsData(out var dists, out var heigths, out var alignment);
      return dists;
    }

    private object GetHeigths(object beam)
    {
      ((BeamTapered)beam).GetSegmentsData(out var dists, out var heigths, out var alignment);
      return heigths;
    }
  }
}
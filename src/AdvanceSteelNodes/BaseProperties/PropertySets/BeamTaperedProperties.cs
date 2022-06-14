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
  public class BeamTaperedProperties : BaseProperties<BeamTapered>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertCustomProperty(dictionary, "Distances", nameof(BeamTaperedProperties.GetDistances), null);
      InsertCustomProperty(dictionary, "Heigths", nameof(BeamTaperedProperties.GetHeigths), null);

      return dictionary;
    }

    private static double[] GetDistances(BeamTapered beam)
    {
      beam.GetSegmentsData(out var dists, out var heigths, out var alignment);
      return dists;
    }

    private static double[] GetHeigths(BeamTapered beam)
    {
      beam.GetSegmentsData(out var dists, out var heigths, out var alignment);
      return heigths;
    }
  }
}
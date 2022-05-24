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
  public class GridProperties : BaseProperties<Grid>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Coordinate System of Grid", nameof(Grid.CS));
      InsertItem(dictionary, "Grid Numbering Start Text", nameof(Grid.NumberingStart));
      InsertItem(dictionary, "Vertical Series", nameof(Grid.VerticalSeries));
      InsertItem(dictionary, "Axis Frame", nameof(Grid.AxisFrame));
      InsertItem(dictionary, "Grid Numbering Prefix", nameof(Grid.NumberingPrefix));
      InsertItem(dictionary, "Grid Numbering Suffix", nameof(Grid.NumberingSuffix));

      InsertItem(dictionary, "Grid Numbering Suffix", nameof(Grid.GetNumSequences));
      InsertItem(dictionary, "Grid Type", GridType);
      InsertItem(dictionary, "Vertical Projection Status", VerticalProjectionStatus);
      InsertItem(dictionary, "Numbering Type", NumberingType);

      return dictionary;
    }

    private object GridType(object grid)
    {
      return ((Grid)grid).GridType.ToString();
    }

    private object VerticalProjectionStatus(object grid)
    {
      return ((Grid)grid).VerticalProjectionStatus.ToString();
    }

    private object NumberingType(object grid)
    {
      return ((Grid)grid).NumberingType.ToString();
    }

  }
}

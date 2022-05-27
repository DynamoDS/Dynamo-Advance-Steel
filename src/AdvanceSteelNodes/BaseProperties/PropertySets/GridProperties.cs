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

      InsertProperty(dictionary, "Coordinate System of Grid", nameof(Grid.CS), LevelEnum.Default);
      InsertProperty(dictionary, "Grid Numbering Start Text", nameof(Grid.NumberingStart));
      InsertProperty(dictionary, "Vertical Series", nameof(Grid.VerticalSeries));
      InsertProperty(dictionary, "Axis Frame", nameof(Grid.AxisFrame));
      InsertProperty(dictionary, "Grid Numbering Prefix", nameof(Grid.NumberingPrefix));
      InsertProperty(dictionary, "Grid Numbering Suffix", nameof(Grid.NumberingSuffix));

      InsertProperty(dictionary, "Grid Numbering Suffix", nameof(Grid.GetNumSequences));
      InsertCustomProperty(dictionary, "Grid Type", nameof(GridProperties.GridType), null);
      InsertCustomProperty(dictionary, "Vertical Projection Status", nameof(GridProperties.VerticalProjectionStatus), null);
      InsertCustomProperty(dictionary, "Numbering Type", nameof(GridProperties.NumberingType), null);

      return dictionary;
    }

    private static string GridType(Grid grid)
    {
      return grid.GridType.ToString();
    }

    private static string VerticalProjectionStatus(Grid grid)
    {
      return grid.VerticalProjectionStatus.ToString();
    }

    private static string NumberingType(Grid grid)
    {
      return grid.NumberingType.ToString();
    }

  }
}

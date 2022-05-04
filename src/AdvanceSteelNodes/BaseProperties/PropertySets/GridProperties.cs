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
  class GridProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kGrid;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Coordinate System of Grid", nameof(Grid.CS));
      InsertItem(dictionary, objectASType, "Grid Numbering Start Text", nameof(Grid.NumberingStart));
      InsertItem(dictionary, objectASType, "Vertical Series", nameof(Grid.VerticalSeries));
      InsertItem(dictionary, objectASType, "Axis Frame", nameof(Grid.AxisFrame));
      InsertItem(dictionary, objectASType, "Grid Numbering Prefix", nameof(Grid.NumberingPrefix));
      InsertItem(dictionary, objectASType, "Grid Numbering Suffix", nameof(Grid.NumberingSuffix));

      InsertItem(dictionary, objectASType, "Grid Numbering Suffix", nameof(Grid.GetNumSequences));
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

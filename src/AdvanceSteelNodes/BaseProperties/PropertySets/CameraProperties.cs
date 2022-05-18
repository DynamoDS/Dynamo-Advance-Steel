using Autodesk.AdvanceSteel.ConstructionHelper;
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
  public class CameraProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(Camera);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Description", nameof(Camera.Description));
      InsertItem(dictionary, "Type", nameof(Camera.CameraType));
      InsertItem(dictionary, "Coordinate System of Camera", nameof(Camera.CameraCS));
      InsertItem(dictionary, "Scale", nameof(Camera.Scale));
      InsertItem(dictionary, "Type Description", nameof(Camera.TypeDescription), LevelEnum.Default);
      InsertItem(dictionary, "Disable Supports Detailing", nameof(Camera.SupportsDetailingDisable), LevelEnum.Default);
      InsertItem(dictionary, "Detail Style Location Index Number", nameof(Camera.DetailStyleLocation));
      InsertItem(dictionary, "Detail Style Index Number", nameof(Camera.DetailStyle));
      InsertItem(dictionary, "Disable Detailing", nameof(Camera.DisableDetailing));

      return dictionary;
    }
  }
}
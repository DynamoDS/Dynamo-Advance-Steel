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
  public class CameraProperties : BaseProperties<Camera>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Description", nameof(Camera.Description));
      InsertProperty(dictionary, "Camera Type", nameof(Camera.CameraType));
      InsertProperty(dictionary, "Coordinate System of Camera", nameof(Camera.CameraCS));
      InsertProperty(dictionary, "Scale", nameof(Camera.Scale));
      InsertProperty(dictionary, "Type Description", nameof(Camera.TypeDescription), LevelEnum.Default);
      InsertProperty(dictionary, "Disable Supports Detailing", nameof(Camera.SupportsDetailingDisable), LevelEnum.Default);
      InsertProperty(dictionary, "Detail Style Location Index Number", nameof(Camera.DetailStyleLocation));
      InsertProperty(dictionary, "Detail Style Index Number", nameof(Camera.DetailStyle));
      InsertProperty(dictionary, "Disable Detailing", nameof(Camera.DisableDetailing));

      return dictionary;
    }
  }
}
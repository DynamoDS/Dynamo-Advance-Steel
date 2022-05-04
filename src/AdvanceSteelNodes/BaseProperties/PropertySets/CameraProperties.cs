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
  class CameraProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kCamera;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Description", nameof(Camera.Description));
      InsertItem(dictionary, objectASType, "Type", nameof(Camera.CameraType));
      InsertItem(dictionary, objectASType, "Coordinate System of Camera", nameof(Camera.CameraCS));
      InsertItem(dictionary, objectASType, "Scale", nameof(Camera.Scale));
      InsertItem(dictionary, objectASType, "Type Description", nameof(Camera.TypeDescription), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Disable Supports Detailing", nameof(Camera.SupportsDetailingDisable), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Detail Style Location Index Number", nameof(Camera.DetailStyleLocation));
      InsertItem(dictionary, objectASType, "Detail Style Index Number", nameof(Camera.DetailStyle));
      InsertItem(dictionary, objectASType, "Disable Detailing", nameof(Camera.DisableDetailing));

      return dictionary;
    }
  }
}
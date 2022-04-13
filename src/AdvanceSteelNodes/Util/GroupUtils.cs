using Autodesk.AdvanceSteel.BuildingStructure;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CADObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using ASObjectId = Autodesk.AdvanceSteel.CADLink.Database.ObjectId;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.CADAccess;

namespace AdvanceSteel.Nodes.Util
{
  [IsVisibleInDynamoLibrary(false)]
  public class GroupUtils
  {
    internal GroupUtils()
    {

    }

    /// <summary>
    /// Get all model groups by structure
    /// </summary>
    /// <returns></returns>
    public static List<(CADObjectId, string)> GetListGroups(string structureHandle)
    {
      BuildingStructureObject asStructure = Utils.GetObject(structureHandle) as BuildingStructureObject;

      if (asStructure == null)
        return null;

      BuildingStructureTreeObject groupsTreeObject = asStructure.GroupsTreeObject;

      return groupsTreeObject.StructureItems.OfType<ObjectsGroup>().Select(x => (new CADObjectId(x.GetObjectId().AsOldId()), x.Parent.GetStructureItemName(x))).ToList();
    }

    public static IEnumerable<SteelDbObject> GetASObjectsByGroupHandle(string groupName, string handle)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        ObjectsGroup objectsGroup = Utils.GetObject(handle) as ObjectsGroup;
        var listASObjectId = objectsGroup.getObjectsIDs().Where(x => !x.IsNull());

        return Utils.GetDynObjects(listASObjectId);
      }
    }
  }
}
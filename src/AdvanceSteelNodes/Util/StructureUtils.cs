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

namespace AdvanceSteel.Nodes.Util
{
  [IsVisibleInDynamoLibrary(false)]
  public class StructureUtils
  {
    internal StructureUtils()
    {

    }

    /// <summary>
    /// Get all model structures
    /// </summary>
    /// <returns></returns>
    public static List<(CADObjectId, string)> GetListStructures()
    {
      BuildingStructureManager buildStructMan = BuildingStructureManager.getBuildingStructureManager();
      // get the list objects from the manager
      BuildingStructureManagerListObject buildStructManListObject = buildStructMan.ListObject;
      // get all of the structures in the BuildingStructureManagerListObject 
      List<BuildingStructureObject> bsoList = buildStructManListObject.Structures;

      return bsoList.Select(x => (new CADObjectId(x.GetObjectId().AsOldId()), buildStructManListObject.GetStructureName(x))).ToList();
    }

    /// <summary>
    /// Rename Structure 
    /// </summary>
    /// <param name="structureHandle"></param>
    /// <param name="name"></param>
    public static void RenameStructure(string structureHandle, string name)
    {
      if (String.IsNullOrEmpty(name))
      {
        throw new ArgumentNullException(nameof(name));
      }

      using (var ctx = new SteelServices.DocContext())
      {
        BuildingStructureObject asStructure = Utils.GetObject(structureHandle) as BuildingStructureObject;

        BuildingStructureManager buildStructMan = BuildingStructureManager.getBuildingStructureManager();

        // get the list objects from the manager
        BuildingStructureManagerListObject buildStructManListObject = buildStructMan.ListObject;

        string currentName = buildStructManListObject.GetStructureName(asStructure);

        if (currentName.ToLower().Equals(name.ToLower()))
        {
          throw new Exception(string.Format("The Structure '{0}' already has this name", name));
        }

        bool hasAlreadyName = buildStructManListObject.Structures.Any(x => buildStructManListObject.GetStructureName(x).ToLower().Equals(name.ToLower()));
        if (hasAlreadyName)
        {
          throw new Exception(string.Format("There is another Structure with this name '{0}'", name));
        }

        buildStructManListObject.SetStructureName(asStructure, name);

        //DocumentController.Instance.StructureRenamed(new CADObjectId(asStructure.GetObjectId().AsOldId()));
      }
    }
  }
}

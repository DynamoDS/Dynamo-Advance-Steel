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
  public class ProjectExplorer
  {
    internal ProjectExplorer()
    {

    }

    /// <summary>
    /// Rename Structure 
    /// </summary>
    /// <param name="structureHandle"></param>
    /// <param name="name"></param>
    public static void RenameStructure(string structureHandle, string name)
    {
      StructureUtils.RenameStructure(structureHandle, name);
    }
  }
}
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Connection;
using Autodesk.AdvanceSteel.ConstructionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ASObjectId = Autodesk.AdvanceSteel.CADLink.Database.ObjectId;

namespace AdvanceSteel.Nodes
{
  public static class HolesUtils
  {
    public static List<ConnectionHoleFeature> GetHoles(AtomicElement pAtomicElement)
    {
      List<ConnectionHoleFeature> holes = new List<ConnectionHoleFeature>();

      if (pAtomicElement == null)
        return holes;

      var features = pAtomicElement.GetFeatures(true);

      foreach (ASObjectId objectIDASFeature in features)
      {
        FilerObject filerObject = DatabaseManager.Open(objectIDASFeature);
        if (filerObject is ConnectionHoleFeature)
        {
          ConnectionHoleFeature connectionHoleFeature = (ConnectionHoleFeature)filerObject;
          holes.Add(connectionHoleFeature);
        }
      }

      return holes;
    }
  }
}
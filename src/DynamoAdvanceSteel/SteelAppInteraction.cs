using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.DocumentManagement;
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.CADLink.Database;
using Autodesk.AdvanceSteel.Geometry;

namespace Dynamo.Applications.AdvanceSteel
{
  class SteelAppInteraction : IAppInteraction
  {
    public Autodesk.AdvanceSteel.DotNetRoots.Units.UnitsSet DbUnits
    {
      get { return DocumentManager.GetCurrentDocument().CurrentDatabase.Units; }
    }

    public event EventHandler DocumentOpened;

    public void ExecuteOnIdle(Action a)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<string> GetCurrentSelection()
    {
      throw new NotImplementedException();
    }

    public Point3d PickPoint()
    {
      return UserInteraction.GetPoint("Pick Point: ", 1);
    }


    public IEnumerable<string> PickElements()
    {
      List<string> ret = new List<string>() { };

      using (var ctx = new DocContext())
      {
        List<ObjectId> OIDx = UserInteraction.SelectObjects();
        if (OIDx.Count > 0)
        {
          for (int i = 0; i < OIDx.Count; i++)
          {
            FilerObject obj = FilerObject.GetFilerObject(OIDx[i]);
            if (obj != null)
            {
              ret.Add(obj.Handle);
            }
          }
        }
      }
      return ret;
    }
  }
}

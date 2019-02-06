using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.DocumentManagement;

namespace Dynamo.Applications.AdvanceSteel
{
  class SteelAppInteraction : IAppInteraction
  {
    public Autodesk.AdvanceSteel.DotNetRoots.Units.UnitsSet DbUnits
    {
      get { return DocumentManager.GetCurrentDocument().CurrentDatabase.Units; }
    }

    public event EventHandler DocumentOpened;

    public IEnumerable<string> GetCurrentSelection()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<string> PickElements()
    {
      throw new NotImplementedException();
    }
  }
}

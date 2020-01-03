using Autodesk.AdvanceSteel.DotNetRoots.Units;
using System;
using System.Collections.Generic;

namespace Dynamo.Applications.AdvanceSteel.Services
{
  public interface IAppInteraction
  {
    event EventHandler DocumentOpened;
    UnitsSet DbUnits { get; }
    IEnumerable<string> PickElements();
    IEnumerable<string> GetCurrentSelection();
    void ExecuteOnIdle(Action a);
  }
}

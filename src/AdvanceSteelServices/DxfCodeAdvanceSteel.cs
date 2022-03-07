using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamo.Applications.AdvanceSteel.Services
{
  public static class DxfCodeAdvanceSteel
  {
    public static string GroupFilerObject { get; private set; } = "ASTOBJECTSGROUP";

    public static string StructureFilerObject { get; private set; } = "ASTBUILDINGSTRUCTUREOBJECT";

    public static string StructureManagerFilerObject { get; private set; } = "ASTBUILDINGSTRUCTUREMANAGERLISTOBJECT";
  }
}

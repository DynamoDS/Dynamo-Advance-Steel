using Autodesk.AdvanceSteel.ConstructionTypes;
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
  public class UserAutoConstructionObjectProperties : BaseProperties<UserAutoConstructionObject>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Export Available", nameof(UserAutoConstructionObject.ExportIsAvailable));
      InsertItem(dictionary, "Use Detailed Torsors", nameof(UserAutoConstructionObject.UseDetailedTorsors));
      InsertItem(dictionary, "Use Load Cases Is Available", nameof(UserAutoConstructionObject.UseLoadCasesIsAvailable));
      InsertItem(dictionary, "Import Is Available", nameof(UserAutoConstructionObject.ImportIsAvailable));
      InsertItem(dictionary, "Presize Is Available", nameof(UserAutoConstructionObject.PresizeIsAvailable));

      InsertItem(dictionary, "NSA Module Internal Name", nameof(UserAutoConstructionObject.NSAModuleInternalName));
      InsertItem(dictionary, "New Rule Type RunName", nameof(UserAutoConstructionObject.NewRuleTypeRunName));
      InsertItem(dictionary, "NSA Module Standard", nameof(UserAutoConstructionObject.NSAModuleStandard));
      InsertItem(dictionary, "NSA Module Name", nameof(UserAutoConstructionObject.NSAModuleName));
      InsertItem(dictionary, "Check Is Available", nameof(UserAutoConstructionObject.CheckIsAvailable));
      InsertItem(dictionary, "Update When Driver Feature Changed", nameof(UserAutoConstructionObject.UpdateWhenDriverFeatChanged));

      InsertItem(dictionary, "Check Status", GetCheckStatus);
      InsertItem(dictionary, "Approval Status", GetApprovalStatus);

      InsertItem(dictionary, "Input Objects", GetInputObjects);
      InsertItem(dictionary, "Created Objects", GetCreatedObjects);

      return dictionary;
    }
    private object GetCheckStatus(object joint)
    {
      return ((UserAutoConstructionObject)joint).CheckStatus.ToString();
    }

    private object GetApprovalStatus(object joint)
    {
      return ((UserAutoConstructionObject)joint).ApprovalStatus.ToString();
    }

    private object GetInputObjects(object joint)
    {
      return ((UserAutoConstructionObject)joint).InputObjects.Select(x => x.ToDSType());
    }

    private object GetCreatedObjects(object joint)
    {
      return ((UserAutoConstructionObject)joint).CreatedObjects.Where(x => x is Beam || x is Plate || x is BoltPattern).Select(x => x.ToDSType());
    }

  }
}

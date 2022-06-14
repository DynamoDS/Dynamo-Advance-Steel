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

      InsertProperty(dictionary, "Export Available", nameof(UserAutoConstructionObject.ExportIsAvailable));
      InsertProperty(dictionary, "Use Detailed Torsors", nameof(UserAutoConstructionObject.UseDetailedTorsors));
      InsertProperty(dictionary, "Use Load Cases Is Available", nameof(UserAutoConstructionObject.UseLoadCasesIsAvailable));
      InsertProperty(dictionary, "Import Is Available", nameof(UserAutoConstructionObject.ImportIsAvailable));
      InsertProperty(dictionary, "Presize Is Available", nameof(UserAutoConstructionObject.PresizeIsAvailable));

      InsertProperty(dictionary, "NSA Module Internal Name", nameof(UserAutoConstructionObject.NSAModuleInternalName));
      InsertProperty(dictionary, "New Rule Type RunName", nameof(UserAutoConstructionObject.NewRuleTypeRunName));
      InsertProperty(dictionary, "NSA Module Standard", nameof(UserAutoConstructionObject.NSAModuleStandard));
      InsertProperty(dictionary, "NSA Module Name", nameof(UserAutoConstructionObject.NSAModuleName));
      InsertProperty(dictionary, "Check Is Available", nameof(UserAutoConstructionObject.CheckIsAvailable));
      InsertProperty(dictionary, "Update When Driver Feature Changed", nameof(UserAutoConstructionObject.UpdateWhenDriverFeatChanged));

      InsertCustomProperty(dictionary, "Check Status", nameof(UserAutoConstructionObjectProperties.GetCheckStatus), null);
      InsertCustomProperty(dictionary, "Approval Status", nameof(UserAutoConstructionObjectProperties.GetApprovalStatus), null);

      InsertCustomProperty(dictionary, "Input Objects", nameof(UserAutoConstructionObjectProperties.GetInputObjects), null);
      InsertCustomProperty(dictionary, "Created Objects", nameof(UserAutoConstructionObjectProperties.GetCreatedObjects), null);

      return dictionary;
    }
    private static string GetCheckStatus(UserAutoConstructionObject joint)
    {
      return joint.CheckStatus.ToString();
    }

    private static string GetApprovalStatus(UserAutoConstructionObject joint)
    {
      return joint.ApprovalStatus.ToString();
    }

    private static List<SteelDbObject> GetInputObjects(UserAutoConstructionObject joint)
    {
      return joint.InputObjects.Select(x => x.ToDSType()).ToList();
    }

    private static List<SteelDbObject> GetCreatedObjects(UserAutoConstructionObject joint)
    {
      return joint.CreatedObjects.Where(x => x is Beam || x is Plate || x is BoltPattern).Select(x => x.ToDSType()).ToList();
    }

  }
}

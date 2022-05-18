using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;

namespace AdvanceSteel.Nodes
{
  public class MainAliasProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(MainAlias);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Used For Numbering - Fabrication Station", nameof(MainAlias.FabricationStationUsedForNumbering));
      InsertItem(dictionary, "Load Number", nameof(MainAlias.LoadNumber));
      InsertItem(dictionary, "Carrier", nameof(MainAlias.Carrier));
      InsertItem(dictionary, "Fabrication Station", nameof(MainAlias.FabricationStation));
      InsertItem(dictionary, "Supplier", nameof(MainAlias.Supplier));
      InsertItem(dictionary, "PO Number", nameof(MainAlias.PONumber));
      InsertItem(dictionary, "Requisition Number", nameof(MainAlias.RequisitionNumber));
      InsertItem(dictionary, "Heat Number", nameof(MainAlias.HeatNumber));
      InsertItem(dictionary, "Shipped Date", nameof(MainAlias.ShippedDate));
      InsertItem(dictionary, "Delivery Date", nameof(MainAlias.DeliveryDate));
      InsertItem(dictionary, "Used For Numbering - Supplier", nameof(MainAlias.SupplierUsedForNumbering));
      InsertItem(dictionary, "Used For Numbering - RequisitionNumber", nameof(MainAlias.RequisitionNumberUsedForNumbering));
      InsertItem(dictionary, "Approval Comment", nameof(MainAlias.ApprovalComment));
      InsertItem(dictionary, "Used For Numbering - Heat Number", nameof(MainAlias.HeatNumberUsedForNumbering));
      InsertItem(dictionary, "Used For Numbering - PO Number", nameof(MainAlias.PONumberUsedForNumbering));
      InsertItem(dictionary, "Approval Status Code", nameof(MainAlias.ApprovalStatusCode));

      InsertItem(dictionary, "Standard Weight", nameof(MainAlias.GetStandardWeight));

      return dictionary;
    }

  }
}

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
  public class MainAliasProperties : BaseProperties<MainAlias>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Used For Numbering - Fabrication Station", nameof(MainAlias.FabricationStationUsedForNumbering));
      InsertProperty(dictionary, "Load Number", nameof(MainAlias.LoadNumber));
      InsertProperty(dictionary, "Carrier", nameof(MainAlias.Carrier));
      InsertProperty(dictionary, "Fabrication Station", nameof(MainAlias.FabricationStation));
      InsertProperty(dictionary, "Supplier", nameof(MainAlias.Supplier));
      InsertProperty(dictionary, "PO Number", nameof(MainAlias.PONumber));
      InsertProperty(dictionary, "Requisition Number", nameof(MainAlias.RequisitionNumber));
      InsertProperty(dictionary, "Heat Number", nameof(MainAlias.HeatNumber));
      InsertProperty(dictionary, "Shipped Date", nameof(MainAlias.ShippedDate));
      InsertProperty(dictionary, "Delivery Date", nameof(MainAlias.DeliveryDate));
      InsertProperty(dictionary, "Used For Numbering - Supplier", nameof(MainAlias.SupplierUsedForNumbering));
      InsertProperty(dictionary, "Used For Numbering - RequisitionNumber", nameof(MainAlias.RequisitionNumberUsedForNumbering));
      InsertProperty(dictionary, "Approval Comment", nameof(MainAlias.ApprovalComment));
      InsertProperty(dictionary, "Used For Numbering - Heat Number", nameof(MainAlias.HeatNumberUsedForNumbering));
      InsertProperty(dictionary, "Used For Numbering - PO Number", nameof(MainAlias.PONumberUsedForNumbering));
      InsertProperty(dictionary, "Approval Status Code", nameof(MainAlias.ApprovalStatusCode));

      InsertProperty(dictionary, "Standard Weight", nameof(MainAlias.GetStandardWeight));

      return dictionary;
    }

  }
}

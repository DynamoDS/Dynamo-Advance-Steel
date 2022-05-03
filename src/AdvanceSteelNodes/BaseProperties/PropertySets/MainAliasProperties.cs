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
    public override eObjectType GetObjectType => eObjectType.kMainAlias;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Used For Numbering - Fabrication Station", nameof(MainAlias.FabricationStationUsedForNumbering));
      InsertItem(dictionary, objectASType, "Load Number", nameof(MainAlias.LoadNumber));
      InsertItem(dictionary, objectASType, "Carrier", nameof(MainAlias.Carrier));
      InsertItem(dictionary, objectASType, "Fabrication Station", nameof(MainAlias.FabricationStation));
      InsertItem(dictionary, objectASType, "Supplier", nameof(MainAlias.Supplier));
      InsertItem(dictionary, objectASType, "PO Number", nameof(MainAlias.PONumber));
      InsertItem(dictionary, objectASType, "Requisition Number", nameof(MainAlias.RequisitionNumber));
      InsertItem(dictionary, objectASType, "Heat Number", nameof(MainAlias.HeatNumber));
      InsertItem(dictionary, objectASType, "Shipped Date", nameof(MainAlias.ShippedDate));
      InsertItem(dictionary, objectASType, "Delivery Date", nameof(MainAlias.DeliveryDate));
      InsertItem(dictionary, objectASType, "Used For Numbering - Supplier", nameof(MainAlias.SupplierUsedForNumbering));
      InsertItem(dictionary, objectASType, "Used For Numbering - RequisitionNumber", nameof(MainAlias.RequisitionNumberUsedForNumbering));
      InsertItem(dictionary, objectASType, "Approval Comment", nameof(MainAlias.ApprovalComment));
      InsertItem(dictionary, objectASType, "Used For Numbering - Heat Number", nameof(MainAlias.HeatNumberUsedForNumbering));
      InsertItem(dictionary, objectASType, "Used For Numbering - PO Number", nameof(MainAlias.PONumberUsedForNumbering));
      InsertItem(dictionary, objectASType, "Approval Status Code", nameof(MainAlias.ApprovalStatusCode));

      InsertItem(dictionary, objectASType, "Standard Weight", nameof(MainAlias.GetStandardWeight));

      return dictionary;
    }

  }
}

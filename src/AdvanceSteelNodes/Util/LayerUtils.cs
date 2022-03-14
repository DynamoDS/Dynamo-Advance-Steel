using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceSteel.Nodes.Util
{
  public static class LayerUtils
  {
    public static List<LayerTableRecord> GetAllLayers()
    {
      Database db = HostApplicationServices.WorkingDatabase;
      var trans = db.TransactionManager.TopTransaction;

      LayerTable lt = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;

      List<LayerTableRecord> listaLayers = new List<LayerTableRecord>();
      foreach (ObjectId ltrId in lt)
      {
        LayerTableRecord ltr = trans.GetObject(ltrId, OpenMode.ForRead) as LayerTableRecord;

        if (ltr != null && !ltr.ObjectId.IsNull && !ltr.ObjectId.IsErased)
          listaLayers.Add(ltr);
      }

      return listaLayers;
    }
  }
}

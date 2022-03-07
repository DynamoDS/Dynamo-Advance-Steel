using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace Dynamo.Applications.AdvanceSteel.Services
{
  public class ObjectCADEventArgs : EventArgs
  {
    public enum UpdateType
    {
      Added,
      Modified,
      Deleted,
    }

    public UpdateType Operation { get; private set; }

    public List<ObjectId> ListObjectId { get; private set; }

    public ObjectCADEventArgs(UpdateType operation, List<ObjectId> listObjectId)
    {
      Operation = operation;
      ListObjectId = listObjectId;
    }
  }
}

using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamo.Applications.AdvanceSteel.Services
{
  public class DocumentEventArgs : EventArgs
  {
    public Document Document { get; set; }
  }
}

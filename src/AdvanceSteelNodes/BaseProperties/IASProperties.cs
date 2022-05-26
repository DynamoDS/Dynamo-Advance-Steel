﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;

namespace AdvanceSteel.Nodes
{
  public interface IASProperties
  {
    Type GetObjectType { get; }

    Dictionary<string, Property> BuildPropertyList();
  }
}
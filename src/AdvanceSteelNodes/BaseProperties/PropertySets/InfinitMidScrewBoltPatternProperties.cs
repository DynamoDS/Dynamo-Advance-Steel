﻿using Autodesk.AdvanceSteel.Arrangement;
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
  public class InfinitMidScrewBoltPatternProperties : BaseProperties<InfinitMidScrewBoltPattern>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Midpoint On Lower Left", nameof(InfinitMidScrewBoltPattern.MidpointOnLowerLeft), LevelEnum.Default);
      InsertProperty(dictionary, "Midpoint On Lower Right", nameof(InfinitMidScrewBoltPattern.MidpointOnLowerRight), LevelEnum.Default);
      InsertProperty(dictionary, "Midpoint On Upper Left", nameof(InfinitMidScrewBoltPattern.MidpointOnUpperLeft), LevelEnum.Default);

      return dictionary;
    }
  }
}
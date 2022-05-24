﻿using Autodesk.AdvanceSteel.ConstructionTypes;
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
  public class FeatureObjectProperties : BaseProperties<FeatureObject>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Is From Fitter", nameof(FeatureObject.IsFromFitter), LevelEnum.Default);
      InsertItem(dictionary, "Coordinate System", nameof(FeatureObject.CS));
      InsertItem(dictionary, "Use Gap", nameof(FeatureObject.UseGap));
      InsertItem(dictionary, "Object Index", nameof(FeatureObject.ObjectIndex));

      return dictionary;
    }
  }
}
﻿using Autodesk.AdvanceSteel.Geometry;
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
  public class PolyBeamProperties : BaseProperties<PolyBeam>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Vector Reference Orientation", nameof(PolyBeam.VecRefOrientation), LevelEnum.Default);
      InsertProperty(dictionary, "Continuous", nameof(PolyBeam.IsContinuous));
      InsertProperty(dictionary, "Poly Curve", nameof(PolyBeamProperties.GetPolyline));
      InsertCustomProperty(dictionary, "Orientation", nameof(PolyBeamProperties.GetOrientation), null);

      return dictionary;
    }

    private string GetOrientation(PolyBeam beam)
    {
      return beam.Orientation.ToString();
    }

    private Polyline3d GetPolyline(PolyBeam beam)
    {
      return beam.GetPolyline(true);
    }

    private void SetPolyline(PolyBeam beam, Polyline3d newPolyline)
    {
      beam.SetPolyline(newPolyline);
    }

  }
}
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modeler;
using Autodesk.AdvanceSteel.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Util
{
  /// <summary>
  /// Functions to work with Polybeam Steel Objects
  /// </summary>
  public class Polybeams
  {

    internal Polybeams()
    {
    }

    /// <summary>
    /// Get Polycurve from Polybeam
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <returns></returns>
    public static Autodesk.DesignScript.Geometry.PolyCurve GetPolyCurve(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      List<Autodesk.DesignScript.Geometry.Curve> intRet = new List<Autodesk.DesignScript.Geometry.Curve>() { };
      Autodesk.DesignScript.Geometry.PolyCurve ret = null;
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kPolyBeam))
            {

              PolyBeam selectedObj = filerObj as PolyBeam;
              Polyline3d poly = selectedObj.GetPolyline();
              intRet = Utils.ToDynPolyCurves(poly, true);
              ret = Autodesk.DesignScript.Geometry.PolyCurve.ByJoinedCurves(intRet);
            }
            throw new System.Exception("Wrong type of Steel Object found, must be a Polybeam");
          }
        }
        else
          throw new System.Exception("No Steel Object found or Line Object is null");
      }
      return ret;
    }

    /// <summary>
    /// Sets the Polycurve in an Advance Steel Polybeam
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <param name="polyCurve"> Input Dynamo Polycurve</param>
    /// <returns></returns>
    public static void SetPolyCurve(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                        Autodesk.DesignScript.Geometry.PolyCurve polyCurve)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kPolyBeam))
            {
              Autodesk.AdvanceSteel.Modelling.PolyBeam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.PolyBeam;
              selectedObj.SetPolyline(Utils.ToAstPolyline3d(polyCurve, true));
            }
            throw new System.Exception("Wrong type of Steel Object found, must be a Polybeam");
          }
        }
        else
          throw new System.Exception("No Steel Object found or Line Object is null");
      }
    }
  }
}
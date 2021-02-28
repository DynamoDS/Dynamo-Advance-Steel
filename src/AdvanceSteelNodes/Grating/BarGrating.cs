using System;
using AdvanceSteel.Nodes.Plates;
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.DesignScript.Runtime;
using DynGeometry = Autodesk.DesignScript.Geometry;
using SteelGeometry = Autodesk.AdvanceSteel.Geometry;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.ConstructionTypes;
using System.Collections.Generic;
using Autodesk.AdvanceSteel.Geometry;
using System.Linq;

namespace AdvanceSteel.Nodes.Gratings
{
  /// <summary>
  /// Advance Steel Bar Grating Pattern
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class BarGrating : GraphicObject
  {
    internal BarGrating()
    {
    }

    internal BarGrating(Vector3d vNormal, Point3d ptCenter, double dLength, List<Property> additionalGratingParameters)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          List<Property> defaultData = additionalGratingParameters.Where(x => x.Level == ".").ToList<Property>();
          List<Property> postWriteDBData = additionalGratingParameters.Where(x => x.Level == "Z_PostWriteDB").ToList<Property>();

          Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(ptCenter, vNormal);
          Autodesk.AdvanceSteel.Modelling.Grating gratings = null;
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {

            gratings = new Autodesk.AdvanceSteel.Modelling.Grating("ADT", 11, 2, "3 / 16 inch", "10", "3/16", plane, ptCenter, dLength);

            if (defaultData != null)
            {
              Utils.SetParameters(gratings, defaultData);
            }

            gratings.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(gratings, postWriteDBData);
            }
          }
          else
          {
            gratings = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Grating;
            if (gratings != null && gratings.IsKindOf(FilerObject.eObjectType.kGrating))
            {
              gratings.DefinitionPlane = plane;
              gratings.SetLength(dLength, true);

              if (defaultData != null)
              {
                Utils.SetParameters(gratings, defaultData);
              }

              if (postWriteDBData != null)
              {
                Utils.SetParameters(gratings, postWriteDBData);
              }
            }
            else
            {
              throw new System.Exception("Not a Bar Grating pattern");
            }
          }
          Handle = gratings.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(gratings);
        }
      }
    }
    /// <summary>
    /// Create an Advance Steel Bar Grating
    /// </summary>
    /// <param name="line"> Input Dynamo Line</param>
    /// <param name="planeDirection"> Input Dynamo Vector to set Normal of Grating</param>
    /// <param name="additionalGratingParameters"> Optional Input Grating Build Properties </param>
    /// <returns name="barGrating"> grating</returns>
    public static BarGrating ByLine(Autodesk.DesignScript.Geometry.Line line,
                                    Autodesk.DesignScript.Geometry.Vector planeDirection,
                                    [DefaultArgument("null")] List<Property> additionalGratingParameters)
    {
      var start = Utils.ToAstPoint(line.StartPoint, true);
      var end = Utils.ToAstPoint(line.EndPoint, true);
      var refPoint = start + (end - start) * 0.5;
      var planeNorm = Utils.ToAstVector3d(planeDirection, true);

      if (!planeNorm.IsPerpendicularTo(Utils.ToAstVector3d(line.Direction, true)))
      {
        throw new System.Exception("Plan Direction must be perpendicular to line");
      }

      additionalGratingParameters = PreSetDefaults(additionalGratingParameters);
      return new BarGrating(planeNorm, refPoint, Utils.ToInternalDistanceUnits(line.Length, true), additionalGratingParameters);
    }

    private static List<Property> PreSetDefaults(List<Property> listGratingData)
    {
      if (listGratingData == null)
      {
        listGratingData = new List<Property>() { };
      }
      return listGratingData;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var grating = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.Grating;

          if (grating == null)
          {
            throw new Exception("Null Variable Grating pattern");
          }

          List<DynGeometry.Point> polyPoints = GratingDraw.GetPointsToDraw(grating);

          return Autodesk.DesignScript.Geometry.Polygon.ByPoints(polyPoints);
        }
      }
    }
  }
}


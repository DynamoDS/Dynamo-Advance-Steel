using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.NonSteelItems
{
  /// <summary>
  /// Advance Steel Special Part
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class SpecialPart : GraphicObject
  {
    internal SpecialPart()
    {
    }

    internal SpecialPart(Matrix3d insertMatrix, string blockName, List<ASProperty> cameraProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {

          List<ASProperty> defaultData = cameraProperties.Where(x => x.PropLevel == ".").ToList<ASProperty>();
          List<ASProperty> postWriteDBData = cameraProperties.Where(x => x.PropLevel == "Z_PostWriteDB").ToList<ASProperty>();

          double scale = (double)defaultData.FirstOrDefault<ASProperty>(x => x.PropName == "Scale").PropValue;

          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Autodesk.AdvanceSteel.Modelling.SpecialPart specPart = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            specPart = new Autodesk.AdvanceSteel.Modelling.SpecialPart(insertMatrix);
            specPart.SetBlock(blockName, scale);
            if (defaultData != null)
            {
              Utils.SetParameters(specPart, defaultData);
            }
            specPart.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(specPart, postWriteDBData);
            }

          }
          else
          {
            specPart = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.SpecialPart;

            if (specPart != null && specPart.IsKindOf(FilerObject.eObjectType.kSpecialPart))
            {
              specPart.SetBlock(blockName, scale);
              if (defaultData != null)
              {
                Utils.SetParameters(specPart, defaultData);
              }

              if (postWriteDBData != null)
              {
                Utils.SetParameters(specPart, postWriteDBData);
              }
            }
            else
              throw new System.Exception("Not a Special Part");
          }

          Handle = specPart.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(specPart);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel Special Part
    /// </summary>
    /// <param name="coordinateSystem">Input Dynamo Coordinate System</param>
    /// <param name="blockName"> Input Blockname to be used by SpecialPart</param>
    /// <param name="scale"> Input Special Part Scale</param>
    /// <param name="additionalSpecialPartsParameters"> Optional Input Camera Build Properties </param>
    /// <returns></returns>
    public static SpecialPart ByCSAndBlockName(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                              string blockName,
                              [DefaultArgument("1")]double scale,
                              [DefaultArgument("null")]List<ASProperty> additionalSpecialPartsParameters)
    {
      Matrix3d spMatrix = Utils.ToAstMatrix3d(coordinateSystem, true);
      additionalSpecialPartsParameters = PreSetDefaults(additionalSpecialPartsParameters, scale);

      return new SpecialPart(spMatrix, blockName, additionalSpecialPartsParameters);
    }

    private static List<ASProperty> PreSetDefaults(List<ASProperty> listSpecialPartData, double scale)
    {
      if (listSpecialPartData == null)
      {
        listSpecialPartData = new List<ASProperty>() { };
        Utils.CheckListUpdateOrAddValue(listSpecialPartData, "Scale", scale, ".");
      }
      return listSpecialPartData;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var camera = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.SpecialPart;

          Matrix3d cameraCS = camera.CS;
          Vector3d xVect = null;
          Vector3d yVect = null;
          Vector3d ZVect = null;
          Point3d origin = null;
          cameraCS.GetCoordSystem(out origin, out xVect, out yVect, out ZVect);

          using (var dynPoint = Utils.ToDynPoint(origin, true))
          {
            return Autodesk.DesignScript.Geometry.Circle.ByCenterPointRadius(dynPoint, 0.01);
          }
        }
      }
    }
  }
}
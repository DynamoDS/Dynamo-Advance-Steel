using AdvanceSteel.Nodes.Plates;
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.DesignScript.Runtime;
using DynGeometry = Autodesk.DesignScript.Geometry;
using SteelGeometry = Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.DotNetRoots.DatabaseAccess;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.ConstructionTypes;
using System.Collections.Generic;
using Autodesk.AdvanceSteel.Geometry;
using System.Linq;
using System;


namespace AdvanceSteel.Nodes.Util
{
  /// <summary>
  /// Store Camera properties in a Node
  /// </summary>
  public class Cameras
  {
    internal Cameras()
    {
    }

    /// <summary>
    /// Set Advance Steel Camera Clipping Values
    /// </summary>
    /// <param name="steelObject"> Selected Advance Steel Camera Object</param>
    /// <param name="clippingSide"> Set Clipping Side of Camera 0 - None, 1 = Upper, 2 - Lower, 3 - Both</param>
    /// <param name="upperClippingValue"> Set Upper Clipping Value</param>
    /// <param name="lowerClippingValue"> Set Lower Clipping Value</param>
    /// <returns></returns>
    public static void SetZClipping(SteelDbObject steelObject,
                                    [DefaultArgument("3")] int clippingSide,
                                    [DefaultArgument("0")] double upperClippingValue,
                                    [DefaultArgument("0")] double lowerClippingValue)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        string handle = steelObject.Handle;
        FilerObject obj = Utils.GetObject(handle);

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kCamera))
        {
          Autodesk.AdvanceSteel.ConstructionHelper.Camera camera = obj as Autodesk.AdvanceSteel.ConstructionHelper.Camera;
          camera.setZClipping((Autodesk.AdvanceSteel.ConstructionHelper.eZClip)clippingSide,
                              Utils.ToInternalUnits(upperClippingValue, true),
                              Utils.ToInternalUnits(lowerClippingValue, true));
        }
        else
          throw new System.Exception("Failed to Get Camera Object");
      }
    }

    /// <summary>
    /// Set Advance Steel Camera Extents / Size
    /// </summary>
    /// <param name="steelObject"> Selected Advance Steel Camera Object</param>
    /// <param name="cameraExtents"> Set Camera Extents 0 - Automatic, 3 - Fixed Size</param>
    /// <param name="xCameraSize"> Set Camera Extents in X Direction</param>
    /// <param name="yCameraSize"> Set Camera Extents in Y Direction</param>
    public static void SetXYExtents(SteelDbObject steelObject,
                                    [DefaultArgument("0")] int cameraExtents,
                                    [DefaultArgument("0")] double xCameraSize,
                                    [DefaultArgument("0")] double yCameraSize)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        string handle = steelObject.Handle;
        FilerObject obj = Utils.GetObject(handle);

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kCamera))
        {
          Autodesk.AdvanceSteel.ConstructionHelper.Camera camera = obj as Autodesk.AdvanceSteel.ConstructionHelper.Camera;
          camera.setXYExtents((Autodesk.AdvanceSteel.ConstructionHelper.Camera.eXYExtents)cameraExtents,
                              Utils.ToInternalUnits(xCameraSize, true),
                              Utils.ToInternalUnits(yCameraSize, true));
        }
        else
          throw new System.Exception("Failed to Get Camera Object");
      }
    }

    /// <summary>
    /// Get Camera Extents Values
    /// </summary>
    /// <param name="steelObject"> Selected Advance Steel Camera Object</param>
    /// <returns></returns>
    [MultiReturn(new[] { "X_Length", "Y_Length" })]
    public static Dictionary<string, double> GetCameraExtents(SteelDbObject steelObject)
    {
      Dictionary<string, double> ret = new Dictionary<string, double>();

      double xLength = 0;
      double yLength = 0;
      ret.Add("X_Length", xLength);
      ret.Add("Y_Length", yLength);

      Autodesk.AdvanceSteel.ConstructionHelper.Camera.eXYExtents extentsType;
      using (var ctx = new SteelServices.DocContext())
      {
        string handle = steelObject.Handle;
        FilerObject obj = Utils.GetObject(handle);

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kCamera))
        {
          Autodesk.AdvanceSteel.ConstructionHelper.Camera camera = obj as Autodesk.AdvanceSteel.ConstructionHelper.Camera;
          camera.getXYExtents(out extentsType, out xLength, out yLength);
          if (yLength >= 0)
          {
            ret["X_Length"] = Utils.FromInternalUnits(xLength, true);
            ret["Y_Length"] = Utils.FromInternalUnits(yLength, true);
          }
        }
        else
          throw new System.Exception("Failed to Get Camera Object");
      }
      return ret;
    }

    /// <summary>
    /// Get Camera Clipping Values
    /// </summary>
    /// <param name="steelObject"> Selected Advance Steel Camera Object</param>
    /// <returns></returns>
    [MultiReturn(new[] { "Nearside_Clipping", "Farside_Clipping" })]
    public static Dictionary<string, double> GetCameraClipping(SteelDbObject steelObject)
    {
      Dictionary<string, double> ret = new Dictionary<string, double>();

      double nearSideClippingValue = 0;
      double farSideClippingValue = 0;
      ret.Add("Nearside_Clipping", nearSideClippingValue);
      ret.Add("Farside_Clipping", farSideClippingValue);

      Autodesk.AdvanceSteel.ConstructionHelper.eZClip clipType;
      using (var ctx = new SteelServices.DocContext())
      {
        string handle = steelObject.Handle;
        FilerObject obj = Utils.GetObject(handle);

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kCamera))
        {
          Autodesk.AdvanceSteel.ConstructionHelper.Camera camera = obj as Autodesk.AdvanceSteel.ConstructionHelper.Camera;
          camera.getZClipping(out clipType, out nearSideClippingValue, out farSideClippingValue);
          if (nearSideClippingValue >= 0)
          {
            ret["Nearside_Clipping"] = Utils.FromInternalUnits(nearSideClippingValue, true);
            ret["Farside_Clipping"] = Utils.FromInternalUnits(farSideClippingValue, true);
          }
        }
        else
          throw new System.Exception("Failed to Get Camera Object");
      }
      return ret;
    }
  }
}

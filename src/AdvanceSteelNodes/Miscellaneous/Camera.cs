using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Miscellaneous
{
  /// <summary>
  /// Advance Steel camera
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class Camera : GraphicObject
  {
    internal Camera()
    {
    }

    internal Camera(List<Property> cameraProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {

          List<Property> defaultData = cameraProperties.Where(x => x.Level == ".").ToList<Property>();
          List<Property> postWriteDBData = cameraProperties.Where(x => x.Level == "Z_PostWriteDB").ToList<Property>();

          Matrix3d cameraMat = (Matrix3d)defaultData.FirstOrDefault<Property>(x => x.Name == "CameraCS").InternalValue;

          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Autodesk.AdvanceSteel.ConstructionHelper.Camera camera = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            camera = new Autodesk.AdvanceSteel.ConstructionHelper.Camera(cameraMat);
            if (defaultData != null)
            {
              Utils.SetParameters(camera, defaultData);
            }

            camera.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(camera, postWriteDBData);
            }

          }
          else
          {
            camera = Utils.GetObject(handle) as Autodesk.AdvanceSteel.ConstructionHelper.Camera;

            if (camera != null && camera.IsKindOf(FilerObject.eObjectType.kCamera))
            {
              if (defaultData != null)
              {
                Utils.SetParameters(camera, defaultData);
              }

              if (postWriteDBData != null)
              {
                Utils.SetParameters(camera, postWriteDBData);
              }

            }
            else
              throw new System.Exception("Not a Camera");
          }

          Handle = camera.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(camera);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel Camera
    /// </summary>
    /// <param name="coordinateSystem">Input Dynamo Coordinate System</param>
    /// <param name="additionalCameraParameters"> Optional Input Camera Build Properties </param>
    /// <returns></returns>
    public static Camera ByCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                  [DefaultArgument("null")] List<Property> additionalCameraParameters)
    {
      Matrix3d cameraMat = Utils.ToAstMatrix3d(coordinateSystem, true);
      additionalCameraParameters = PreSetDefaults(additionalCameraParameters, cameraMat);

      return new Camera(additionalCameraParameters);
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
                              Utils.ToInternalDistanceUnits(upperClippingValue, true),
                              Utils.ToInternalDistanceUnits(lowerClippingValue, true));
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
                              Utils.ToInternalDistanceUnits(xCameraSize, true),
                              Utils.ToInternalDistanceUnits(yCameraSize, true));
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
            ret["X_Length"] = Utils.FromInternalDistanceUnits(xLength, true);
            ret["Y_Length"] = Utils.FromInternalDistanceUnits(yLength, true);
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
            ret["Nearside_Clipping"] = Utils.FromInternalDistanceUnits(nearSideClippingValue, true);
            ret["Farside_Clipping"] = Utils.FromInternalDistanceUnits(farSideClippingValue, true);
          }
        }
        else
          throw new System.Exception("Failed to Get Camera Object");
      }
      return ret;
    }

    private static List<Property> PreSetDefaults(List<Property> listCameraData, Matrix3d cameraCS)
    {
      if (listCameraData == null)
      {
        listCameraData = new List<Property>() { };
      }
      Utils.CheckListUpdateOrAddValue(listCameraData, "CameraCS", cameraCS, ".");
      return listCameraData;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var camera = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.ConstructionHelper.Camera;

          Matrix3d cameraCS = camera.CameraCS;
          Vector3d xVect = null;
          Vector3d yVect = null;
          Vector3d ZVect = null;
          Point3d origin = null;
          cameraCS.GetCoordSystem(out origin, out xVect, out yVect, out ZVect);
          var cameraPoint = Utils.ToDynPoint(origin, true);
          var p1 = origin + (xVect * -100);
          p1 = p1 + (yVect * 100);
          var p2 = p1 + (xVect * 200);
          var p4 = p1 + (yVect * -200);
          var p3 = p4 + (xVect * 200);

          List<Point3d> lstPoints = new List<Point3d>() { p1, p2, p3, p4 };

          IEnumerable<Autodesk.DesignScript.Geometry.Point> dynPoints = Utils.ToDynPoints(lstPoints.ToArray(), true);
          var poly = Autodesk.DesignScript.Geometry.Polygon.ByPoints(dynPoints, true);
          foreach (var pt in dynPoints) { pt.Dispose(); }

          return poly;
        }
      }
    }
  }
}
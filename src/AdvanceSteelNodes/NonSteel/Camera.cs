using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.NonSteelItems
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

    internal Camera(List<ASProperty> cameraProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {

          List<ASProperty> defaultData = cameraProperties.Where(x => x.PropLevel == ".").ToList<ASProperty>();
          List<ASProperty> postWriteDBData = cameraProperties.Where(x => x.PropLevel == "Z_PostWriteDB").ToList<ASProperty>();

          Matrix3d cameraMat = (Matrix3d)defaultData.FirstOrDefault<ASProperty>(x => x.PropName == "CameraCS").PropValue;

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
                                  [DefaultArgument("null")] List<ASProperty> additionalCameraParameters)
    {
      Matrix3d cameraMat = Utils.ToAstMatrix3d(coordinateSystem, true);
      additionalCameraParameters = PreSetDefaults(additionalCameraParameters, cameraMat);

      return new Camera(additionalCameraParameters);
    }

    private static List<ASProperty> PreSetDefaults(List<ASProperty> listCameraData, Matrix3d cameraCS)
    {
      if (listCameraData == null)
      {
        listCameraData = new List<ASProperty>() { };
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
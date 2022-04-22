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
using System;
using ASWeldPoint = Autodesk.AdvanceSteel.Modelling.WeldPoint;

namespace AdvanceSteel.Nodes.ConnectionObjects.Welds
{
  /// <summary>
  /// Advance Steel Weld Point, including Onsite v InShop Connection Type
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class WeldPoint : GraphicObject
  {
    private WeldPoint(SteelGeometry.Point3d astPoint, IEnumerable<string> handlesToConnect, int connectionType)
    {
      SafeInit(() => InitWeldPoint(astPoint, handlesToConnect, connectionType));
    }

    private WeldPoint(ASWeldPoint weld)
    {
      SafeInit(() => SetHandle(weld));
    }

    internal static WeldPoint FromExisting(ASWeldPoint weld)
    {
      return new WeldPoint(weld)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitWeldPoint(SteelGeometry.Point3d astPoint, IEnumerable<string> handlesToConnect, int connectionType)
    {
      ASWeldPoint weld = SteelServices.ElementBinder.GetObjectASFromTrace<ASWeldPoint>();
      if (weld == null)
      {
        weld = new ASWeldPoint(astPoint, Vector3d.kXAxis, Vector3d.kYAxis);
        weld.WriteToDb();
      }
      else
      {
        if (weld != null && weld.IsKindOf(FilerObject.eObjectType.kWeldPattern))
        {
          Matrix3d coordinateSystem = new Matrix3d();
          coordinateSystem.SetCoordSystem(astPoint, Vector3d.kXAxis, Vector3d.kYAxis, Vector3d.kZAxis);
          weld.SetCS(coordinateSystem);
        }
        else
          throw new System.Exception("Not a weld point");
      }

      SetHandle(weld);

      FilerObject[] filerObjects = Utils.GetFilerObjects(handlesToConnect);
      weld.Connect(filerObjects, (AtomicElement.eAssemblyLocation)connectionType);

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(weld);
    }

    /// <summary>
    /// Create an Advance Steel Weld Pattern By Point
    /// </summary>
    /// <param name="point"> Input Weld Point Location</param>
    /// <param name="objectsToConnect"> Input Connected Objects</param>
    /// <param name="connectionType"> Input Weld Type - 0-OnSite or 2-InShop</param>
    /// <returns name="weldPoint"> weldPoint</returns>
    public static WeldPoint ByPoint(DynGeometry.Point point,
                                    IEnumerable<SteelDbObject> objectsToConnect,
                                    [DefaultArgument("2;")] int connectionType)
    {

      List<string> handlesList = new List<string>();
      handlesList = Utils.GetSteelDbObjectsToConnect(objectsToConnect);

      var astPoint = Utils.ToAstPoint(point, true);
      return new WeldPoint(astPoint, handlesList, connectionType);
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      var weld = Utils.GetObject(Handle) as ASWeldPoint;

      if (weld == null)
      {
        throw new Exception("Null weld point");
      }

      using (var dynPoint = Utils.ToDynPoint(weld.CenterPoint, true))
      {
        return Autodesk.DesignScript.Geometry.Circle.ByCenterPointRadius(dynPoint, 0.01);
      }
    }

  }
}



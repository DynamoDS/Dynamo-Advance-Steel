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
using ASWeldLine = Autodesk.AdvanceSteel.Modelling.WeldLine;

namespace AdvanceSteel.Nodes.ConnectionObjects.Welds
{
  /// <summary>
  /// Advance Steel Weld Line, including Onsite v InShop Connection Type
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class WeldLine : GraphicObject
  {
    private WeldLine(SteelGeometry.Point3d[] astPoints, IEnumerable<string> handlesToConnect, int connectionType, bool isClosed = false)
    {
      SafeInit(() => InitWeldLine(astPoints, handlesToConnect, connectionType, isClosed));
    }

    private WeldLine(ASWeldLine weld)
    {
      SafeInit(() => SetHandle(weld));
    }

    internal static WeldLine FromExisting(ASWeldLine weld)
    {
      return new WeldLine(weld)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitWeldLine(SteelGeometry.Point3d[] astPoints, IEnumerable<string> handlesToConnect, int connectionType, bool isClosed = false)
    {
      ASWeldLine weld = SteelServices.ElementBinder.GetObjectASFromTrace<ASWeldLine>();
      weld?.DelFromDb();

      weld = new ASWeldLine(astPoints, Vector3d.kXAxis, Vector3d.kYAxis);
      weld.IsClosed = isClosed;
      weld.WriteToDb();

      weld.Connect(Utils.GetSteelObjectsToConnect(handlesToConnect), (AtomicElement.eAssemblyLocation)connectionType);

      SetHandle(weld);
      SteelServices.ElementBinder.CleanupAndSetElementForTrace(weld);
    }

    /// <summary>
    /// Create an Advance Steel Weld Line By PolyCurve
    /// </summary>
    /// <param name="polyCurve"> Input Weld PolyCurve</param>
    /// <param name="objectsToConnect"> Input Weld Connected Objects</param>
    /// <param name="connectionType"> Input Weld Type - 0-OnSite or 2-InShop</param>
    /// <returns name="weldLine"> weldLine</returns>
    public static WeldLine ByPolyCurve(DynGeometry.PolyCurve polyCurve,
                                        IEnumerable<SteelDbObject> objectsToConnect,
                                        [DefaultArgument("2;")] int connectionType)
    {
      List<string> handlesList = Utils.GetSteelDbObjectsToConnect(objectsToConnect);

      var temp = polyCurve.Curves();
      SteelGeometry.Point3d[] astArr = new SteelGeometry.Point3d[temp.Length + 1];
      for (int i = 0; i < temp.Length; i++)
      {
        Point3d startPoint = Utils.ToAstPoint(temp[i].StartPoint, true);
        astArr[i] = startPoint;
      }

      Point3d endPoint = Utils.ToAstPoint(temp[temp.Length - 1].EndPoint, true);
      astArr[temp.Length] = endPoint;

      return new WeldLine(astArr, handlesList, connectionType, polyCurve.IsClosed);
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      var weld = Utils.GetObject(Handle) as ASWeldLine;

      if (weld == null)
        throw new Exception("Null weld line");

      weld.GetWeldPoints(out Point3d[] arrPoints, Autodesk.AdvanceSteel.Modelling.WeldPattern.eSeamPosition.kUpper);
      DynGeometry.Point[] dynPoints = Utils.ToDynPoints(arrPoints, true);
      return Autodesk.DesignScript.Geometry.PolyCurve.ByPoints(new HashSet<DynGeometry.Point>(dynPoints), weld.IsClosed);
    }

  }
}



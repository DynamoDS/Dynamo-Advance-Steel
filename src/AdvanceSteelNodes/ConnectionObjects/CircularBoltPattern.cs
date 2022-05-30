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
using ASCircleScrewBoltPattern = Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern;

namespace AdvanceSteel.Nodes.ConnectionObjects.Bolts
{
  /// <summary>
  /// Advance Steel Circular Bolt Pattern
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class CircularBoltPattern : GraphicObject
  {
    private CircularBoltPattern(SteelGeometry.Point3d holeInsertPoint, IEnumerable<string> handlesToConnect,
                                  SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy,
                                  List<Property> boltData,
                                  int boltCon)
    {
      SafeInit(() => InitCircularBoltPattern(holeInsertPoint, handlesToConnect, vx, vy, boltData, boltCon));
    }

    private CircularBoltPattern(ASCircleScrewBoltPattern bolt)
    {
      SafeInit(() => SetHandle(bolt));
    }

    internal static CircularBoltPattern FromExisting(ASCircleScrewBoltPattern bolt)
    {
      return new CircularBoltPattern(bolt)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitCircularBoltPattern(SteelGeometry.Point3d holeInsertPoint, IEnumerable<string> handlesToConnect,
                                  SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy,
                                  List<Property> boltData,
                                  int boltCon)
    {
      ASCircleScrewBoltPattern bolt = SteelServices.ElementBinder.GetObjectASFromTrace<ASCircleScrewBoltPattern>();
      if (bolt == null)
      {
        bolt = new ASCircleScrewBoltPattern(holeInsertPoint, vx, vy);

        UtilsProperties.SetParameters(bolt, boltData);

        bolt.WriteToDb();
      }
      else
      {
        if (!bolt.IsKindOf(FilerObject.eObjectType.kCircleScrewBoltPattern))
          throw new System.Exception("Not a circular pattern");

        bolt.RefPoint = holeInsertPoint;
        bolt.XDirection = vx;
        bolt.YDirection = vy;

        UtilsProperties.SetParameters(bolt, boltData);
      }

      SetHandle(bolt);

      FilerObject[] filerObjects = Utils.GetFilerObjects(handlesToConnect);
      bolt.Connect(filerObjects, (AtomicElement.eAssemblyLocation)boltCon);

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(bolt);
    }

    /// <summary>
    /// Create an Advance Steel Circular Bolt Pattern By Circle
    /// </summary>
    /// <param name="circle"> Input circle</param>
    /// <param name="referenceVector"> Input Dynamo Vector for alignment of circle</param>
    /// <param name="objectsToConnect"> Input Objects to be bolted </param>
    /// <param name="boltConnectionType"> Input Bolt Connection type - Shop Bolt Default</param>
    /// <param name="additionalBoltParameters"> Optional Input Bolt Build Properties </param>
    /// <returns name="circularBoltPattern"> bolts</returns>
    public static CircularBoltPattern ByCircle(DynGeometry.Circle circle,
                                                DynGeometry.Vector referenceVector,
                                                IEnumerable<SteelDbObject> objectsToConnect,
                                                [DefaultArgument("2;")] int boltConnectionType,
                                                [DefaultArgument("null")] List<Property> additionalBoltParameters)
    {
      var norm = Utils.ToAstVector3d(circle.Normal, true);
      var vx = Utils.ToAstVector3d(referenceVector, true);
      var vy = norm.CrossProduct(vx);

      vx = vx.Normalize();
      vy = vy.Normalize();

      additionalBoltParameters = PreSetCircularValuesInListProps(additionalBoltParameters, Utils.ToInternalDistanceUnits(circle.Radius, true));

      List<string> handlesList = Utils.GetSteelDbObjectsToConnect(objectsToConnect);
      return new CircularBoltPattern(Utils.ToAstPoint(circle.CenterPoint, true), handlesList, vx, vy, additionalBoltParameters, boltConnectionType);
    }

    /// <summary>
    /// Create an Advance Steel Circular Bolt Pattern By Center Point Radius Normal
    /// </summary>
    /// <param name="point"> Input radius center point</param>
    /// <param name="boltCS"> Input Coordinate System </param>
    /// <param name="objectsToConnect"> Input Objects to be bolted </param>
    /// <param name="boltConnectionType"> Input Bolt Connection type - Shop Bolt Default</param>
    /// <param name="additionalBoltParameters"> Optional Input Bolt Build Properties </param>
    /// <returns name="circularBoltPattern"> bolts</returns>
    public static CircularBoltPattern AtCentrePoint(DynGeometry.Point point,
                                                    DynGeometry.CoordinateSystem boltCS,
                                                    IEnumerable<SteelDbObject> objectsToConnect,
                                                    [DefaultArgument("2;")] int boltConnectionType,
                                                    [DefaultArgument("null")] List<Property> additionalBoltParameters)
    {
      SteelGeometry.Point3d astPointRef = Utils.ToAstPoint(point, true);

      var vx = Utils.ToAstVector3d(boltCS.XAxis, true);
      var vy = Utils.ToAstVector3d(boltCS.YAxis, true);

      vx = vx.Normalize();
      vy = vy.Normalize();

      IEnumerable<string> handles = Utils.GetSteelDbObjectsToConnect(objectsToConnect);
      return new CircularBoltPattern(Utils.ToAstPoint(point, true), handles, vx, vy, additionalBoltParameters, boltConnectionType);
    }

    private static List<Property> PreSetCircularValuesInListProps(List<Property> listOfBoltParameters, double radius)
    {
      if (listOfBoltParameters == null)
      {
        listOfBoltParameters = new List<Property>() { };
      }

      UtilsProperties.CheckListUpdateOrAddValue(typeof(ASCircleScrewBoltPattern), listOfBoltParameters, nameof(ASCircleScrewBoltPattern.Radius), radius);

      return listOfBoltParameters;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      var boltPattern = Utils.GetObject(Handle) as ASCircleScrewBoltPattern;
      if (boltPattern == null)
      {
        throw new Exception("Null bolt pattern");
      }

      using (var point = Utils.ToDynPoint(boltPattern.RefPoint, true))
      using (var norm = Utils.ToDynVector(boltPattern.Normal, true))
      {
        return Autodesk.DesignScript.Geometry.Circle.ByCenterPointRadiusNormal(point, Utils.FromInternalDistanceUnits(boltPattern.Radius, true), norm);
      }
    }

  }
}
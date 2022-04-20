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
using ASAnchorPattern = Autodesk.AdvanceSteel.Modelling.AnchorPattern;

namespace AdvanceSteel.Nodes.ConnectionObjects.Anchors
{
  /// <summary>
  /// Advance Steel Circular Anchor Pattern
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class CircularAnchorPattern : GraphicObject
  {
    private CircularAnchorPattern(SteelGeometry.Point3d astPointRef, IEnumerable<string> handlesToConnect,
                                   SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy,
                                   List<Property> anchorBoltData, int boltCon)
    {
      SafeInit(() => InitCircularAnchorPattern(astPointRef, handlesToConnect, vx, vy, anchorBoltData, boltCon));
    }

    private CircularAnchorPattern(ASAnchorPattern anchors)
    {
      SafeInit(() => SetHandle(anchors));
    }

    internal static CircularAnchorPattern FromExisting(ASAnchorPattern anchors)
    {
      return new CircularAnchorPattern(anchors)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitCircularAnchorPattern(SteelGeometry.Point3d astPointRef, IEnumerable<string> handlesToConnect,
                                   SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy,
                                   List<Property> anchorBoltData, int boltCon)
    {
      ASAnchorPattern anchors = SteelServices.ElementBinder.GetObjectASFromTrace<ASAnchorPattern>();
      if (anchors == null)
      {
        anchors = new ASAnchorPattern(astPointRef, vx, vy);
        SetAnchorSetOutDetails(anchors, astPointRef, Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kCircle);
        Utils.SetParameters(anchors, anchorBoltData);
        anchors.WriteToDb();
      }
      else
      {
        if (anchors != null && anchors.IsKindOf(FilerObject.eObjectType.kAnchorPattern))
        {
          SetAnchorSetOutDetails(anchors, astPointRef, Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kCircle);
          anchors.XDirection = vx;
          anchors.YDirection = vy;
          Utils.SetParameters(anchors, anchorBoltData);
        }
        else
          throw new System.Exception("Not a circular pattern");
      }

      SetHandle(anchors);

      FilerObject[] filerObjects = Utils.GetFilerObjects(handlesToConnect);
      anchors.Connect(filerObjects, (AtomicElement.eAssemblyLocation)boltCon);

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(anchors);
    }

    /// <summary>
    /// Create an Advance Steel Circular Anchor Pattern By Circle
    /// </summary>
    /// <param name="circle"> Input circle</param>
    /// <param name="referenceVector"> Input Dynamo Vector for alignment of circle</param>
    /// <param name="objectsToConnect"> Input Objects to be bolted </param>
    /// <param name="anchorBoltConnectionType"> Input Bolt Connection type - Shop Bolt Default</param>
    /// <param name="additionalAnchorBoltParameters"> Optional Input Bolt Build Properties </param>
    /// <returns name="circularAnchorPattern"> anchor</returns>
    public static CircularAnchorPattern ByCircle(DynGeometry.Circle circle,
                                                  DynGeometry.Vector referenceVector,
                                                  IEnumerable<SteelDbObject> objectsToConnect,
                                                  [DefaultArgument("2;")] int anchorBoltConnectionType,
                                                  [DefaultArgument("null")] List<Property> additionalAnchorBoltParameters)
    {
      var norm = Utils.ToAstVector3d(circle.Normal, true);
      var vx = Utils.ToAstVector3d(referenceVector, true);
      var vy = norm.CrossProduct(vx);

      vx = vx.Normalize();
      vy = vy.Normalize();

      List<string> handlesList = Utils.GetSteelDbObjectsToConnect(objectsToConnect);
      additionalAnchorBoltParameters = PreSetValuesInListProps(additionalAnchorBoltParameters, Utils.ToInternalDistanceUnits(circle.Radius, true));
      return new CircularAnchorPattern(Utils.ToAstPoint(circle.CenterPoint, true), handlesList, vx, vy, additionalAnchorBoltParameters, anchorBoltConnectionType);
    }

    /// <summary>
    /// Create an Advance Steel Circular Anchor Pattern By Center Point and Dynamo Coordinate System
    /// </summary>
    /// <param name="point"> Input radius center point</param>
    /// <param name="anchorCS"> Input Dynamo Coordinate System</param>
    /// <param name="objectsToConnect"> Input Objects to be bolted </param>
    /// <param name="anchorBoltConnectionType"> Input Bolt Connection type - Shop Bolt Default</param>
    /// <param name="additionalAnchorBoltParameters"> Optional Input Bolt Build Properties </param>
    /// <returns name="circularAnchorPattern"> anchor</returns>
    public static CircularAnchorPattern AtCentrePoint(DynGeometry.Point point,
                                                      DynGeometry.CoordinateSystem anchorCS,
                                                      IEnumerable<SteelDbObject> objectsToConnect,
                                                      [DefaultArgument("2;")] int anchorBoltConnectionType,
                                                      [DefaultArgument("null")] List<Property> additionalAnchorBoltParameters)
    {
      SteelGeometry.Point3d astPointRef = Utils.ToAstPoint(point, true);

      var vx = Utils.ToAstVector3d(anchorCS.XAxis, true);
      var vy = Utils.ToAstVector3d(anchorCS.YAxis, true);
      vx = vx.Normalize();
      vy = vy.Normalize();

      IEnumerable<string> handles = Utils.GetSteelDbObjectsToConnect(objectsToConnect);
      
      return new CircularAnchorPattern(astPointRef, handles, vx, vy, additionalAnchorBoltParameters, anchorBoltConnectionType);
    }

    private static List<Property> PreSetValuesInListProps(List<Property> listOfAnchorBoltParameters, double radius)
    {
      if (listOfAnchorBoltParameters == null)
      {
        listOfAnchorBoltParameters = new List<Property>() { };
      }

      if (radius > 0 )
      {
        Utils.CheckListUpdateOrAddValue(listOfAnchorBoltParameters, "Radius", radius);
      }

      return listOfAnchorBoltParameters;
    }

    private void SetAnchorSetOutDetails(Autodesk.AdvanceSteel.Modelling.AnchorPattern anchors,
                                Point3d RefPoint,
                                Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType anchorArr)
    {
      anchors.ArrangerType = anchorArr;
      anchors.BoundedSide = Autodesk.AdvanceSteel.Modelling.AnchorPattern.eBoundedSide.kAll;
      anchors.RefPoint = RefPoint;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      var anchorPattern = Utils.GetObject(Handle) as ASAnchorPattern;
      if (anchorPattern == null)
      {
        throw new Exception("Null anchor pattern");
      }

      using (var point = Utils.ToDynPoint(anchorPattern.RefPoint, true))
      using (var norm = Utils.ToDynVector(anchorPattern.Normal, true))
      {
        return Autodesk.DesignScript.Geometry.Circle.ByCenterPointRadiusNormal(point, Utils.FromInternalDistanceUnits(anchorPattern.Radius, true), norm);
      }
    }

  }
}
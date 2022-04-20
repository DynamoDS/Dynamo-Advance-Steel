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
  /// Advance Steel Rectangular Anchor Pattern
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class RectangularAnchorPattern : GraphicObject
  {
    private RectangularAnchorPattern(SteelGeometry.Point3d anchorBoltPatternInsertPoint, IEnumerable<string> handlesToConnect,
                                      SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy,
                                      List<Property> anchorBoltData, int boltCon)
    {
      SafeInit(() => InitRectangularAnchorPattern(anchorBoltPatternInsertPoint, handlesToConnect, vx, vy, anchorBoltData, boltCon));
    }

    private RectangularAnchorPattern(ASAnchorPattern anchors)
    {
      SafeInit(() => SetHandle(anchors));
    }

    internal static RectangularAnchorPattern FromExisting(ASAnchorPattern anchors)
    {
      return new RectangularAnchorPattern(anchors)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitRectangularAnchorPattern(SteelGeometry.Point3d anchorBoltPatternInsertPoint, IEnumerable<string> handlesToConnect,
                                      SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy,
                                      List<Property> anchorBoltData, int boltCon)
    {
      ASAnchorPattern anchors = SteelServices.ElementBinder.GetObjectASFromTrace<ASAnchorPattern>();
      if (anchors == null)
      {
        anchors = new ASAnchorPattern(anchorBoltPatternInsertPoint, vx, vy);
        SetAnchorSetOutDetails(anchors, anchorBoltPatternInsertPoint, Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kRectangular);
        Utils.SetParameters(anchors, anchorBoltData);

        anchors.WriteToDb();
      }
      else
      {
        if (anchors != null && anchors.IsKindOf(FilerObject.eObjectType.kAnchorPattern))
        {
          SetAnchorSetOutDetails(anchors, anchorBoltPatternInsertPoint, Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kRectangular);
          Utils.SetParameters(anchors, anchorBoltData);
        }
        else
          throw new System.Exception("Not an anchor pattern");
      }

      SetHandle(anchors);

      FilerObject[] filerObjects = Utils.GetFilerObjects(handlesToConnect);
      anchors.Connect(filerObjects, (AtomicElement.eAssemblyLocation)boltCon);

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(anchors);
    }

    internal RectangularAnchorPattern(SteelGeometry.Point3d astPoint1, SteelGeometry.Point3d astPoint2, IEnumerable<string> handlesToConnect,
                                      SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy,
                                      List<Property> anchorBoltData, int boltCon)
    {
      var astPointRef = astPoint1 + (astPoint2 - astPoint1) * 0.5;

      ASAnchorPattern anchors = SteelServices.ElementBinder.GetObjectASFromTrace<ASAnchorPattern>();
      if (anchors == null)
      {
        anchors = new ASAnchorPattern(astPoint1, astPoint2, vx, vy);
        SetAnchorSetOutDetails(anchors, astPoint1 + (astPoint2 - astPoint1) * 0.5, Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kBounded);

        double aLengthX = Utils.GetRectangleLength(astPoint1, astPoint2, vx);// / (anchors.Nx - 1);
        double aLengthY = Utils.GetRectangleLength(astPoint1, astPoint2, vy);// / (anchors.Ny - 1);
        anchors.SetArrangerLength(aLengthX, 0);
        anchors.SetArrangerLength(aLengthY, 1);

        Utils.SetParameters(anchors, anchorBoltData);

        anchors.WriteToDb();
      }
      else
      {
        if (anchors != null && anchors.IsKindOf(FilerObject.eObjectType.kAnchorPattern))
        {
          SetAnchorSetOutDetails(anchors, astPoint1 + (astPoint2 - astPoint1) * 0.5, Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kBounded);

          double aLengthX = Utils.GetRectangleLength(astPoint1, astPoint2, vx);
          double aLengthY = Utils.GetRectangleLength(astPoint1, astPoint2, vy);
          anchors.SetArrangerLength(aLengthX, 0);
          anchors.SetArrangerLength(aLengthY, 1);

          Utils.SetParameters(anchors, anchorBoltData);

        }
        else
          throw new System.Exception("Not an anchor pattern");
      }

      SetHandle(anchors);

      FilerObject[] filerObjects = Utils.GetFilerObjects(handlesToConnect);
      anchors.Connect(filerObjects, (AtomicElement.eAssemblyLocation)boltCon);

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(anchors);
    }

    /// <summary>
    /// Create an Advance Steel Rectangular Anchor Pattern bound By Rectangle
    /// </summary>
    /// <param name="rectangle"> Input rectangle </param>
    /// <param name="noOfBoltsX"> Input No of Anchor Bolts in the X direction</param>
    /// <param name="noOfBoltsY"> Input No of Anchor Bolts in the Y direction</param>
    /// <param name="objectsToConnect"> Input Objects to be bolted </param>
    /// <param name="boltConnectionType"> Input Bolt Connection type - Shop Bolt Default</param>
    /// <param name="additionalAnchorBoltParameters"> Optional Input Anchor Bolt Build Properties </param>
    /// <returns name="rectangularAnchorPattern"> rectangularAnchorPattern</returns>
    public static RectangularAnchorPattern ByRectangle(Autodesk.DesignScript.Geometry.Rectangle rectangle,
                                                      [DefaultArgument("2;")] int noOfBoltsX,
                                                      [DefaultArgument("2;")] int noOfBoltsY,
                                                      IEnumerable<SteelDbObject> objectsToConnect,
                                                      [DefaultArgument("2;")] int boltConnectionType,
                                                      [DefaultArgument("null")] List<Property> additionalAnchorBoltParameters)
    {
      List<string> handlesList = Utils.GetSteelDbObjectsToConnect(objectsToConnect);

      var dynCorners = rectangle.Corners();
      var astCorners = Utils.ToAstPoints(dynCorners, true);
      var vx = astCorners[1] - astCorners[0];
      var vy = astCorners[3] - astCorners[0];

      additionalAnchorBoltParameters = PreSetValuesInListProps(additionalAnchorBoltParameters, noOfBoltsX, noOfBoltsY);

      return new RectangularAnchorPattern(astCorners[0], astCorners[2], handlesList, vx, vy, additionalAnchorBoltParameters, boltConnectionType);
    }

    /// <summary>
    /// Create an Advance Steel unbounded Rectangular Pattern at a Point
    /// </summary>
    /// <param name="connectionPoint"> Input Insertion point of Anchor Bolt Pattern </param>
    /// <param name="boltCS"> Input Anchor Bolt Coordinate System </param>
    /// <param name="noOfBoltsX"> Input No of Anchor Bolts in the X direction</param>
    /// <param name="noOfBoltsY"> Input No of Anchor Bolts in the Y direction</param>
    /// <param name="objectsToConnect"> Input Objects to be bolted </param>
    /// <param name="boltConnectionType"> Input Bolt Connection type - Shop Bolt Default</param>
    /// <param name="additionalAnchorBoltParameters"> Optional Input Anchor Bolt Build Properties </param>
    /// <returns name="rectangularAnchorPattern"> rectangularAnchorPattern</returns>
    public static RectangularAnchorPattern AtCentrePoint(Autodesk.DesignScript.Geometry.Point connectionPoint,
                                                  Autodesk.DesignScript.Geometry.CoordinateSystem boltCS,
                                                  [DefaultArgument("2;")] int noOfBoltsX,
                                                  [DefaultArgument("2;")] int noOfBoltsY,
                                                  IEnumerable<SteelDbObject> objectsToConnect,
                                                  [DefaultArgument("2;")] int boltConnectionType,
                                                  [DefaultArgument("null")] List<Property> additionalAnchorBoltParameters)
    {
      List<string> handlesList = Utils.GetSteelDbObjectsToConnect(objectsToConnect);

      var vx = Utils.ToAstVector3d(boltCS.XAxis, true);
      var vy = Utils.ToAstVector3d(boltCS.YAxis, true);

      additionalAnchorBoltParameters = PreSetValuesInListProps(additionalAnchorBoltParameters, noOfBoltsX, noOfBoltsY);

      return new RectangularAnchorPattern(Utils.ToAstPoint(connectionPoint, true), handlesList, vx, vy, additionalAnchorBoltParameters, boltConnectionType);
    }

    private static List<Property> PreSetValuesInListProps(List<Property> listOfAnchorBoltParameters, int nx, int ny)
    {
      if (listOfAnchorBoltParameters == null)
      {
        listOfAnchorBoltParameters = new List<Property>() { };
      }

      Utils.CheckListUpdateOrAddValue(listOfAnchorBoltParameters, "Nx", nx);
      Utils.CheckListUpdateOrAddValue(listOfAnchorBoltParameters, "Ny", ny);

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
    public override DynGeometry.Curve GetDynCurve()
    {
      var anchorPattern = Utils.GetObject(Handle) as ASAnchorPattern;

      if (anchorPattern == null)
      {
        throw new Exception("Null anchor pattern");
      }

      var temp1 = anchorPattern.XDirection * anchorPattern.Dx / 2.0;
      var temp2 = anchorPattern.YDirection * anchorPattern.Dy / 2.0;

      var pt1 = new SteelGeometry.Point3d(anchorPattern.RefPoint);
      pt1.Add(temp1 + temp2);

      var pt2 = new SteelGeometry.Point3d(anchorPattern.RefPoint);
      pt2.Add(temp1 - temp2);

      var pt3 = new SteelGeometry.Point3d(anchorPattern.RefPoint);
      pt3.Add(-temp1 - temp2);

      var pt4 = new SteelGeometry.Point3d(anchorPattern.RefPoint);
      pt4.Add(-temp1 + temp2);

      {
        List<DynGeometry.Point> polyPoints = new List<DynGeometry.Point>
            {
              Utils.ToDynPoint(pt1, true),
              Utils.ToDynPoint(pt2, true),
              Utils.ToDynPoint(pt3, true),
              Utils.ToDynPoint(pt4, true)
            };

        return Autodesk.DesignScript.Geometry.Polygon.ByPoints(polyPoints);
      }
    }

  }
}


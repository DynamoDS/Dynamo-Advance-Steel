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


namespace AdvanceSteel.Nodes.ConnectionObjects.Bolts
{
  /// <summary>
  /// Advance Steel Rectangular Bolt Pattern
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class RectangularBoltPattern : GraphicObject
  {
    internal RectangularBoltPattern()
    {
    }

    internal RectangularBoltPattern(SteelGeometry.Point3d boltPatternInsertPoint,
                                    SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy,
                                    IEnumerable<string> handlesToConnect,
                                    List<Property> boltData, int boltCon)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          Autodesk.AdvanceSteel.Modelling.InfinitMidScrewBoltPattern bolts = null;

          string handle = SteelServices.ElementBinder.GetHandleFromTrace();
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            bolts = new Autodesk.AdvanceSteel.Modelling.InfinitMidScrewBoltPattern(boltPatternInsertPoint, vx, vy);
            Utils.SetParameters(bolts, boltData);
            bolts.WriteToDb();
          }
          else
          {
            bolts = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.InfinitMidScrewBoltPattern;

            if (bolts != null && bolts.IsKindOf(FilerObject.eObjectType.kInfinitMidScrewBoltPattern))
            {
              Utils.SetParameters(bolts, boltData);
            }
            else
              throw new System.Exception("Not a rectangular pattern");
          }

          FilerObject[] filerObjects = Utils.GetSteelObjectsToConnect(handlesToConnect);
          bolts.Connect(filerObjects, (AtomicElement.eAssemblyLocation)boltCon);

          Handle = bolts.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(bolts);
        }
      }
    }

    internal RectangularBoltPattern(SteelGeometry.Point3d astPoint1, SteelGeometry.Point3d astPoint2,
                                            IEnumerable<string> handlesToConnect,
                                            SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy,
                                            List<Property> boltData, int boltCon)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern bolts = null;

          string handle = SteelServices.ElementBinder.GetHandleFromTrace();
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            bolts = new Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern(astPoint1, astPoint2, vx, vy);

            bolts.RefPoint = astPoint1 + (astPoint2 - astPoint1) * 0.5;
            bolts.Length = Utils.GetRectangleLength(astPoint1, astPoint2, vx);
            bolts.Height = Utils.GetRectangleHeight(astPoint1, astPoint2, vy);

            Utils.SetParameters(bolts, boltData);

            bolts.WriteToDb();
          }
          else
          {
            bolts = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern;

            if (bolts != null && bolts.IsKindOf(FilerObject.eObjectType.kFinitRectScrewBoltPattern))
            {
              bolts.XDirection = vx;
              bolts.YDirection = vy;

              bolts.RefPoint = astPoint1 + (astPoint2 - astPoint1) * 0.5;
              bolts.Length = Utils.GetRectangleLength(astPoint1, astPoint2, vx);
              bolts.Height = Utils.GetRectangleHeight(astPoint1, astPoint2, vy);

              Utils.SetParameters(bolts, boltData);

            }
            else
              throw new System.Exception("Not a rectangular pattern");
          }

          FilerObject[] filerObjects = Utils.GetSteelObjectsToConnect(handlesToConnect);
          bolts.Connect(filerObjects, (AtomicElement.eAssemblyLocation)boltCon);

          Handle = bolts.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(bolts);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel Rectangular Pattern By Rectangle
    /// </summary>
    /// <param name="rectangle"> Input rectangle </param>
    /// <param name="noOfBoltsX"> Input No of Bolts in the X direction</param>
    /// <param name="noOfBoltsY"> Input No of Bolts in the Y direction</param>
    /// <param name="objectsToConnect"> Objects to be bolted </param>
    /// <param name="boltConnectionType"> Input Bolt Connection type - Shop Bolt Default</param>
    /// <param name="additionalBoltParameters"> Optional Bolt Build Properties </param>
    /// <returns name="rectangularBoltPattern"> rectangularBoltPattern</returns>
    public static RectangularBoltPattern ByRectangle(Autodesk.DesignScript.Geometry.Rectangle rectangle,
                                                      [DefaultArgument("2;")] int noOfBoltsX,
                                                      [DefaultArgument("2;")] int noOfBoltsY,
                                                      IEnumerable<SteelDbObject> objectsToConnect,
                                                      [DefaultArgument("2;")] int boltConnectionType,
                                                      [DefaultArgument("null")] List<Property> additionalBoltParameters)
    {
      var norm = Utils.ToAstVector3d(rectangle.Normal, true);

      List<string> handlesList = new List<string>();
      handlesList = Utils.GetSteelDbObjectsToConnect(objectsToConnect);

      var dynCorners = rectangle.Corners();
      var astCorners = Utils.ToAstPoints(dynCorners, true);
      var vx = astCorners[1] - astCorners[0];
      var vy = astCorners[3] - astCorners[0];

      additionalBoltParameters = PreSetValuesInListProps(additionalBoltParameters, noOfBoltsX, noOfBoltsY);

      return new RectangularBoltPattern(astCorners[0], astCorners[2], handlesList, vx, vy, additionalBoltParameters, boltConnectionType);
    }

    /// <summary>
    /// Create an Advance Steel Rectangular Pattern By Rectangle
    /// </summary>
    /// <param name="connectionPoint"> Input Insertion point of Bolt Pattern </param>
    /// <param name="boltCS"> Input Bolt Coordinate System </param>
    /// <param name="noOfBoltsX"> Input No of Bolts in the X direction</param>
    /// <param name="noOfBoltsY"> Input No of Bolts in the Y direction</param>
    /// <param name="objectsToConnect"> Objects to be bolted </param>
    /// <param name="boltConnectionType"> Input Bolt Connection type - Shop Bolt Default</param>
    /// <param name="additionalBoltParameters"> Optional Bolt Build Properties </param>
    /// <returns name="rectangularBoltPattern"> rectangularBoltPattern</returns>
    public static RectangularBoltPattern AtCentrePoint(Autodesk.DesignScript.Geometry.Point connectionPoint,
                                                  Autodesk.DesignScript.Geometry.CoordinateSystem boltCS,
                                                  [DefaultArgument("2;")] int noOfBoltsX,
                                                  [DefaultArgument("2;")] int noOfBoltsY,
                                                  IEnumerable<SteelDbObject> objectsToConnect,
                                                  [DefaultArgument("2;")] int boltConnectionType,
                                                  [DefaultArgument("null")] List<Property> additionalBoltParameters)
    {
      List<string> handlesList = new List<string>();
      handlesList = Utils.GetSteelDbObjectsToConnect(objectsToConnect);

      var vx = Utils.ToAstVector3d(boltCS.XAxis, true);
      var vy = Utils.ToAstVector3d(boltCS.YAxis, true);

      additionalBoltParameters = PreSetValuesInListProps(additionalBoltParameters, noOfBoltsX, noOfBoltsY);

      return new RectangularBoltPattern(Utils.ToAstPoint(connectionPoint, true), vx, vy, handlesList, additionalBoltParameters, boltConnectionType);
    }

    private static List<Property> PreSetValuesInListProps(List<Property> listOfBoltParameters, int nx, int ny)
    {
      if (listOfBoltParameters == null)
      {
        listOfBoltParameters = new List<Property>() { };
      }

      Utils.CheckListUpdateOrAddValue(listOfBoltParameters, "Nx", nx);
      Utils.CheckListUpdateOrAddValue(listOfBoltParameters, "Ny", ny);

      return listOfBoltParameters;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var boltPattern = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.CountableScrewBoltPattern;

          if (boltPattern == null)
          {
            throw new Exception("Null bolt pattern");
          }

          var temp1 = boltPattern.XDirection * boltPattern.Length / 2.0;
          var temp2 = boltPattern.YDirection * boltPattern.Height / 2.0;

          var pt1 = new SteelGeometry.Point3d(boltPattern.CenterPoint);
          pt1.Add(temp1 + temp2);

          var pt2 = new SteelGeometry.Point3d(boltPattern.CenterPoint);
          pt2.Add(temp1 - temp2);

          var pt3 = new SteelGeometry.Point3d(boltPattern.CenterPoint);
          pt3.Add(-temp1 - temp2);

          var pt4 = new SteelGeometry.Point3d(boltPattern.CenterPoint);
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
  }
}

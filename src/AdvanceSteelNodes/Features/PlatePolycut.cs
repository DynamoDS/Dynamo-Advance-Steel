using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.Modelling;
using ASPlateFeatContour = Autodesk.AdvanceSteel.Modelling.PlateFeatContour;
using ASPlateContourNotch = Autodesk.AdvanceSteel.Modelling.PlateContourNotch;

namespace AdvanceSteel.Nodes.Features
{
  /// <summary>
  /// Advance Steel Polycut on object
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class PlatePolycut : GraphicObject
  {
    private PlatePolycut(AdvanceSteel.Nodes.SteelDbObject element,
                      double xOffset, double yOffset, int corner,
                      int cutShapeRectCircle,
                      List<Property> plateFeatureProperties)
    {
      SafeInit(() => InitPlatePolycut(element, xOffset, yOffset, corner, cutShapeRectCircle, plateFeatureProperties));
    }

    private PlatePolycut(AdvanceSteel.Nodes.SteelDbObject element,
                          Polyline3d cutPolyline,
                          Autodesk.AdvanceSteel.Geometry.Vector3d normal,
                          Autodesk.AdvanceSteel.Geometry.Vector3d lengthVector,
                          List<Property> plateFeatureProperties)
    {
      SafeInit(() => InitPlatePolycut(element, cutPolyline, normal, lengthVector, plateFeatureProperties));
    }

    private PlatePolycut(ASPlateFeatContour plateFeat)
    {
      SafeInit(() => SetHandle(plateFeat));
    }

    private PlatePolycut(ASPlateContourNotch plateFeat)
    {
      SafeInit(() => SetHandle(plateFeat));
    }

    internal static PlatePolycut FromExisting(ASPlateFeatContour plateFeat)
    {
      return new PlatePolycut(plateFeat)
      {
        IsOwnedByDynamo = false
      };
    }

    internal static PlatePolycut FromExisting(ASPlateContourNotch plateFeat)
    {
      return new PlatePolycut(plateFeat)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitPlatePolycut(AdvanceSteel.Nodes.SteelDbObject element,
                      double xOffset, double yOffset, int corner,
                      int cutShapeRectCircle,
                      List<Property> plateFeatureProperties)
    {
      List<Property> defaultData = plateFeatureProperties.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> postWriteDBData = plateFeatureProperties.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();

      double length = 0;
      double width = 0;
      double radius = 0;

      if (defaultData.FirstOrDefault<Property>(x => x.Name == "Length") != null)
      {
        length = (double)defaultData.FirstOrDefault<Property>(x => x.Name == "Length").InternalValue;
      }
      if (defaultData.FirstOrDefault<Property>(x => x.Name == "Width") != null)
      {
        width = (double)defaultData.FirstOrDefault<Property>(x => x.Name == "Width").InternalValue;
      }
      if (defaultData.FirstOrDefault<Property>(x => x.Name == "Radius") != null)
      {
        radius = (double)defaultData.FirstOrDefault<Property>(x => x.Name == "Radius").InternalValue;
      }

      FilerObject obj = Utils.GetObject(element.Handle);
      if (obj == null || !(obj.IsKindOf(FilerObject.eObjectType.kPlate)))
        throw new System.Exception("No Input Element found");

      ASPlateFeatContour plateFeat = SteelServices.ElementBinder.GetObjectASFromTrace<ASPlateFeatContour>();
      if (plateFeat == null)
      {
        Matrix2d m2d = new Matrix2d();
        m2d.SetCoordSystem(new Point2d(xOffset, yOffset), new Vector2d(1, 0), new Vector2d(0, 1));
        switch (cutShapeRectCircle)
        {
          case 0:
            plateFeat = new PlateFeatContour(m2d, length, width);
            break;
          case 1:
            plateFeat = new PlateFeatContour(m2d, radius);
            break;
        }

        Vector2d offset;
        switch (corner)
        {
          case 0:  //Top Left
            offset = new Vector2d(-1, 1);
            break;
          case 1: //Top Right
            offset = new Vector2d(1, 1);
            break;
          case 2: //Bottom Right
            offset = new Vector2d(1, -1);
            break;
          case 3: //Bottom left
            offset = new Vector2d(-1, -1);
            break;
          default: //Anything else ignore
            offset = new Vector2d(0, 0);
            break;
        }
        plateFeat.Offset = offset;
        AtomicElement atomic = obj as AtomicElement;

        if (defaultData != null)
        {
          Utils.SetParameters(plateFeat, defaultData);
        }

        atomic.AddFeature(plateFeat);
      }
      else
      {
        if (!plateFeat.IsKindOf(FilerObject.eObjectType.kPlateFeatContour))
          throw new System.Exception("Not a Plate Feature");

        Plate plate = obj as Plate;
        plate.DelFeature(plateFeat);
        plate.WriteToDb();

        Matrix2d m2d = new Matrix2d();
        m2d.SetCoordSystem(new Point2d(xOffset, yOffset), new Vector2d(1, 0), new Vector2d(0, 1));
        switch (cutShapeRectCircle)
        {
          case 0:
            plateFeat = new PlateFeatContour(m2d, length, width);
            break;
          case 1:
            plateFeat = new PlateFeatContour(m2d, radius);
            break;
        }

        Vector2d offset;
        switch (corner)
        {
          case 0:  //Top Left
            offset = new Vector2d(-1, 1);
            break;
          case 1: //Top Right
            offset = new Vector2d(1, 1);
            break;
          case 2: //Bottom Right
            offset = new Vector2d(1, -1);
            break;
          case 3: //Bottom left
            offset = new Vector2d(-1, -1);
            break;
          default: //Anything else ignore
            offset = new Vector2d(0, 0);
            break;
        }
        plateFeat.Offset = offset;
        AtomicElement atomic = obj as AtomicElement;

        if (defaultData != null)
        {
          Utils.SetParameters(plateFeat, defaultData);
        }

        atomic.AddFeature(plateFeat);
      }

      SetHandle(plateFeat);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(plateFeat, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(plateFeat);
    }

    private void InitPlatePolycut(AdvanceSteel.Nodes.SteelDbObject element,
                          Polyline3d cutPolyline,
                          Autodesk.AdvanceSteel.Geometry.Vector3d normal,
                          Autodesk.AdvanceSteel.Geometry.Vector3d lengthVector,
                          List<Property> plateFeatureProperties)
    {
      List<Property> defaultData = plateFeatureProperties.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> postWriteDBData = plateFeatureProperties.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();

      FilerObject obj = Utils.GetObject(element.Handle);
      if (obj == null || !(obj.IsKindOf(FilerObject.eObjectType.kPlate)))
        throw new System.Exception("No Input Element found");

      ASPlateContourNotch plateFeat = SteelServices.ElementBinder.GetObjectASFromTrace<ASPlateContourNotch>();
      if (plateFeat == null)
      {
        Plate plate = obj as Plate;
        plateFeat = new ASPlateContourNotch(plate, 0, cutPolyline, normal, lengthVector);

        if (defaultData != null)
        {
          Utils.SetParameters(plateFeat, defaultData);
        }

        plate.AddFeature(plateFeat);
      }
      else
      {
        if (!plateFeat.IsKindOf(FilerObject.eObjectType.kPlateContourNotch))
          throw new System.Exception("Not a Plate Feature");

        Plate plate = obj as Plate;
        plate.DelFeature(plateFeat);
        plate.WriteToDb();

        plateFeat = new PlateContourNotch(plate, 0, cutPolyline, normal, lengthVector);
        AtomicElement atomic = obj as AtomicElement;

        if (defaultData != null)
        {
          Utils.SetParameters(plateFeat, defaultData);
        }

        atomic.AddFeature(plateFeat);
      }

      SetHandle(plateFeat);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(plateFeat, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(plateFeat);
    }

    /// <summary>
    /// Create an Advance Steel rectangular plate feature
    /// </summary>
    /// <param name="element"> Input Plate</param>
    /// <param name="length"> Input Length</param>
    /// <param name="width"> Input Width</param>
    /// <param name="xOffset"> Input X Offset from plate orgin</param>
    /// <param name="yOffset"> Input Y Offset from plate orgin</param>
    /// <param name="corner"> 0 - TL, 1 - TR, 2 - BR, 3 - BL, else center</param>
    /// <param name="additionalPlateFeatureParameters"> Optional Input Plate Build Properties </param>
    /// <returns name="polyCut">polyCut</returns>
    public static PlatePolycut ByLengthAndWidth(AdvanceSteel.Nodes.SteelDbObject element,
                                    double length,
                                    double width,
                                    [DefaultArgument("0")] double xOffset,
                                    [DefaultArgument("0")] double yOffset,
                                    [DefaultArgument("-1")] int corner,
                                    [DefaultArgument("null")] List<Property> additionalPlateFeatureParameters)
    {
      additionalPlateFeatureParameters = PreSetDefaults(additionalPlateFeatureParameters, Utils.ToInternalDistanceUnits(length, true), Utils.ToInternalDistanceUnits(width, true));
      return new PlatePolycut(element, Utils.ToInternalDistanceUnits(xOffset, true), Utils.ToInternalDistanceUnits(yOffset, true), corner, 0, additionalPlateFeatureParameters);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"> Input Plate</param>
    /// <param name="radius"> Input Radius</param>
    /// <param name="xOffset"> Input X Offset from plate orgin</param>
    /// <param name="yOffset"> Input Y Offset from plate orgin</param>
    /// <param name="corner"> 0 - TL, 1 - TR, 2 - BR, 3 - BL, else center</param>
    /// <param name="additionalPlateFeatureParameters"> Optional Input Plate Build Properties </param>
    /// <returns name="polyCut">polyCut</returns>
    public static PlatePolycut ByRadius(AdvanceSteel.Nodes.SteelDbObject element,
                                double radius,
                                [DefaultArgument("0")] double xOffset,
                                [DefaultArgument("0")] double yOffset,
                                [DefaultArgument("-1")] int corner,
                                [DefaultArgument("null")] List<Property> additionalPlateFeatureParameters)
    {
      additionalPlateFeatureParameters = PreSetDefaults(additionalPlateFeatureParameters, 0, 0, Utils.ToInternalDistanceUnits(radius, true));
      return new PlatePolycut(element, Utils.ToInternalDistanceUnits(xOffset, true), Utils.ToInternalDistanceUnits(yOffset, true), corner, 1, additionalPlateFeatureParameters);
    }

    /// <summary>
    /// Create an Advance Steel Polycut driven by Dynamo Curves on a Plate
    /// </summary>
    /// <param name="element"> Input Beam</param>
    /// <param name="curves"> Input Dynamo Curves referencing Clockwise in sequence to form a closed polyline</param>
    /// <param name="lengthVec"> Input vector in the length direction of rectangular polycut</param>
    /// <param name="additionalPlateFeatureParameters"> Optional Input Plate Notch Countour Build Properties </param>
    /// <returns name="polyCut">polyCut</returns>
    public static PlatePolycut FromListCurves(AdvanceSteel.Nodes.SteelDbObject element,
                                            List<Autodesk.DesignScript.Geometry.Curve> curves,
                                            Autodesk.DesignScript.Geometry.Vector lengthVec,
                                            [DefaultArgument("null")] List<Property> additionalPlateFeatureParameters)
    {

      Polyline3d curveCreatedPolyline = Utils.ToAstPolyline3d(curves, true);
      additionalPlateFeatureParameters = PreSetDefaults(additionalPlateFeatureParameters);
      return new PlatePolycut(element,
                        curveCreatedPolyline,
                        curveCreatedPolyline.Normal,
                        Utils.ToAstVector3d(lengthVec, true),
                        additionalPlateFeatureParameters);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"> Input Beam</param>
    /// <param name="polyCurve"> Input Dynamo PolyCurve Object</param>
    /// <param name="lengthVec"> Input vector in the length direction of rectangular polycut</param>
    /// <param name="additionalPlateFeatureParameters"> Optional Input Plate Notch Countour Build Properties </param>
    /// <returns name="polyCut">polyCut</returns>
    public static PlatePolycut FromPolyCurve(AdvanceSteel.Nodes.SteelDbObject element,
                                    Autodesk.DesignScript.Geometry.PolyCurve polyCurve,
                                    Autodesk.DesignScript.Geometry.Vector lengthVec,
                                    [DefaultArgument("null")] List<Property> additionalPlateFeatureParameters)
    {

      Polyline3d curveCreatedPolyline = Utils.ToAstPolyline3d(polyCurve, true);
      additionalPlateFeatureParameters = PreSetDefaults(additionalPlateFeatureParameters);
      return new PlatePolycut(element,
                        curveCreatedPolyline,
                        curveCreatedPolyline.Normal,
                        Utils.ToAstVector3d(lengthVec, true),
                        additionalPlateFeatureParameters);
    }

    private static List<Property> PreSetDefaults(List<Property> listPlateFeatureData, double length = 0, double width = 0, double radius = 0)
    {
      if (listPlateFeatureData == null)
      {
        listPlateFeatureData = new List<Property>() { };
      }
      if (length > 0) Utils.CheckListUpdateOrAddValue(listPlateFeatureData, "Length", length);
      if (width > 0) Utils.CheckListUpdateOrAddValue(listPlateFeatureData, "Width", width);
      if (radius > 0) Utils.CheckListUpdateOrAddValue(listPlateFeatureData, "Radius", radius);
      return listPlateFeatureData;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      Autodesk.DesignScript.Geometry.PolyCurve poly = null;
      FilerObject obj = Utils.GetObject(Handle);

      if (obj is ASPlateFeatContour)
      {
        var plateFeat = obj as ASPlateFeatContour;

        var dynPoints = Utils.ToDynPoints(plateFeat.GetContourPolygon(0), true);
        poly = Autodesk.DesignScript.Geometry.Polygon.ByPoints(dynPoints, true);
        foreach (var pt in dynPoints) { pt.Dispose(); }
      }
      else if (obj is ASPlateContourNotch)
      {
        var plateFeatx = obj as ASPlateContourNotch;
        //Autodesk.AdvanceSteel.Geometry.Matrix3d matrix = plateFeatx.CS;
        poly = Autodesk.DesignScript.Geometry.PolyCurve.ByJoinedCurves(Utils.ToDynPolyCurves(plateFeatx.GetPolygon(), true));
      }

      return poly;
    }

  }
}
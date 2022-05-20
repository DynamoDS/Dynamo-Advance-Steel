using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using ASBeamMultiContourNotch = Autodesk.AdvanceSteel.Modelling.BeamMultiContourNotch;

namespace AdvanceSteel.Nodes.Features
{
  /// <summary>
  /// Advance Steel Polycut on a beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class BeamPolycut : GraphicObject
  {
    private BeamPolycut(AdvanceSteel.Nodes.SteelDbObject element,
                      int cutShapeRectCircle,
                      Autodesk.AdvanceSteel.Geometry.Point3d insertPoint,
                      Autodesk.AdvanceSteel.Geometry.Vector3d normal,
                      Autodesk.AdvanceSteel.Geometry.Vector3d lengthVector,
                      int corner,
                      List<Property> beamFeatureProperties)
    {
      SafeInit(() => InitBeamPolycut(element, cutShapeRectCircle, insertPoint, normal, lengthVector, corner, beamFeatureProperties));
    }

    private BeamPolycut(AdvanceSteel.Nodes.SteelDbObject element,
                      Polyline3d cutPolyline,
                      Autodesk.AdvanceSteel.Geometry.Vector3d normal,
                      Autodesk.AdvanceSteel.Geometry.Vector3d lengthVector,
                      List<Property> beamFeatureProperties)
    {
      SafeInit(() => InitBeamPolycut(element, cutPolyline, normal, lengthVector, beamFeatureProperties));
    }

    private BeamPolycut(ASBeamMultiContourNotch beamFeat)
    {
      SafeInit(() => SetHandle(beamFeat));
    }

    internal static BeamPolycut FromExisting(ASBeamMultiContourNotch beamFeat)
    {
      return new BeamPolycut(beamFeat)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitBeamPolycut(AdvanceSteel.Nodes.SteelDbObject element,
                      int cutShapeRectCircle,
                      Autodesk.AdvanceSteel.Geometry.Point3d insertPoint,
                      Autodesk.AdvanceSteel.Geometry.Vector3d normal,
                      Autodesk.AdvanceSteel.Geometry.Vector3d lengthVector,
                      int corner,
                      List<Property> beamFeatureProperties)
    {
      List<Property> defaultData = beamFeatureProperties.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> postWriteDBData = beamFeatureProperties.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();

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
      if (obj == null || !(obj.IsKindOf(FilerObject.eObjectType.kBeam)))
        throw new System.Exception("No Input Element found");

      ASBeamMultiContourNotch beamFeat = SteelServices.ElementBinder.GetObjectASFromTrace<ASBeamMultiContourNotch>();
      if (beamFeat == null)
      {
        Beam bmObj = obj as Beam;
        switch (cutShapeRectCircle)
        {
          case 0:
            beamFeat = new ASBeamMultiContourNotch(bmObj, (Beam.eEnd)1, insertPoint, normal, lengthVector, length, width);
            break;
          case 1:
            beamFeat = new ASBeamMultiContourNotch(bmObj, (Beam.eEnd)1, insertPoint, normal, lengthVector, radius);
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
        beamFeat.Offset = offset;

        if (defaultData != null)
        {
          Utils.SetParameters(beamFeat, defaultData);
        }

        bmObj.AddFeature(beamFeat);
      }
      else
      {
        if (!beamFeat.IsKindOf(FilerObject.eObjectType.kBeamMultiContourNotch))
          throw new System.Exception("Not a Beam Feature");

        beamFeat.End = (Beam.eEnd)0;
        Matrix3d cutMatrix = beamFeat.CS;
        Point3d orgin = null;
        Vector3d xVec = null;
        Vector3d yVec = null;
        Vector3d zVec = null;
        cutMatrix.GetCoordSystem(out orgin, out xVec, out yVec, out zVec);
        xVec = new Vector3d(lengthVector);
        yVec = xVec.CrossProduct(normal);
        zVec = xVec.CrossProduct(yVec);
        cutMatrix.SetCoordSystem(insertPoint, xVec, yVec, zVec);
        beamFeat.CS = cutMatrix;

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
        beamFeat.Offset = offset;

        if (defaultData != null)
        {
          Utils.SetParameters(beamFeat, defaultData);
        }
      }

      SetHandle(beamFeat);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(beamFeat, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(beamFeat);
    }

    private void InitBeamPolycut(AdvanceSteel.Nodes.SteelDbObject element,
                        Polyline3d cutPolyline,
                        Autodesk.AdvanceSteel.Geometry.Vector3d normal,
                        Autodesk.AdvanceSteel.Geometry.Vector3d lengthVector,
                        List<Property> beamFeatureProperties)
    {
      List<Property> defaultData = beamFeatureProperties.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> postWriteDBData = beamFeatureProperties.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();

      FilerObject obj = Utils.GetObject(element.Handle);
      if (obj == null || !(obj.IsKindOf(FilerObject.eObjectType.kBeam)))
        throw new System.Exception("No Input Element found");

      ASBeamMultiContourNotch beamFeat = SteelServices.ElementBinder.GetObjectASFromTrace<ASBeamMultiContourNotch>();
      if (beamFeat == null)
      {
        Beam bmObj = obj as Beam;
        beamFeat = new ASBeamMultiContourNotch(bmObj, (Beam.eEnd)1, cutPolyline, normal, lengthVector);

        if (defaultData != null)
        {
          Utils.SetParameters(beamFeat, defaultData);
        }

        bmObj.AddFeature(beamFeat);
      }
      else
      {
        if (!beamFeat.IsKindOf(FilerObject.eObjectType.kBeamMultiContourNotch))
          throw new System.Exception("Not a Beam Feature");

        Beam bmObj = obj as Beam;
        bmObj.DelFeature(beamFeat);
        bmObj.WriteToDb();

        beamFeat = new ASBeamMultiContourNotch(bmObj, (Beam.eEnd)1, cutPolyline, normal, lengthVector);

        if (defaultData != null)
        {
          Utils.SetParameters(beamFeat, defaultData);
        }

        bmObj.AddFeature(beamFeat);
      }

      SetHandle(beamFeat);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(beamFeat, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(beamFeat);
    }

    /// <summary>
    /// Create an Advance Steel Polycut driven by Dynamo Curves on a Beam
    /// </summary>
    /// <param name="element"> Input Beam</param>
    /// <param name="curves"> Input Dynamo Curves referencing Clockwise in sequence to form a closed polyline</param>
    /// <param name="lengthVec"> Input vector in the length direction of rectangular polycut</param>
    /// <param name="additionalBeamFeatureParameters"> Optional Input Beam Feature Build Properties </param>
    /// <returns name="polyCut">polyCut</returns>
    public static BeamPolycut FromListCurves(AdvanceSteel.Nodes.SteelDbObject element,
                                            List<Autodesk.DesignScript.Geometry.Curve> curves,
                                            Autodesk.DesignScript.Geometry.Vector lengthVec,
                                            [DefaultArgument("null")] List<Property> additionalBeamFeatureParameters)
    {

      Polyline3d curveCreatedPolyline = Utils.ToAstPolyline3d(curves, true);
      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters);
      return new BeamPolycut(element,
                        curveCreatedPolyline,
                        curveCreatedPolyline.Normal,
                        Utils.ToAstVector3d(lengthVec, true),
                        additionalBeamFeatureParameters);//ToAstPolyline3d
    }

    /// <summary>
    /// Create an Advance Steel Polycut driven by Dynamo PolyCurve on a Beam
    /// </summary>
    /// <param name="element"> Input Beam</param>
    /// <param name="polyCurve"> Input Dynamo PolyCurve Object</param>
    /// <param name="lengthVec"> Input vector in the length direction of rectangular polycut</param>
    /// <param name="additionalBeamFeatureParameters"> Optional Input Beam Feature Build Properties </param>
    /// <returns name="polyCut">polyCut</returns>
    public static BeamPolycut FromPolyCurve(AdvanceSteel.Nodes.SteelDbObject element,
                                        Autodesk.DesignScript.Geometry.PolyCurve polyCurve,
                                        Autodesk.DesignScript.Geometry.Vector lengthVec,
                                        [DefaultArgument("null")] List<Property> additionalBeamFeatureParameters)
    {

      Polyline3d curveCreatedPolyline = Utils.ToAstPolyline3d(polyCurve, true);
      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters);
      return new BeamPolycut(element,
                        curveCreatedPolyline,
                        curveCreatedPolyline.Normal,
                        Utils.ToAstVector3d(lengthVec, true),
                        additionalBeamFeatureParameters);//ToAstPolyline3d
    }

    /// <summary>
    /// Create an Advance Steel Rectangular Polycut feature by Length and Width
    /// </summary>
    /// <param name="element"> Input Beam</param>
    /// <param name="rectangleInsertPoint"> Input Insert Point for Rectangular polycut on Beam</param>
    /// <param name="normal"> Input normal vector to rectangular polycut</param>
    /// <param name="lengthVec"> Input vector in the length direction of rectangular polycut</param>
    /// <param name="length"> Input Length of Cut</param>
    /// <param name="width"> Input depth of Cut</param>
    /// <param name="corner">0 - TL, 1 - TR, 2 - BR, 3 - BL, else center</param>
    /// <param name="additionalBeamFeatureParameters"> Optional Input Beam Feature Build Properties </param>
    /// <returns name="polyCut">polyCut</returns>
    public static BeamPolycut ByLengthWidth(AdvanceSteel.Nodes.SteelDbObject element,
                                    Autodesk.DesignScript.Geometry.Point rectangleInsertPoint,
                                    Autodesk.DesignScript.Geometry.Vector normal,
                                    Autodesk.DesignScript.Geometry.Vector lengthVec,
                                    double length, double width,
                                    [DefaultArgument("-1")] int corner,
                                    [DefaultArgument("null")] List<Property> additionalBeamFeatureParameters)
    {
      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters, Utils.ToInternalDistanceUnits(length, true), Utils.ToInternalDistanceUnits(width, true));
      return new BeamPolycut(element, 0, Utils.ToAstPoint(rectangleInsertPoint, true),
                              Utils.ToAstVector3d(normal, true),
                              Utils.ToAstVector3d(lengthVec, true),
                              corner,
                              additionalBeamFeatureParameters);
    }

    /// <summary>
    /// Create an Advance Steel Rectangular Polycut feature by Length and Width
    /// </summary>
    /// <param name="element"> Input Plate</param>
    /// <param name="rectangle"> Input Dynamo Rectangle</param>
    /// <param name="lengthVec"> Input vector in the length direction of rectangular polycut</param>
    /// <param name="corner">0 - TL, 1 - TR, 2 - BR, 3 - BL, else center</param>
    /// <param name="additionalBeamFeatureParameters"> Optional Input Beam Feature Build Properties </param>
    /// <returns name="polyCut">polyCut</returns>
    public static BeamPolycut ByRectangle(AdvanceSteel.Nodes.SteelDbObject element,
                        Autodesk.DesignScript.Geometry.Rectangle rectangle,
                        Autodesk.DesignScript.Geometry.Vector lengthVec,
                        [DefaultArgument("-1")] int corner,
                        [DefaultArgument("null")] List<Property> additionalBeamFeatureParameters)
    {
      Autodesk.DesignScript.Geometry.Point rectangleInsertPoint = rectangle.Center();
      Autodesk.DesignScript.Geometry.Vector normal = rectangle.Normal;
      double length = rectangle.Width;
      double width = rectangle.Height;

      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters, Utils.ToInternalDistanceUnits(length, true), Utils.ToInternalDistanceUnits(width, true));
      return new BeamPolycut(element, 0, Utils.ToAstPoint(rectangleInsertPoint, true),
                              Utils.ToAstVector3d(normal, true),
                              Utils.ToAstVector3d(lengthVec, true),
                              corner,
                              additionalBeamFeatureParameters);
    }

    /// <summary>
    /// Create an Advance Steel Circular Polycut feature by Radius
    /// </summary>
    /// <param name="element"> Input Plate</param>
    /// <param name="circularInsertPoint"> Input Insert Point for Rectangular polycut on Beam</param>
    /// <param name="normal"> Input normal vector to rectangular polycut</param>
    /// <param name="lengthVec"> Input vector in the length direction of rectangular polycut</param>
    /// <param name="radius"> Input Radius of Cut</param>
    /// <param name="corner">0 - TL, 1 - TR, 2 - BR, 3 - BL, else center</param>
    /// <param name="additionalBeamFeatureParameters"> Optional Input Beam Feature Build Properties </param>
    /// <returns name="polyCut">polyCut</returns>
    public static BeamPolycut ByRadius(AdvanceSteel.Nodes.SteelDbObject element,
                                Autodesk.DesignScript.Geometry.Point circularInsertPoint,
                                Autodesk.DesignScript.Geometry.Vector normal,
                                Autodesk.DesignScript.Geometry.Vector lengthVec,
                                double radius,
                                [DefaultArgument("-1")] int corner,
                                [DefaultArgument("null")] List<Property> additionalBeamFeatureParameters)
    {
      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters, 0, 0, Utils.ToInternalDistanceUnits(radius, true));
      return new BeamPolycut(element, 0, Utils.ToAstPoint(circularInsertPoint, true),
                              Utils.ToAstVector3d(normal, true),
                              Utils.ToAstVector3d(lengthVec, true),
                              corner,
                              additionalBeamFeatureParameters);
    }

    /// <summary>
    /// Create an Advance Steel Circular Polycut feature by Dynamo Circle
    /// </summary>
    /// <param name="element"> Input Plate</param>
    /// <param name="circle"> Input Dynamo Circle</param>
    /// <param name="lengthVec"> Input vector in the length direction of rectangular polycut</param>
    /// <param name="corner">0 - TL, 1 - TR, 2 - BR, 3 - BL, else center</param>
    /// <param name="additionalBeamFeatureParameters"> Optional Input Beam Feature Build Properties </param>
    /// <returns name="polyCut">polyCut</returns>
    public static BeamPolycut ByCircle(AdvanceSteel.Nodes.SteelDbObject element,
                            Autodesk.DesignScript.Geometry.Circle circle,
                            Autodesk.DesignScript.Geometry.Vector lengthVec,
                            [DefaultArgument("-1")] int corner,
                            [DefaultArgument("null")] List<Property> additionalBeamFeatureParameters)
    {
      Autodesk.DesignScript.Geometry.Point circularInsertPoint = circle.CenterPoint;
      Autodesk.DesignScript.Geometry.Vector normal = circle.Normal;

      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters, 0, 0, Utils.ToInternalDistanceUnits(circle.Radius, true));
      return new BeamPolycut(element, 0, Utils.ToAstPoint(circularInsertPoint, true),
                              Utils.ToAstVector3d(normal, true),
                              Utils.ToAstVector3d(lengthVec, true),
                              corner,
                              additionalBeamFeatureParameters);
    }


    private static List<Property> PreSetDefaults(List<Property> listBeamFeatureData, double length = 0, double width = 0, double radius = 0)
    {
      if (listBeamFeatureData == null)
      {
        listBeamFeatureData = new List<Property>() { };
      }
      if (length > 0) Utils.CheckListUpdateOrAddValue(listBeamFeatureData, "Length", length);
      if (width > 0) Utils.CheckListUpdateOrAddValue(listBeamFeatureData, "Width", width);
      return listBeamFeatureData;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      var beamFeat = Utils.GetObject(Handle) as ASBeamMultiContourNotch;

      Autodesk.AdvanceSteel.Geometry.Matrix3d matrix = beamFeat.CS;
      var poly = Autodesk.DesignScript.Geometry.PolyCurve.ByJoinedCurves(Utils.ToDynPolyCurves(beamFeat.GetPolygon(), true));

      return poly;
    }

  }
}
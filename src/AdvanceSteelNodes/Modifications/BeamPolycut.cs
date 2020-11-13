﻿using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Modifications
{
  /// <summary>
  /// Advance Steel Polycut on a beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class BeamPolycut : GraphicObject
  {
    internal BeamPolycut()
    {
    }

    internal BeamPolycut(AdvanceSteel.Nodes.SteelDbObject element,
                      int cutShapeRectCircle,
                      Autodesk.AdvanceSteel.Geometry.Point3d insertPoint,
                      Autodesk.AdvanceSteel.Geometry.Vector3d normal,
                      Autodesk.AdvanceSteel.Geometry.Vector3d lengthVector,
                      int corner,
                      List<ASProperty> beamFeatureProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          List<ASProperty> defaultData = beamFeatureProperties.Where(x => x.PropLevel == ".").ToList<ASProperty>();
          List<ASProperty> postWriteDBData = beamFeatureProperties.Where(x => x.PropLevel == "Z_PostWriteDB").ToList<ASProperty>();

          double length = 0;
          double width = 0;
          double radius = 0;

          if (defaultData.FirstOrDefault<ASProperty>(x => x.PropName == "Length") != null)
          {
            length = (double)defaultData.FirstOrDefault<ASProperty>(x => x.PropName == "Length").PropValue;
          }
          if (defaultData.FirstOrDefault<ASProperty>(x => x.PropName == "Width") != null)
          {
            width = (double)defaultData.FirstOrDefault<ASProperty>(x => x.PropName == "Width").PropValue;
          }
          if (defaultData.FirstOrDefault<ASProperty>(x => x.PropName == "Radius") != null)
          {
            radius = (double)defaultData.FirstOrDefault<ASProperty>(x => x.PropName == "Radius").PropValue;
          }

          string existingFeatureHandle = SteelServices.ElementBinder.GetHandleFromTrace();

          string elementHandle = element.Handle;
          FilerObject obj = Utils.GetObject(elementHandle);
          BeamMultiContourNotch beamFeat = null;
          if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kBeam))
          {
            if (string.IsNullOrEmpty(existingFeatureHandle) || Utils.GetObject(existingFeatureHandle) == null)
            {
              Beam bmObj = obj as Beam;
              switch (cutShapeRectCircle)
              {
                case 0:
                  beamFeat = new BeamMultiContourNotch(bmObj, (Beam.eEnd)1, insertPoint, normal, lengthVector, length, width);
                  break;
                case 1:
                  beamFeat = new BeamMultiContourNotch(bmObj, (Beam.eEnd)1, insertPoint, normal, lengthVector, radius);
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

              if (postWriteDBData != null)
              {
                Utils.SetParameters(beamFeat, postWriteDBData);
              }
              
            }
            else
            {
              beamFeat = Utils.GetObject(existingFeatureHandle) as BeamMultiContourNotch;
              if (beamFeat != null && beamFeat.IsKindOf(FilerObject.eObjectType.kBeamMultiContourNotch))
              {
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

                if (postWriteDBData != null)
                {
                  Utils.SetParameters(beamFeat, postWriteDBData);
                }

              }
              else
                throw new System.Exception("Not a Beam Feature");
            }
          }
          else
            throw new System.Exception("No Input Element found");

          Handle = beamFeat.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(beamFeat);
        }
      }
    }

    internal BeamPolycut(AdvanceSteel.Nodes.SteelDbObject element,
                        Polyline3d cutPolyline,
                        Autodesk.AdvanceSteel.Geometry.Vector3d normal,
                        Autodesk.AdvanceSteel.Geometry.Vector3d lengthVector,
                        List<ASProperty> beamFeatureProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          List<ASProperty> defaultData = beamFeatureProperties.Where(x => x.PropLevel == ".").ToList<ASProperty>();
          List<ASProperty> postWriteDBData = beamFeatureProperties.Where(x => x.PropLevel == "Z_PostWriteDB").ToList<ASProperty>();

          string existingFeatureHandle = SteelServices.ElementBinder.GetHandleFromTrace();

          string elementHandle = element.Handle;
          FilerObject obj = Utils.GetObject(elementHandle);
          BeamMultiContourNotch beamFeat = null;
          if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kBeam))
          {
            if (string.IsNullOrEmpty(existingFeatureHandle) || Utils.GetObject(existingFeatureHandle) == null)
            {
              Beam bmObj = obj as Beam;
              beamFeat = new BeamMultiContourNotch(bmObj, (Beam.eEnd)1, cutPolyline, normal, lengthVector);

              if (defaultData != null)
              {
                Utils.SetParameters(beamFeat, defaultData);
              }

              bmObj.AddFeature(beamFeat);

              if (postWriteDBData != null)
              {
                Utils.SetParameters(beamFeat, postWriteDBData);
              }

            }
            else
            {
              beamFeat = Utils.GetObject(existingFeatureHandle) as BeamMultiContourNotch;
              if (beamFeat != null && beamFeat.IsKindOf(FilerObject.eObjectType.kBeamMultiContourNotch))
              {

                Beam bmObj = obj as Beam;
                bmObj.DelFeature(beamFeat);
                bmObj.WriteToDb();

                beamFeat = new BeamMultiContourNotch(bmObj, (Beam.eEnd)1, cutPolyline, normal, lengthVector);
                
                if (defaultData != null)
                {
                  Utils.SetParameters(beamFeat, defaultData);
                }

                bmObj.AddFeature(beamFeat);

                if (postWriteDBData != null)
                {
                  Utils.SetParameters(beamFeat, postWriteDBData);
                }

              }
              else
                throw new System.Exception("Not a Beam Feature");
            }
          }
          else
            throw new System.Exception("No Input Element found");

          Handle = beamFeat.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(beamFeat);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel Polycut driven by Dynamo Curves on a Beam
    /// </summary>
    /// <param name="element"> Input Beam</param>
    /// <param name="curves"> Input Dynamo Curves referencing Clockwise in sequence to form a closed polyline</param>
    /// <param name="lengthVec"> Input vector in the length direction of rectangular polycut</param>
    /// <param name="additionalBeamFeatureParameters"> Optional Input Beam Feature Build Properties </param>
    /// <returns></returns>
    public static BeamPolycut FromListCurves(AdvanceSteel.Nodes.SteelDbObject element, 
                                            List<Autodesk.DesignScript.Geometry.Curve> curves,
                                            Autodesk.DesignScript.Geometry.Vector lengthVec,
                                            [DefaultArgument("null")]List<ASProperty> additionalBeamFeatureParameters)
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
    /// <returns></returns>
    public static BeamPolycut FromPolyCurve(AdvanceSteel.Nodes.SteelDbObject element,
                                        Autodesk.DesignScript.Geometry.PolyCurve polyCurve,
                                        Autodesk.DesignScript.Geometry.Vector lengthVec,
                                        [DefaultArgument("null")]List<ASProperty> additionalBeamFeatureParameters)
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
    /// <returns></returns>
    public static BeamPolycut ByLengthWidth(AdvanceSteel.Nodes.SteelDbObject element,
                                    Autodesk.DesignScript.Geometry.Point rectangleInsertPoint,
                                    Autodesk.DesignScript.Geometry.Vector normal,
                                    Autodesk.DesignScript.Geometry.Vector lengthVec,
                                    double length, double width,
                                    [DefaultArgument("-1")]int corner,
                                    [DefaultArgument("null")]List<ASProperty> additionalBeamFeatureParameters)
    {
      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters, Utils.ToInternalUnits(length, true), Utils.ToInternalUnits(width, true));
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
    /// <returns></returns>
    public static BeamPolycut ByRectangle(AdvanceSteel.Nodes.SteelDbObject element,
                        Autodesk.DesignScript.Geometry.Rectangle rectangle,
                        Autodesk.DesignScript.Geometry.Vector lengthVec,
                        [DefaultArgument("-1")]int corner,
                        [DefaultArgument("null")]List<ASProperty> additionalBeamFeatureParameters)
    {
      Autodesk.DesignScript.Geometry.Point rectangleInsertPoint = rectangle.Center();
      Autodesk.DesignScript.Geometry.Vector normal = rectangle.Normal;
      double length = rectangle.Width;
      double width = rectangle.Height;

      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters, Utils.ToInternalUnits(length, true), Utils.ToInternalUnits(width, true));
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
    /// <returns></returns>
    public static BeamPolycut ByRadius(AdvanceSteel.Nodes.SteelDbObject element,
                                Autodesk.DesignScript.Geometry.Point circularInsertPoint,
                                Autodesk.DesignScript.Geometry.Vector normal,
                                Autodesk.DesignScript.Geometry.Vector lengthVec,
                                double radius,
                                [DefaultArgument("-1")]int corner,
                                [DefaultArgument("null")]List<ASProperty> additionalBeamFeatureParameters)
    {
      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters, 0, 0, Utils.ToInternalUnits(radius, true));
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
    /// <returns></returns>
    public static BeamPolycut ByCircle(AdvanceSteel.Nodes.SteelDbObject element,
                            Autodesk.DesignScript.Geometry.Circle circle,
                            Autodesk.DesignScript.Geometry.Vector lengthVec,
                            [DefaultArgument("-1")]int corner,
                            [DefaultArgument("null")]List<ASProperty> additionalBeamFeatureParameters)
    {
      Autodesk.DesignScript.Geometry.Point circularInsertPoint = circle.CenterPoint;
      Autodesk.DesignScript.Geometry.Vector normal = circle.Normal;

      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters, 0, 0, Utils.ToInternalUnits(circle.Radius, true));
      return new BeamPolycut(element, 0, Utils.ToAstPoint(circularInsertPoint, true),
                              Utils.ToAstVector3d(normal, true),
                              Utils.ToAstVector3d(lengthVec, true),
                              corner,
                              additionalBeamFeatureParameters);
    }


    private static List<ASProperty> PreSetDefaults(List<ASProperty> listBeamFeatureData, double length = 0, double width = 0, double radius = 0)
    {
      if (listBeamFeatureData == null)
      {
        listBeamFeatureData = new List<ASProperty>() { };
      }
      if (length > 0) Utils.CheckListUpdateOrAddValue(listBeamFeatureData, "Length", length, ".");
      if (width > 0) Utils.CheckListUpdateOrAddValue(listBeamFeatureData, "Width", width, ".");
      return listBeamFeatureData;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var beamFeat = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.BeamMultiContourNotch;

          Autodesk.AdvanceSteel.Geometry.Matrix3d matrix = beamFeat.CS;
          var poly = Autodesk.DesignScript.Geometry.PolyCurve.ByJoinedCurves( Utils.ToDynPolyCurves(beamFeat.GetPolygon(), true) );

          return poly;
        }
      }
    }
  }
}
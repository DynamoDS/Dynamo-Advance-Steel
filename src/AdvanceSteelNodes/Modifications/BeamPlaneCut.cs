﻿using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Modifications
{
  /// <summary>
  /// Advance Steel Beam Plane Cut
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class BeamPlaneCut : GraphicObject
  {
    internal BeamPlaneCut(AdvanceSteel.Nodes.SteelDbObject element, 
                      Point3d cutPoint,
                      Vector3d normal,
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
          BeamShortening beamFeat = null;
          if (obj != null && (obj.IsKindOf(FilerObject.eObjectType.kBeam)))
          {
            if (string.IsNullOrEmpty(existingFeatureHandle) || Utils.GetObject(existingFeatureHandle) == null)
            {
              AtomicElement atomic = obj as AtomicElement;
              beamFeat = new BeamShortening();
              atomic.AddFeature(beamFeat);
              beamFeat.Set(cutPoint, normal);
              if (defaultData != null)
              {
                Utils.SetParameters(beamFeat, defaultData);
              }
              atomic.AddFeature(beamFeat);
              if (postWriteDBData != null)
              {
                Utils.SetParameters(beamFeat, postWriteDBData);
              }
            }
            else
            {
              beamFeat = Utils.GetObject(existingFeatureHandle) as BeamShortening;
              if (beamFeat != null && beamFeat.IsKindOf(FilerObject.eObjectType.kBeamShortening))
              {
                beamFeat.Set(cutPoint, normal);
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
                throw new System.Exception("Not a Beam Shorting Feature");
            }
          }
          else
            throw new System.Exception("No Input Element found");

          Handle = beamFeat.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(beamFeat);
        }
      }
    }

    internal BeamPlaneCut(AdvanceSteel.Nodes.SteelDbObject element,
                  int end,
                  double shorteningLength,
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
          BeamShortening beamFeat = null;
          if (obj != null && (obj.IsKindOf(FilerObject.eObjectType.kBeam)))
          {
            Matrix3d matrixAtPointOnBeam = null;
            Point3d shortPt = null;
            Point3d cutPoint = null;
            Vector3d normal = null;
            if (obj.IsKindOf(FilerObject.eObjectType.kBentBeam))
            {
              BentBeam actObj = obj as BentBeam;
              switch (end)
              {
                case 0: //Start
                  shortPt = actObj.GetPointAtStart(shorteningLength);
                  break;
                case 1: //End
                  shortPt = actObj.GetPointAtEnd(shorteningLength);
                  break;
              }
              matrixAtPointOnBeam = actObj.GetCSAtPoint(shortPt);
            }
            else
            {
              Beam actObj = obj as Beam;
              switch (end)
              {
                case 0: //Start
                  shortPt = actObj.GetPointAtStart(shorteningLength);
                  break;
                case 1: //End
                  shortPt = actObj.GetPointAtEnd(shorteningLength);
                  break;
              }
              matrixAtPointOnBeam = actObj.GetCSAtPoint(shortPt);
            }
            Point3d orgin = null;
            Vector3d xV = null;
            Vector3d xY = null;
            Vector3d xZ = null;
            matrixAtPointOnBeam.GetCoordSystem(out orgin, out xV, out xY, out xZ);
            cutPoint = orgin;
            normal = xV;
            if (string.IsNullOrEmpty(existingFeatureHandle) || Utils.GetObject(existingFeatureHandle) == null)
            {
              AtomicElement atomic = obj as AtomicElement;
              beamFeat = new BeamShortening();
              atomic.AddFeature(beamFeat);
              beamFeat.Set(cutPoint, normal);
              if (defaultData != null)
              {
                Utils.SetParameters(beamFeat, defaultData);
              }
              atomic.AddFeature(beamFeat);
              if (postWriteDBData != null)
              {
                Utils.SetParameters(beamFeat, postWriteDBData);
              }
            }
            else
            {
              beamFeat = Utils.GetObject(existingFeatureHandle) as BeamShortening;
              if (beamFeat != null && beamFeat.IsKindOf(FilerObject.eObjectType.kBeamShortening))
              {
                beamFeat.Set(cutPoint, normal);
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
                throw new System.Exception("Not a Beam Shorting Feature");
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
    /// Create an Advance Steel Beam Plane Cut by Coordinate System
    /// </summary>
    /// <param name="element"> Input Beam</param>
    /// <param name="coordinateSystem"> Input Dynamo CoordinateSytem</param>
    /// <param name="additionalBeamFeatureParameters"> Optional Input Beam Cut Build Properties </param>
    /// <returns></returns>
    public static BeamPlaneCut ByCS(AdvanceSteel.Nodes.SteelDbObject element,
                                    Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                    [DefaultArgument("null")]List<ASProperty> additionalBeamFeatureParameters)
    {
      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters);
      return new BeamPlaneCut(element, Utils.ToAstPoint(coordinateSystem.Origin, true), Utils.ToAstVector3d(coordinateSystem.ZAxis, true), additionalBeamFeatureParameters);
    }

    /// <summary>
    /// Create an Advance Steel Beam Plane Cut by Point and Normal
    /// </summary>
    /// <param name="element"></param>
    /// <param name="origin"></param>
    /// <param name="normal"></param>
    /// <param name="additionalBeamFeatureParameters"></param>
    /// <returns></returns>
    public static BeamPlaneCut ByPointAndNormal(AdvanceSteel.Nodes.SteelDbObject element,
                                                Autodesk.DesignScript.Geometry.Point origin,
                                                Autodesk.DesignScript.Geometry.Vector normal,
                                                [DefaultArgument("null")]List<ASProperty> additionalBeamFeatureParameters)
    {
      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters);
      return new BeamPlaneCut(element, Utils.ToAstPoint(origin, true), Utils.ToAstVector3d(normal, true), additionalBeamFeatureParameters);
    }

    /// <summary>
    /// Create an Advance Steel Beam Shorting from the Start of the Beam by a Value
    /// </summary>
    /// <param name="element"> Input Beam</param>
    /// <param name="shorteningLength"> Input shortening Value</param>
    /// <param name="additionalBeamFeatureParameters"> Optional Input Beam Cut Build Properties </param>
    /// <returns></returns>
    public static BeamPlaneCut ByStartValue(AdvanceSteel.Nodes.SteelDbObject element,
                        double shorteningLength,
                        [DefaultArgument("null")]List<ASProperty> additionalBeamFeatureParameters)
    {
      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters);
      return new BeamPlaneCut(element, 0, Utils.ToInternalUnits(shorteningLength, true), additionalBeamFeatureParameters);
    }

    /// <summary>
    /// Create an Advance Steel Beam Shorting from the End of the Beam by a Value
    /// </summary>
    /// <param name="element"> Input Beam</param>
    /// <param name="shorteningLength"> Input shortening Value</param>
    /// <param name="additionalBeamFeatureParameters"> Optional Input Beam Cut Build Properties </param>
    /// <returns></returns>
    public static BeamPlaneCut ByEndValue(AdvanceSteel.Nodes.SteelDbObject element,
                        double shorteningLength,
                        [DefaultArgument("null")]List<ASProperty> additionalBeamFeatureParameters)
    {
      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters);
      return new BeamPlaneCut(element, 1, Utils.ToInternalUnits(shorteningLength, true), additionalBeamFeatureParameters);
    }

    private static List<ASProperty> PreSetDefaults(List<ASProperty> listBeamFeatureData)
    {
      if (listBeamFeatureData == null)
      {
        listBeamFeatureData = new List<ASProperty>() { };
      }
      return listBeamFeatureData;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var beamFeat = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.BeamShortening;
          Autodesk.AdvanceSteel.Geometry.Matrix3d matrix = beamFeat.CS;
          var poly = Autodesk.DesignScript.Geometry.Rectangle.ByWidthLength(Utils.ToDynCoordinateSys(matrix, true),
                                                         Utils.FromInternalUnits(200, true),
                                                         Utils.FromInternalUnits(100, true));

          return poly;
        }
      }
    }
  }
}
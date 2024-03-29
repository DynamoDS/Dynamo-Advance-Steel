﻿using System;
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

namespace AdvanceSteel.Nodes.Gratings
{
  /// <summary>
  /// Advance Steel Variable Grating Pattern
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class VariableGrating : GraphicObject
  {
    internal VariableGrating()
    {
    }

    internal VariableGrating(Point3d ptCenter,
                      Vector3d vNormal,
                      double dWidth,
                      double dLength,
                      List<Property> additionalGratingParameters)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {

          List<Property> defaultData = additionalGratingParameters.Where(x => x.Level == ".").ToList<Property>();
          List<Property> postWriteDBData = additionalGratingParameters.Where(x => x.Level == "Z_PostWriteDB").ToList<Property>();

          string strClass = (string)defaultData.FirstOrDefault<Property>(x => x.Name == "GratingClass").InternalValue;
          string strName = (string)defaultData.FirstOrDefault<Property>(x => x.Name == "GratingSize").InternalValue;

          Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(ptCenter, vNormal);
          Autodesk.AdvanceSteel.Modelling.Grating gratings = null;
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            gratings = new Autodesk.AdvanceSteel.Modelling.Grating(strClass, strName, plane, ptCenter, dWidth, dLength);

            if (defaultData != null)
            {
              Utils.SetParameters(gratings, defaultData);
            }

            gratings.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(gratings, postWriteDBData);
            }

          }
          else
          {
            gratings = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Grating;
            if (gratings != null && gratings.IsKindOf(FilerObject.eObjectType.kGrating))
            {
              gratings.GratingClass = strClass;
              gratings.GratingSize = strName;
              gratings.DefinitionPlane = plane;
              gratings.SetLength(dWidth, true);
              gratings.SetWidth(dLength, true);

              if (defaultData != null)
              {
                Utils.SetParameters(gratings, defaultData);
              }

              if (postWriteDBData != null)
              {
                Utils.SetParameters(gratings, postWriteDBData);
              }
            }
            else
            {
              throw new System.Exception("Not a Variable Grating pattern");
            }
          }

          Handle = gratings.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(gratings);
        }
      }
    }

    internal VariableGrating(Autodesk.DesignScript.Geometry.Polygon poly,
                              Vector3d vNormal,
                              List<Property> additionalGratingParameters)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          Autodesk.DesignScript.Geometry.PolyCurve polyCurve = poly;

          List<Property> defaultData = additionalGratingParameters.Where(x => x.Level == ".").ToList<Property>();
          List<Property> postWriteDBData = additionalGratingParameters.Where(x => x.Level == "Z_PostWriteDB").ToList<Property>();

          string strClass = (string)defaultData.FirstOrDefault<Property>(x => x.Name == "GratingClass").InternalValue;
          string strName = (string)defaultData.FirstOrDefault<Property>(x => x.Name == "GratingSize").InternalValue;

          Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(Utils.ToAstPoint(poly.Center(), true), vNormal);
          Autodesk.AdvanceSteel.Modelling.Grating gratings = null;
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();
          Point3d[] astPoints = Utils.ToAstPoints(polyCurve.Points, true);

          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            gratings = new Autodesk.AdvanceSteel.Modelling.Grating(strClass, strName, plane, astPoints);

            if (defaultData != null)
            {
              Utils.SetParameters(gratings, defaultData);
            }

            gratings.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(gratings, postWriteDBData);
            }
          }
          else
          {
            gratings = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Grating;
            if (gratings != null && gratings.IsKindOf(FilerObject.eObjectType.kGrating))
            {
              gratings.GratingClass = strClass;
              gratings.GratingSize = strName;
              gratings.DefinitionPlane = plane;
              gratings.SetPolygonContour(astPoints);

              if (defaultData != null)
              {
                Utils.SetParameters(gratings, defaultData);
              }

              if (postWriteDBData != null)
              {
                Utils.SetParameters(gratings, postWriteDBData);
              }
            }
            else
            {
              throw new System.Exception("Not a Variable Grating pattern");
            }
          }

          Handle = gratings.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(gratings);
        }
      }
    }

    /// <summary>
    /// Create Advance Steel Variable Grating by Dynamo Rectangle and Coordinate System
    /// </summary>
    /// <param name="coordinateSystem"> Input Dynamo Coordinate System</param>
    /// <param name="gratingClass"> Input Grating Class</param>
    /// <param name="gratingName"> Input Grating Size</param>
    /// <param name="width"> Input Grating Width</param>
    /// <param name="length"> Input Grating Length</param>
    /// <param name="additionalGratingParameters"> Optional Input Grating Build Properties </param>
    /// <returns name="variableGrating"> grating</returns>
    public static VariableGrating ByRectangularByCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                    string gratingClass,
                                                    string gratingName,
                                                    double width,
                                                    double length,
                                                    [DefaultArgument("null")] List<Property> additionalGratingParameters)
    {
      additionalGratingParameters = PreSetDefaults(additionalGratingParameters, gratingClass, gratingName);

      return new VariableGrating(Utils.ToAstPoint(coordinateSystem.Origin, true),
                                 Utils.ToAstVector3d(coordinateSystem.ZAxis, true),
                                 Utils.ToInternalDistanceUnits(width, true),
                                 Utils.ToInternalDistanceUnits(length, true),
                                 additionalGratingParameters);
    }

    /// <summary>
    /// Create Advance Steel Variable Grating by Dynamo Rectangle and Point and Vectors
    /// </summary>
    /// <param name="origin"> Input Dynamo Point</param>
    /// <param name="xVector"> Input Dynamo X Vector</param>
    /// <param name="yVector"> Input Dynamo Y Vector</param>
    /// <param name="gratingClass"> Input Grating Class</param>
    /// <param name="gratingName"> Input Grating Size</param>
    /// <param name="width"> Input Grating Width</param>
    /// <param name="length"> Input Grating Length</param>
    /// <param name="additionalGratingParameters"> Optional Input Grating Build Properties </param>
    /// <returns name="variableGrating"> grating</returns>
    public static VariableGrating ByRectangularByPointAndVectors(Autodesk.DesignScript.Geometry.Point origin,
                                                Autodesk.DesignScript.Geometry.Vector xVector,
                                                Autodesk.DesignScript.Geometry.Vector yVector,
                                                string gratingClass,
                                                string gratingName,
                                                double width,
                                                double length,
                                                [DefaultArgument("null")] List<Property> additionalGratingParameters)
    {
      Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem = Autodesk.DesignScript.Geometry.CoordinateSystem.ByOriginVectors(origin, xVector, yVector);

      additionalGratingParameters = PreSetDefaults(additionalGratingParameters, gratingClass, gratingName);

      return new VariableGrating(Utils.ToAstPoint(coordinateSystem.Origin, true),
                                 Utils.ToAstVector3d(coordinateSystem.ZAxis, true),
                                 Utils.ToInternalDistanceUnits(width, true),
                                 Utils.ToInternalDistanceUnits(length, true),
                                 additionalGratingParameters);
    }

    /// <summary>
    /// Create Advance Steel Variable Grating using Dynamo Origin Point and Normal to Grating Plate - Assumes World X Vector to get cross product
    /// </summary>
    /// <param name="origin"> Input Dynamo Point</param>
    /// <param name="normal"> Input Dynamo Vector for Normal to Grating Plane</param>
    /// <param name="gratingClass"> Input Grating Class</param>
    /// <param name="gratingName"> Input Grating Size</param>
    /// <param name="width"> Input Grating Width</param>
    /// <param name="length"> Input Grating Length</param>
    /// <param name="additionalGratingParameters"> Optional Input Grating Build Properties </param>
    /// <returns name="variableGrating"> grating</returns>
    public static VariableGrating ByRectangularByPointAndNormal(Autodesk.DesignScript.Geometry.Point origin,
                                            Autodesk.DesignScript.Geometry.Vector normal,
                                            string gratingClass,
                                            string gratingName,
                                            double width,
                                            double length,
                                            [DefaultArgument("null")] List<Property> additionalGratingParameters)
    {
      Vector3d as_normal = Utils.ToAstVector3d(normal, true);
      Vector3d xWorldVec = Vector3d.kXAxis;
      Vector3d xYVector = as_normal.CrossProduct(xWorldVec);

      Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem = Autodesk.DesignScript.Geometry.CoordinateSystem.ByOriginVectors(origin,
                                                  Utils.ToDynVector(xWorldVec, true),
                                                  Utils.ToDynVector(xYVector, true));

      additionalGratingParameters = PreSetDefaults(additionalGratingParameters, gratingClass, gratingName);

      return new VariableGrating(Utils.ToAstPoint(coordinateSystem.Origin, true),
                                 Utils.ToAstVector3d(coordinateSystem.ZAxis, true),
                                 Utils.ToInternalDistanceUnits(width, true),
                                 Utils.ToInternalDistanceUnits(length, true),
                                 additionalGratingParameters);
    }

    /// <summary>
    /// Create Advance Steel Variable Grating by Dynamo Polygon
    /// </summary>
    /// <param name="gratingClass"> Input Grating Class</param>
    /// <param name="gratingName"> Input Grating Size</param>
    /// <param name="poly"> Input dynamo Polygon</param>
    /// <param name="additionalGratingParameters"> Optional Input Grating Build Properties </param>
    /// <returns name="variableGrating"> grating</returns>
		public static VariableGrating ByPolygon(Autodesk.DesignScript.Geometry.Polygon poly,
                                            string gratingClass,
                                            string gratingName,
                                            [DefaultArgument("null")] List<Property> additionalGratingParameters)
    {
      additionalGratingParameters = PreSetDefaults(additionalGratingParameters, gratingClass, gratingName);
      return new VariableGrating(poly, Utils.ToAstVector3d(poly.Normal, true), additionalGratingParameters);
    }

    private static List<Property> PreSetDefaults(List<Property> listGratingData, string gratingClass, string gratingName)
    {
      if (listGratingData == null)
      {
        listGratingData = new List<Property>() { };
      }
      Utils.CheckListUpdateOrAddValue(listGratingData, "GratingClass", gratingClass, ".");
      Utils.CheckListUpdateOrAddValue(listGratingData, "GratingSize", gratingName, ".");
      return listGratingData;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var grating = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.Grating;

          if (grating == null)
          {
            throw new Exception("Null Variable Grating pattern");
          }

          List<DynGeometry.Point> polyPoints = GratingDraw.GetPointsToDraw(grating);

          return Autodesk.DesignScript.Geometry.Polygon.ByPoints(polyPoints);
        }
      }
    }
  }
}


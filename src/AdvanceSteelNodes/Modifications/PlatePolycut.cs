using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.Modelling;

namespace AdvanceSteel.Nodes.Modifications
{
  /// <summary>
  /// Advance Steel Polycut on object
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class PlatePolycut : GraphicObject
  {
    internal PlatePolycut(AdvanceSteel.Nodes.SteelDbObject element, 
                      double xOffset, double yOffset, int corner,
                      int cutShapeRectCircle,
                      List<ASProperty> plateFeatureProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          List<ASProperty> defaultData = plateFeatureProperties.Where(x => x.PropLevel == ".").ToList<ASProperty>();
          List<ASProperty> postWriteDBData = plateFeatureProperties.Where(x => x.PropLevel == "Z_PostWriteDB").ToList<ASProperty>();

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
          PlateFeatContour plateFeat = null;
          if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kPlate))
          {
            if (string.IsNullOrEmpty(existingFeatureHandle) || Utils.GetObject(existingFeatureHandle) == null)
            {
              Matrix2d m2d = new Matrix2d();
              m2d.SetCoordSystem(new Point2d(xOffset, yOffset), new Vector2d(1, 0), new Vector2d(0, 1));
              switch (cutShapeRectCircle)
              {
                case 0:
                  plateFeat = new PlateFeatContour(m2d, length, width);
                  break;
                case 1:
                  plateFeat = new PlateFeatContour(m2d, length);
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

              if (postWriteDBData != null)
              {
                Utils.SetParameters(plateFeat, postWriteDBData);
              }
              
            }
            else
            {
              plateFeat = Utils.GetObject(existingFeatureHandle) as PlateFeatContour;
              if (plateFeat != null && plateFeat.IsKindOf(FilerObject.eObjectType.kPlateFeatContour))
              {
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

                if (defaultData != null)
                {
                  Utils.SetParameters(plateFeat, defaultData);
                }

                if (postWriteDBData != null)
                {
                  Utils.SetParameters(plateFeat, postWriteDBData);
                }

              }
              else
                throw new System.Exception("Not a Plate Feature");
            }
          }
          else
            throw new System.Exception("No Input Element found");

          Handle = plateFeat.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(plateFeat);
        }
      }
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
    /// <returns></returns>
    public static PlatePolycut ByLengthAndWidth(AdvanceSteel.Nodes.SteelDbObject element,
                                    double length,
                                    double width,
                                    [DefaultArgument("0")]double xOffset,
                                    [DefaultArgument("0")]double yOffset,
                                    [DefaultArgument("-1")]int corner,
                                    [DefaultArgument("null")]List<ASProperty> additionalPlateFeatureParameters)
    {
      additionalPlateFeatureParameters = PreSetDefaults(additionalPlateFeatureParameters, Utils.ToInternalUnits(length, true), Utils.ToInternalUnits(width, true));
      return new PlatePolycut(element, Utils.ToInternalUnits(xOffset, true), Utils.ToInternalUnits(yOffset, true), corner, 0, additionalPlateFeatureParameters);
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
    /// <returns></returns>
    public static PlatePolycut ByRadius(AdvanceSteel.Nodes.SteelDbObject element,
                                double radius,
                                [DefaultArgument("0")]double xOffset,
                                [DefaultArgument("0")]double yOffset,
                                [DefaultArgument("-1")]int corner,
                                [DefaultArgument("null")]List<ASProperty> additionalPlateFeatureParameters)
    {
      additionalPlateFeatureParameters = PreSetDefaults(additionalPlateFeatureParameters, 0, 0, Utils.ToInternalUnits(radius, true));
      return new PlatePolycut(element, Utils.ToInternalUnits(xOffset, true), Utils.ToInternalUnits(yOffset, true), corner, 1, additionalPlateFeatureParameters);
    }

    private static List<ASProperty> PreSetDefaults(List<ASProperty> listPlateFeatureData, double length = 0, double width = 0, double radius = 0)
    {
      if (listPlateFeatureData == null)
      {
        listPlateFeatureData = new List<ASProperty>() { };
      }
      if (length > 0) Utils.CheckListUpdateOrAddValue(listPlateFeatureData, "Length", length, ".");
      if (width > 0) Utils.CheckListUpdateOrAddValue(listPlateFeatureData, "Width", width, ".");
      if (radius > 0) Utils.CheckListUpdateOrAddValue(listPlateFeatureData, "Radius", radius, ".");
      return listPlateFeatureData;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var plateFeat = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.PlateFeatContour;

          var dynPoints = Utils.ToDynPoints(plateFeat.GetContourPolygon(0), true);
          var poly = Autodesk.DesignScript.Geometry.Polygon.ByPoints(dynPoints, true);
          foreach (var pt in dynPoints) { pt.Dispose(); }

          return poly;
        }
      }
    }
  }
}
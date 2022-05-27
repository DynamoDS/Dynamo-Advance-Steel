using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using ASPlateFeatVertFillet = Autodesk.AdvanceSteel.Modelling.PlateFeatVertFillet;

namespace AdvanceSteel.Nodes.Features
{
  /// <summary>
  /// Advance Steel Polycut Vertex at Corners
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class PlateVertexCut : GraphicObject
  {
    private PlateVertexCut(AdvanceSteel.Nodes.SteelDbObject element,
                      int vertexFeatureType,
                      List<Property> plateFeatureProperties)
    {
      SafeInit(() => InitPlateVertexCut(element, vertexFeatureType, plateFeatureProperties));
    }

    private PlateVertexCut(ASPlateFeatVertFillet plateFeat)
    {
      SafeInit(() => SetHandle(plateFeat));
    }

    internal static PlateVertexCut FromExisting(ASPlateFeatVertFillet plateFeat)
    {
      return new PlateVertexCut(plateFeat)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitPlateVertexCut(AdvanceSteel.Nodes.SteelDbObject element,
                      int vertexFeatureType,
                      List<Property> plateFeatureProperties)
    {
      List<Property> defaultData = plateFeatureProperties.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> postWriteDBData = plateFeatureProperties.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();

      double length1 = 0;
      double length2 = 0;
      double radius = 0;

      if (defaultData.FirstOrDefault<Property>(x => x.MemberName == nameof(ASPlateFeatVertFillet.Length1)) != null)
      {
        length1 = (double)defaultData.FirstOrDefault<Property>(x => x.MemberName == nameof(ASPlateFeatVertFillet.Length1)).InternalValue;
      }
      if (defaultData.FirstOrDefault<Property>(x => x.MemberName == nameof(ASPlateFeatVertFillet.Length2)) != null)
      {
        length2 = (double)defaultData.FirstOrDefault<Property>(x => x.MemberName == nameof(ASPlateFeatVertFillet.Length2)).InternalValue;
      }
      if (defaultData.FirstOrDefault<Property>(x => x.MemberName == nameof(ASPlateFeatVertFillet.Radius)) != null)
      {
        radius = (double)defaultData.FirstOrDefault<Property>(x => x.MemberName == nameof(ASPlateFeatVertFillet.Radius)).InternalValue;
      }

      FilerObject obj = Utils.GetObject(element.Handle);
      if (obj == null || (!(obj.IsKindOf(FilerObject.eObjectType.kPlate)) && !obj.IsKindOf(FilerObject.eObjectType.kFoldedPlate)))
        throw new System.Exception("No Input Element found");

      ASPlateFeatVertFillet plateFeat = SteelServices.ElementBinder.GetObjectASFromTrace<ASPlateFeatVertFillet>();
      if (plateFeat == null)
      {
        plateFeat = new ASPlateFeatVertFillet();
        plateFeat.FilletType = (FilerObject.eFilletTypes)vertexFeatureType;

        AtomicElement atomic = obj as AtomicElement;
        if (defaultData != null)
        {
          Utils.SetParameters(plateFeat, defaultData);
        }

        atomic.AddFeature(plateFeat);
      }
      else
      {
        if (!plateFeat.IsKindOf(FilerObject.eObjectType.kPlateFeatVertFillet))
          throw new System.Exception("Not a Plate Feature");

        if (defaultData != null)
        {
          Utils.SetParameters(plateFeat, defaultData);
        }
      }

      SetHandle(plateFeat);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(plateFeat, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(plateFeat);
    }

    /// <summary>
    /// Create an Advance Steel corner feature - fillet Cut
    /// </summary>
    /// <param name="element"> Input Plate</param>
    /// <param name="radius"> Input Radius</param>
    /// <param name="filletType"> Input 0 - Convex and 1 - Concave</param>
    /// <param name="plateFoldIndex"> Input plate fold number, 0 for normal plate, zero or greater for folded plate</param>
    /// <param name="cornerIndex"> Input corner number around the edge of the plate</param>
    /// <param name="additionalPlateFeatureParameters"> Optional Input Plate Cut Build Properties </param>
    /// <returns name="vertexCut">vertexCut</returns>
    public static PlateVertexCut ByRadius(AdvanceSteel.Nodes.SteelDbObject element,
                                double radius,
                                [DefaultArgument("0")] int filletType,
                                [DefaultArgument("0")] int plateFoldIndex,
                                [DefaultArgument("0")] short cornerIndex,
                                [DefaultArgument("null")] List<Property> additionalPlateFeatureParameters)
    {
      if (filletType != 0 && filletType != 1)
        throw new System.Exception("Fillet Type Can only be 0 or 1");
      additionalPlateFeatureParameters = PreSetDefaults(additionalPlateFeatureParameters, plateFoldIndex, cornerIndex, 0, 0, Utils.ToInternalDistanceUnits(radius, true));
      return new PlateVertexCut(element, filletType, additionalPlateFeatureParameters);
    }

    /// <summary>
    /// Create an Advance Steel corner feature - Straight Cut
    /// </summary>
    /// <param name="element"> Input Plate</param>
    /// <param name="length1"> Input Chamfer Length Value 1</param>
    /// <param name="length2"> Input Chamfer Length Value 2</param>
    /// <param name="plateFoldIndex"> Input plate fold number, 0 for normal plate, zero or greater for folded plate</param>
    /// <param name="cornerIndex"> Input corner number around the edge of the plate</param>
    /// <param name="additionalPlateFeatureParameters"> Optional Input Plate Cut Build Properties </param>
    /// <returns name="vertexCut">vertexCut</returns>
    public static PlateVertexCut ByChamfer(AdvanceSteel.Nodes.SteelDbObject element,
                            double length1,
                            double length2,
                            [DefaultArgument("0")] int plateFoldIndex,
                            [DefaultArgument("0")] short cornerIndex,
                            [DefaultArgument("null")] List<Property> additionalPlateFeatureParameters)
    {
      additionalPlateFeatureParameters = PreSetDefaults(additionalPlateFeatureParameters, plateFoldIndex, cornerIndex, Utils.ToInternalDistanceUnits(length1, true), Utils.ToInternalDistanceUnits(length2, true));
      return new PlateVertexCut(element, 2, additionalPlateFeatureParameters);
    }

    private static List<Property> PreSetDefaults(List<Property> listPlateFeatureData, int conIndex = -1, short vertIndex = -1, double length1 = 0, double length2 = 0, double radius = 0)
    {
      if (listPlateFeatureData == null)
      {
        listPlateFeatureData = new List<Property>() { };
      }
      if (conIndex > -1) Utils.CheckListUpdateOrAddValue(typeof(PlateFeatEdge), listPlateFeatureData, nameof(PlateFeatEdge.ContourIndex), conIndex);
      if (vertIndex > -1) Utils.CheckListUpdateOrAddValue(typeof(PlateFeatEdge), listPlateFeatureData, nameof(PlateFeatVertex.VertexIndex), vertIndex);
      if (length1 > 0) Utils.CheckListUpdateOrAddValue(typeof(PlateFeatVertFillet), listPlateFeatureData, nameof(PlateFeatVertFillet.Length1), length1);
      if (length2 > 0) Utils.CheckListUpdateOrAddValue(typeof(PlateFeatVertFillet), listPlateFeatureData, nameof(PlateFeatVertFillet.Length2), length2);
      if (radius > 0) Utils.CheckListUpdateOrAddValue(typeof(PlateFeatVertFillet), listPlateFeatureData, nameof(PlateFeatVertFillet.Radius), radius);
      return listPlateFeatureData;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      var plateFeat = Utils.GetObject(Handle) as ASPlateFeatVertFillet;

      var dynPoints = Utils.ToDynPoints(plateFeat.GetBaseContourPolygon(0), true);
      var poly = Autodesk.DesignScript.Geometry.Polygon.ByPoints(dynPoints, true);
      foreach (var pt in dynPoints) { pt.Dispose(); }

      return poly;
    }

  }
}
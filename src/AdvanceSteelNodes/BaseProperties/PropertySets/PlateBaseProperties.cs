using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;
using static Autodesk.AdvanceSteel.DotNetRoots.Units.Unit;

namespace AdvanceSteel.Nodes
{
  public class PlateBaseProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kPlateBase;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Portioning", nameof(PlateBase.Portioning));
      InsertItem(dictionary, objectASType, "Upper Plane", nameof(PlateBase.UpperPlane), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Length", nameof(PlateBase.Length), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Width", nameof(PlateBase.Width), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Length Increment", nameof(PlateBase.LengthIncrement), LevelEnum.NoDefinition, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Radius", nameof(PlateBase.Radius), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Radius Increment", nameof(PlateBase.RadIncrement));
      InsertItem(dictionary, objectASType, "Lower Z Position", nameof(PlateBase.LowerZPos));
      InsertItem(dictionary, objectASType, "Plate Normal", nameof(PlateBase.PlateNormal));
      InsertItem(dictionary, objectASType, "Thickness", nameof(PlateBase.Thickness));
      InsertItem(dictionary, objectASType, "Lower Plane", nameof(PlateBase.LowerPlane), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Upper Z Position", nameof(PlateBase.UpperZPos), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Top Is Z Positive", nameof(PlateBase.TopIsZPositive));
      InsertItem(dictionary, objectASType, "Definition Plane", nameof(PlateBase.DefinitionPlane));

      InsertItem(dictionary, objectASType, "Area", nameof(PlateBase.GetArea), LevelEnum.NoDefinition, eUnitType.kArea);
      InsertItem(dictionary, objectASType, "Circumference", nameof(PlateBase.GetCircumference), LevelEnum.NoDefinition, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Paint Area", nameof(PlateBase.GetPaintArea), LevelEnum.NoDefinition, eUnitType.kArea);
      InsertItem(dictionary, objectASType, "Rectangular", nameof(PlateBase.IsRectangular));
      InsertItem(dictionary, objectASType, "Vertex Manipulation Possible", nameof(PlateBase.IsVertexManipulationPossible));
      InsertItem(dictionary, objectASType, "Definition Plane Coordinate System", nameof(PlateBase.GetDefinitionPlaneCS));

      InsertItem(dictionary, "Weight", GetWeight, LevelEnum.NoDefinition, eUnitType.kWeight);
      InsertItem(dictionary, "Weight (Exact)", GetWeightExact, LevelEnum.NoDefinition, eUnitType.kWeight);

      return dictionary;
    }

    private object GetWeight(object plate)
    {
      //1 yields the weight, 2 the exact weight
      return ((PlateBase)plate).GetWeight(1);
    }

    private object GetWeightExact(object plate)
    {
      //1 yields the weight, 2 the exact weight
      return ((PlateBase)plate).GetWeight(2);
    }
  }
}
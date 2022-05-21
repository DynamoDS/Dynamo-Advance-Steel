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
    public override Type GetObjectType => typeof(PlateBase);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Plate Portioning", nameof(PlateBase.Portioning));
      InsertItem(dictionary, "Upper Plane", nameof(PlateBase.UpperPlane), LevelEnum.Default);
      InsertItem(dictionary, "Length", nameof(PlateBase.Length), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Width", nameof(PlateBase.Width), LevelEnum.Default, eUnitType.kDistance);
      InsertItem(dictionary, "Length Increment", nameof(PlateBase.LengthIncrement), eUnitType.kDistance);
      InsertItem(dictionary, "Radius", nameof(PlateBase.Radius), LevelEnum.Default);
      InsertItem(dictionary, "Radius Increment", nameof(PlateBase.RadIncrement));
      InsertItem(dictionary, "Lower Z Position", nameof(PlateBase.LowerZPos));
      InsertItem(dictionary, "Plate Normal", nameof(PlateBase.PlateNormal));
      InsertItem(dictionary, "Thickness", nameof(PlateBase.Thickness));
      InsertItem(dictionary, "Lower Plane", nameof(PlateBase.LowerPlane), LevelEnum.Default);
      InsertItem(dictionary, "Upper Z Position", nameof(PlateBase.UpperZPos), LevelEnum.Default);
      InsertItem(dictionary, "Top Is Z Positive", nameof(PlateBase.TopIsZPositive));
      InsertItem(dictionary, "Definition Plane", nameof(PlateBase.DefinitionPlane));

      InsertItem(dictionary, "Area", nameof(PlateBase.GetArea), eUnitType.kArea);
      InsertItem(dictionary, "Circumference", nameof(PlateBase.GetCircumference), eUnitType.kDistance);
      InsertItem(dictionary, "Paint Area", nameof(PlateBase.GetPaintArea), eUnitType.kArea);
      InsertItem(dictionary, "Rectangular", nameof(PlateBase.IsRectangular));
      InsertItem(dictionary, "Vertex Manipulation Possible", nameof(PlateBase.IsVertexManipulationPossible));
      InsertItem(dictionary, "Definition Plane Coordinate System", nameof(PlateBase.GetDefinitionPlaneCS));

      InsertItem(dictionary, "Weight", GetWeight, eUnitType.kWeight);
      InsertItem(dictionary, "Weight (Exact)", GetWeightExact, eUnitType.kWeight);

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
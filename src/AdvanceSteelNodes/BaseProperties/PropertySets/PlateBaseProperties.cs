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
  public class PlateBaseProperties : BaseProperties<PlateBase>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Plate Portioning", nameof(PlateBase.Portioning));
      InsertProperty(dictionary, "Upper Plane", nameof(PlateBase.UpperPlane), LevelEnum.Default);
      InsertProperty(dictionary, "Length", nameof(PlateBase.Length), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Width", nameof(PlateBase.Width), LevelEnum.Default, eUnitType.kDistance);
      InsertProperty(dictionary, "Length Increment", nameof(PlateBase.LengthIncrement), eUnitType.kDistance);
      InsertProperty(dictionary, "Radius", nameof(PlateBase.Radius), LevelEnum.Default);
      InsertProperty(dictionary, "Radius Increment", nameof(PlateBase.RadIncrement));
      InsertProperty(dictionary, "Lower Z Position", nameof(PlateBase.LowerZPos));
      InsertProperty(dictionary, "Plate Normal", nameof(PlateBase.PlateNormal));
      InsertProperty(dictionary, "Thickness", nameof(PlateBase.Thickness));
      InsertProperty(dictionary, "Lower Plane", nameof(PlateBase.LowerPlane), LevelEnum.Default);
      InsertProperty(dictionary, "Upper Z Position", nameof(PlateBase.UpperZPos), LevelEnum.Default);
      InsertProperty(dictionary, "Top Is Z Positive", nameof(PlateBase.TopIsZPositive));
      InsertProperty(dictionary, "Definition Plane", nameof(PlateBase.DefinitionPlane));

      InsertProperty(dictionary, "Area", nameof(PlateBase.GetArea), eUnitType.kArea);
      InsertProperty(dictionary, "Circumference", nameof(PlateBase.GetCircumference), eUnitType.kDistance);
      InsertProperty(dictionary, "Paint Area", nameof(PlateBase.GetPaintArea), eUnitType.kArea);
      InsertProperty(dictionary, "Rectangular", nameof(PlateBase.IsRectangular));
      InsertProperty(dictionary, "Vertex Manipulation Possible", nameof(PlateBase.IsVertexManipulationPossible));
      InsertProperty(dictionary, "Definition Plane Coordinate System", nameof(PlateBase.GetDefinitionPlaneCS));

      InsertCustomProperty(dictionary, "Weight", nameof(PlateBaseProperties.GetWeight), null, eUnitType.kWeight);
      InsertCustomProperty(dictionary, "Weight (Exact)", nameof(PlateBaseProperties.GetWeightExact), null, eUnitType.kWeight);
      InsertCustomProperty(dictionary, "Weight (Fast)", nameof(PlateBaseProperties.GetWeightFast), null, eUnitType.kWeight);

      InsertCustomProperty(dictionary, "Physical Length And Width", nameof(PlateBaseProperties.GetPhysicalLengthAndWidth), null);

      return dictionary;
    }

    private static double GetWeight(PlateBase plate)
    {
      //1 yields the weight, 2 the exact weight
      return plate.GetWeight(1);
    }

    private static double GetWeightExact(PlateBase plate)
    {
      //1 yields the weight, 2 the exact weight
      return plate.GetWeight(2);
    }

    private static double GetWeightFast(PlateBase plate)
    {
      //3 the fast weight
      return plate.GetWeight(3);
    }

    public static Dictionary<string, double> GetPhysicalLengthAndWidth(PlateBase plate)
    {
      Dictionary<string, double> ret = new Dictionary<string, double>();

      plate.GetPhysLengthAndWidth(out var length, out var width);
      ret.Add("Length", Utils.FromInternalDistanceUnits(length, true));
      ret.Add("Width", Utils.FromInternalDistanceUnits(width, true));

      return ret;
    }

  }
}
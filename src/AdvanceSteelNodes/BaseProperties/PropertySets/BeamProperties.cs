using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ASBeam = Autodesk.AdvanceSteel.Modelling.Beam;
using ASPolyBeam = Autodesk.AdvanceSteel.Modelling.PolyBeam;

using ASPoint3d = Autodesk.AdvanceSteel.Geometry.Point3d;
using ASVector3d = Autodesk.AdvanceSteel.Geometry.Vector3d;

using DSPoint = Autodesk.DesignScript.Geometry.Point;

using AsType = Autodesk.AdvanceSteel.CADAccess.FilerObject.eObjectType;
using Autodesk.AdvanceSteel.CADAccess;
using static Autodesk.AdvanceSteel.DotNetRoots.Units.Unit;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;
using Autodesk.AdvanceSteel.Modelling;
using static Autodesk.AdvanceSteel.Modelling.Beam;

namespace AdvanceSteel.Nodes
{
  public class BeamProperties : BaseProperties<Beam>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Coordinate System at System Mid", nameof(ASBeam.SysCSMid), LevelEnum.Default);
      InsertProperty(dictionary, "Profile Section Name", nameof(ASBeam.ProfSectionName), LevelEnum.Default);
      InsertProperty(dictionary, "Coordinate System at Physical End", nameof(ASBeam.PhysCSEnd), LevelEnum.Default);
      InsertProperty(dictionary, "Profile Section Type", nameof(ASBeam.ProfSectionType));
      InsertProperty(dictionary, "Systemline Length", nameof(ASBeam.SysLength), LevelEnum.Default);
      InsertProperty(dictionary, "Deviation", nameof(ASBeam.Deviation));
      InsertProperty(dictionary, "Beam Shrink Value", nameof(ASBeam.ShrinkValue), LevelEnum.Default);
      InsertProperty(dictionary, "Coordinate System at System Start", nameof(ASBeam.SysCSStart), LevelEnum.Default);
      InsertProperty(dictionary, "Angle (Radians)", nameof(ASBeam.Angle), LevelEnum.Default, eUnitType.kAngle);
      InsertProperty(dictionary, "Profile Name", nameof(ASBeam.ProfName));
      InsertProperty(dictionary, "Coordinate System at System End", nameof(ASBeam.SysCSEnd), LevelEnum.Default);
      InsertProperty(dictionary, "Beam Runname", nameof(ASBeam.Runname), LevelEnum.Default);
      InsertProperty(dictionary, "Coordinate System at Physical Start", nameof(ASBeam.PhysCSStart), LevelEnum.Default);
      InsertProperty(dictionary, "Coordinate System at Physical Mid", nameof(ASBeam.PhysCSMid), LevelEnum.Default);
      InsertProperty(dictionary, "Offsets", nameof(ASBeam.Offsets));
      InsertProperty(dictionary, "Length", nameof(ASBeam.GetLength), eUnitType.kDistance);
      InsertProperty(dictionary, "Weight (Per Meter)", nameof(ASBeam.GetWeightPerMeter), eUnitType.kWeightPerDistance);
      InsertProperty(dictionary, "Paint Area", nameof(ASBeam.GetPaintArea), eUnitType.kArea);

      InsertCustomProperty(dictionary, "Start Point", nameof(BeamProperties.GetPointAtStart), nameof(BeamProperties.SetPointAtStart));
      InsertCustomProperty(dictionary, "End Point", nameof(BeamProperties.GetPointAtEnd), nameof(BeamProperties.SetPointAtEnd));
      InsertCustomProperty(dictionary, "Is Cross Section Mirrored", nameof(BeamProperties.IsCrossSectionMirrored), nameof(BeamProperties.SetIsCrossSectionMirrored), LevelEnum.Default);
      InsertCustomProperty(dictionary, "Reference Axis Description", nameof(BeamProperties.GetReferenceAxisDescription), null);
      InsertCustomProperty(dictionary, "Reference Axis", nameof(BeamProperties.GetReferenceAxis), nameof(BeamProperties.SetReferenceAxis));
      InsertCustomProperty(dictionary, "Weight", nameof(BeamProperties.GetWeight), null, eUnitType.kWeight);
      InsertCustomProperty(dictionary, "Weight (Exact)", nameof(BeamProperties.GetWeightExact), null, eUnitType.kWeight);
      InsertCustomProperty(dictionary, "Weight (Fast)", nameof(BeamProperties.GetWeightFast), null, eUnitType.kWeight);
      InsertCustomProperty(dictionary, "Beam Points", nameof(BeamProperties.GetListPoints), null);
      InsertCustomProperty(dictionary, "Beam Line", nameof(BeamProperties.GetLine), null);
      InsertCustomProperty(dictionary, "Profile Type Code", nameof(BeamProperties.GetProfileTypeCode), null);
      InsertCustomProperty(dictionary, "Profile Type", nameof(BeamProperties.GetProfileType), null);
      InsertCustomProperty(dictionary, "Saw Length", nameof(BeamProperties.GetSawLength), null, eUnitType.kAreaPerDistance);
      InsertCustomProperty(dictionary, "Flange Angle At Start", nameof(BeamProperties.GetFlangeAngleAtStart), null, eUnitType.kAngle);
      InsertCustomProperty(dictionary, "Flange Angle At End", nameof(BeamProperties.GetFlangeAngleAtEnd), null, eUnitType.kAngle);
      InsertCustomProperty(dictionary, "Web Angle At Start", nameof(BeamProperties.GetWebAngleAtStart), null, eUnitType.kAngle);
      InsertCustomProperty(dictionary, "Web Angle At End", nameof(BeamProperties.GetWebAngleAtEnd), null, eUnitType.kAngle);
      InsertCustomProperty(dictionary, "Saw Information", nameof(BeamProperties.GetSawInformationComplete), null);

      return dictionary;
    }

    private static ASPoint3d GetPointAtStart(ASBeam beam)
    {
      return beam.GetPointAtStart();
    }

    private static void SetPointAtStart(ASBeam beam, ASPoint3d startPoint)
    {
      beam.SetSysStart(startPoint);
    }

    private static ASPoint3d GetPointAtEnd(ASBeam beam)
    {
      return beam.GetPointAtEnd();
    }

    private static void SetPointAtEnd(ASBeam beam, ASPoint3d endPoint)
    {
      beam.SetSysEnd(endPoint);
    }

    private static bool IsCrossSectionMirrored(ASBeam beam)
    {
      return beam.IsCrossSectionMirrored;
    }

    private static void SetIsCrossSectionMirrored(ASBeam beam, bool isCrossSectionMirrored)
    {
      beam.SetCrossSectionMirrored(isCrossSectionMirrored);
    }

    private static string GetReferenceAxisDescription(ASBeam beam)
    {
      return beam.RefAxis.ToString();
    }

    private static int GetReferenceAxis(ASBeam beam)
    {
      return (int)beam.RefAxis;
    }

    private static void SetReferenceAxis(ASBeam beam, int refAxis)
    {
      beam.RefAxis = (eRefAxis)refAxis;
    }

    private static double GetWeight(ASBeam beam)
    {
      //1 yields the weight, 2 the exact weight
      return RoundWeight(beam.GetWeight(1));
    }

    private static double GetWeightExact(ASBeam beam)
    {
      //1 yields the weight, 2 the exact weight
      return RoundWeight(beam.GetWeight(2));
    }

    private static double GetWeightFast(ASBeam beam)
    {
      //3 the fast weight
      return RoundWeight(beam.GetWeight(3));
    }

    private static double RoundWeight(double value)
    {
      return Math.Round(value, 5, MidpointRounding.AwayFromZero);
    }

    private static List<DSPoint> GetListPoints(ASBeam beam)
    {
      List<DSPoint> pointList = new List<DSPoint>();

      if (beam is ASPolyBeam)
      {
        ASPolyBeam polyBeam = beam as ASPolyBeam;

        var polyLine = polyBeam.GetPolyline(true);
        foreach (var item in polyLine.Vertices)
          pointList.Add(item.ToDynPoint());
      }
      else
      {
        pointList.Add(beam.GetPointAtStart().ToDynPoint());
        pointList.Add(beam.GetPointAtEnd().ToDynPoint());
      }

      return pointList;
    }

    private static Autodesk.DesignScript.Geometry.Line GetLine(ASBeam beam)
    {
      return Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(beam.GetPointAtStart().ToDynPoint(), beam.GetPointAtEnd().ToDynPoint());
    }

    private static string GetProfileTypeCode(ASBeam beam)
    {
      return beam.GetProfType().GetDSTVValues().GetProfileTypeString();
    }

    private static int GetProfileType(ASBeam beam)
    {
      return (int)beam.GetProfType().GetDSTVValues().DSTVType;
    }

    private static double GetSawLength(ASBeam beam)
    {
      GetSawInformation(beam, out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return sawLength;
    }

    private static double GetFlangeAngleAtStart(ASBeam beam)
    {
      GetSawInformation(beam, out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return Utils.DegreeToRad(flangeAngleAtStart);
    }

    private static double GetWebAngleAtStart(ASBeam beam)
    {
      GetSawInformation(beam, out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return Utils.DegreeToRad(webAngleAtStart);
    }

    private static double GetFlangeAngleAtEnd(ASBeam beam)
    {
      GetSawInformation(beam, out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return Utils.DegreeToRad(flangeAngleAtEnd);
    }

    private static double GetWebAngleAtEnd(ASBeam beam)
    {
      GetSawInformation(beam, out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return Utils.DegreeToRad(webAngleAtEnd);
    }

    private static Dictionary<string, double> GetSawInformationComplete(ASBeam beam)
    {
      Dictionary<string, double> ret = new Dictionary<string, double>();

      double sawLength = 0;
      double flangeAngleAtStart = 0;
      double webAngleAtStart = 0;
      double flangeAngleAtEnd = 0;
      double webAngleAtEnd = 0;
      ret.Add("SawLength", Utils.FromInternalDistanceUnits(sawLength, true));
      ret.Add("FlangeAngleAtStart", flangeAngleAtStart);
      ret.Add("WebAngleAtStart", webAngleAtStart);
      ret.Add("FlangeAngleAtEnd", flangeAngleAtEnd);
      ret.Add("WebAngleAtEnd", webAngleAtEnd);

      GetSawInformation(beam, out sawLength, out flangeAngleAtStart, out webAngleAtStart, out flangeAngleAtEnd, out webAngleAtEnd);

      ret["SawLength"] = Utils.FromInternalDistanceUnits(sawLength, true);
      ret["FlangeAngleAtStart"] = Utils.FromInternalAngleUnits(Utils.DegreeToRad(flangeAngleAtStart), true);
      ret["WebAngleAtStart"] = Utils.FromInternalAngleUnits(Utils.DegreeToRad(webAngleAtStart), true);
      ret["FlangeAngleAtEnd"] = Utils.FromInternalAngleUnits(Utils.DegreeToRad(flangeAngleAtEnd), true);
      ret["WebAngleAtEnd"] = Utils.FromInternalAngleUnits(Utils.DegreeToRad(webAngleAtEnd), true);

      return ret;
    }

    private static void GetSawInformation(ASBeam beam, out double sawLength, out double flangeAngleAtStart, out double webAngleAtStart, out double flangeAngleAtEnd, out double webAngleAtEnd)
    {
      int executed = beam.GetSawInformation(out sawLength, out flangeAngleAtStart, out webAngleAtStart, out flangeAngleAtEnd, out webAngleAtEnd);
      if (executed <= 0)
      {
        throw new System.Exception("No Values were found for Steel Beam from Function");
      }
    }
  }
}

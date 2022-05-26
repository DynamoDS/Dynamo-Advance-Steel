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
      InsertProperty(dictionary, "Is Cross Section Mirrored", nameof(ASBeam.IsCrossSectionMirrored), LevelEnum.Default);
      InsertProperty(dictionary, "Angle (Radians)", nameof(ASBeam.Angle), LevelEnum.Default, eUnitType.kAngle);
      InsertProperty(dictionary, "Profile Name", nameof(ASBeam.ProfName));
      InsertProperty(dictionary, "Coordinate System at System End", nameof(ASBeam.SysCSEnd), LevelEnum.Default);
      InsertProperty(dictionary, "Beam Runname", nameof(ASBeam.Runname), LevelEnum.Default);
      InsertProperty(dictionary, "Coordinate System at Physical Start", nameof(ASBeam.PhysCSStart), LevelEnum.Default);
      InsertProperty(dictionary, "Coordinate System at Physical Mid", nameof(ASBeam.PhysCSMid), LevelEnum.Default);
      InsertProperty(dictionary, "Offsets", nameof(ASBeam.Offsets));
      InsertProperty(dictionary, "Length", nameof(ASBeam.GetLength), eUnitType.kDistance);
      InsertCustomProperty(dictionary, "Weight", nameof(BeamProperties.GetWeight), null, eUnitType.kWeight);
      InsertCustomProperty(dictionary, "Weight (Exact)", nameof(BeamProperties.GetWeightExact), null, eUnitType.kWeight);
      InsertProperty(dictionary, "Weight (Per Meter)", nameof(ASBeam.GetWeightPerMeter), eUnitType.kWeight);
      InsertProperty(dictionary, "Start Point", nameof(ASBeam.GetPointAtStart));
      InsertProperty(dictionary, "End Point", nameof(ASBeam.GetPointAtEnd));
      InsertCustomProperty(dictionary, "Beam Points", nameof(BeamProperties.GetListPoints), null);
      InsertCustomProperty(dictionary, "Beam Line", nameof(BeamProperties.GetLine), null);
      InsertProperty(dictionary, "Paint Area", nameof(ASBeam.GetPaintArea), eUnitType.kArea);
      InsertCustomProperty(dictionary, "Profile Type Code", nameof(BeamProperties.GetProfileTypeCode), null);
      InsertCustomProperty(dictionary, "Profile Type", nameof(BeamProperties.GetProfileType), null);
      InsertCustomProperty(dictionary, "Saw Length", nameof(BeamProperties.GetSawLength), null, eUnitType.kAreaPerDistance);
      InsertCustomProperty(dictionary, "Flange Angle At Start", nameof(BeamProperties.GetFlangeAngleAtStart), null, eUnitType.kAngle);
      InsertCustomProperty(dictionary, "Flange Angle At End", nameof(BeamProperties.GetFlangeAngleAtEnd), null, eUnitType.kAngle);
      InsertCustomProperty(dictionary, "Web Angle At Start", nameof(BeamProperties.GetWebAngleAtStart), null, eUnitType.kAngle);
      InsertCustomProperty(dictionary, "Web Angle At End", nameof(BeamProperties.GetWebAngleAtEnd), null, eUnitType.kAngle);

      return dictionary;
    }

    private double GetWeight(ASBeam beam)
    {
      //1 yields the weight, 2 the exact weight
      return RoundWeight(beam.GetWeight(1));
    }

    private double GetWeightExact(ASBeam beam)
    {
      //1 yields the weight, 2 the exact weight
      return RoundWeight(beam.GetWeight(2));
    }

    private double RoundWeight(double value)
    {
      return Math.Round(value, 5, MidpointRounding.AwayFromZero);
    }

    private List<DSPoint> GetListPoints(ASBeam beam)
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

    private Autodesk.DesignScript.Geometry.Line GetLine(ASBeam beam)
    {
      return Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(beam.GetPointAtStart().ToDynPoint(), beam.GetPointAtEnd().ToDynPoint());
    }

    private string GetProfileTypeCode(ASBeam beam)
    {
      return beam.GetProfType().GetDSTVValues().GetProfileTypeString();
    }

    private int GetProfileType(ASBeam beam)
    {
      return (int)beam.GetProfType().GetDSTVValues().DSTVType;
    }

    private double GetSawLength(ASBeam beam)
    {
      beam.GetSawInformation(out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return sawLength;
    }

    private double GetFlangeAngleAtStart(ASBeam beam)
    {
      beam.GetSawInformation(out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return Utils.DegreeToRad(flangeAngleAtStart);
    }

    private double GetWebAngleAtStart(ASBeam beam)
    {
      beam.GetSawInformation(out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return Utils.DegreeToRad(webAngleAtStart);
    }

    private double GetFlangeAngleAtEnd(ASBeam beam)
    {
      beam.GetSawInformation(out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return Utils.DegreeToRad(flangeAngleAtEnd);
    }

    private double GetWebAngleAtEnd(ASBeam beam)
    {
      beam.GetSawInformation(out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return Utils.DegreeToRad(webAngleAtEnd);
    }
  }
}

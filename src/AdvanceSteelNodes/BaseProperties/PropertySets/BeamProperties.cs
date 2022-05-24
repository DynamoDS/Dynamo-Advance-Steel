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

      InsertItem(dictionary, "Coordinate System at System Mid", nameof(ASBeam.SysCSMid), LevelEnum.Default);
      InsertItem(dictionary, "Profile Section Name", nameof(ASBeam.ProfSectionName), LevelEnum.Default);
      InsertItem(dictionary, "Coordinate System at Physical End", nameof(ASBeam.PhysCSEnd), LevelEnum.Default);
      InsertItem(dictionary, "Profile Section Type", nameof(ASBeam.ProfSectionType));
      InsertItem(dictionary, "Systemline Length", nameof(ASBeam.SysLength), LevelEnum.Default);
      InsertItem(dictionary, "Deviation", nameof(ASBeam.Deviation));
      InsertItem(dictionary, "Beam Shrink Value", nameof(ASBeam.ShrinkValue), LevelEnum.Default);
      InsertItem(dictionary, "Coordinate System at System Start", nameof(ASBeam.SysCSStart), LevelEnum.Default);
      InsertItem(dictionary, "Is Cross Section Mirrored", nameof(ASBeam.IsCrossSectionMirrored), LevelEnum.Default);
      InsertItem(dictionary, "Angle (Radians)", nameof(ASBeam.Angle), LevelEnum.Default, eUnitType.kAngle);
      InsertItem(dictionary, "Profile Name", nameof(ASBeam.ProfName));
      InsertItem(dictionary, "Coordinate System at System End", nameof(ASBeam.SysCSEnd), LevelEnum.Default);
      InsertItem(dictionary, "Beam Runname", nameof(ASBeam.Runname), LevelEnum.Default);
      InsertItem(dictionary, "Coordinate System at Physical Start", nameof(ASBeam.PhysCSStart), LevelEnum.Default);
      InsertItem(dictionary, "Coordinate System at Physical Mid", nameof(ASBeam.PhysCSMid), LevelEnum.Default);
      InsertItem(dictionary, "Offsets", nameof(ASBeam.Offsets));
      InsertItem(dictionary, "Length", nameof(ASBeam.GetLength), eUnitType.kDistance);
      InsertItem(dictionary, "Weight", GetWeight, eUnitType.kWeight);
      InsertItem(dictionary, "Weight (Exact)", GetWeightExact, eUnitType.kWeight);
      InsertItem(dictionary, "Weight (Per Meter)", nameof(ASBeam.GetWeightPerMeter), eUnitType.kWeight);
      InsertItem(dictionary, "Start Point", nameof(ASBeam.GetPointAtStart));
      InsertItem(dictionary, "End Point", nameof(ASBeam.GetPointAtEnd));
      InsertItem(dictionary, "Beam Points", GetListPoints);
      InsertItem(dictionary, "Beam Line", GetLine);
      InsertItem(dictionary, "Paint Area", nameof(ASBeam.GetPaintArea), eUnitType.kArea);
      InsertItem(dictionary, "Profile Type Code", GetProfileTypeCode);
      InsertItem(dictionary, "Profile Type", GetProfileType);
      InsertItem(dictionary, "Saw Length", GetSawLength, eUnitType.kAreaPerDistance);
      InsertItem(dictionary, "Flange Angle At Start", GetFlangeAngleAtStart, eUnitType.kAngle);
      InsertItem(dictionary, "Flange Angle At End", GetFlangeAngleAtEnd, eUnitType.kAngle);
      InsertItem(dictionary, "Web Angle At Start", GetWebAngleAtStart, eUnitType.kAngle);
      InsertItem(dictionary, "Web Angle At End", GetWebAngleAtEnd, eUnitType.kAngle);

      return dictionary;
    }

    private object GetWeight(object beam)
    {
      //1 yields the weight, 2 the exact weight
      return RoundWeight(((ASBeam)beam).GetWeight(1));
    }

    private object GetWeightExact(object beam)
    {
      //1 yields the weight, 2 the exact weight
      return RoundWeight(((ASBeam)beam).GetWeight(2));
    }

    private double RoundWeight(double value)
    {
      return Math.Round(value, 5, MidpointRounding.AwayFromZero);
    }

    private object GetListPoints(object beam)
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
        ASBeam asBeam = beam as ASBeam;

        pointList.Add(asBeam.GetPointAtStart().ToDynPoint());
        pointList.Add(asBeam.GetPointAtEnd().ToDynPoint());
      }

      return pointList;
    }

    private object GetLine(object beam)
    {
      ASBeam asBeam = beam as ASBeam;

      return Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(asBeam.GetPointAtStart().ToDynPoint(), asBeam.GetPointAtEnd().ToDynPoint());
    }

    private object GetProfileTypeCode(object beam)
    {
      return ((ASBeam)beam).GetProfType().GetDSTVValues().GetProfileTypeString();
    }

    private object GetProfileType(object beam)
    {
      return (int)((ASBeam)beam).GetProfType().GetDSTVValues().DSTVType;
    }

    private object GetSawLength(object beam)
    {
      ((ASBeam)beam).GetSawInformation(out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return sawLength;
    }

    private object GetFlangeAngleAtStart(object beam)
    {
      ((ASBeam)beam).GetSawInformation(out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return Utils.DegreeToRad(flangeAngleAtStart);
    }

    private object GetWebAngleAtStart(object beam)
    {
      ((ASBeam)beam).GetSawInformation(out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return Utils.DegreeToRad(webAngleAtStart);
    }

    private object GetFlangeAngleAtEnd(object beam)
    {
      ((ASBeam)beam).GetSawInformation(out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return Utils.DegreeToRad(flangeAngleAtEnd);
    }

    private object GetWebAngleAtEnd(object beam)
    {
      ((ASBeam)beam).GetSawInformation(out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return Utils.DegreeToRad(webAngleAtEnd);
    }
  }
}

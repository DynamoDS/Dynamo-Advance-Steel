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

namespace AdvanceSteel.Nodes
{
  public class BeamProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kBeam;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Coordinate System at System Mid", nameof(ASBeam.SysCSMid), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Profile Section Name", nameof(ASBeam.ProfSectionName), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Coordinate System at Physical End", nameof(ASBeam.PhysCSEnd), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Profile Section Type", nameof(ASBeam.ProfSectionType));
      InsertItem(dictionary, objectASType, "Systemline Length", nameof(ASBeam.SysLength), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Deviation", nameof(ASBeam.Deviation));
      InsertItem(dictionary, objectASType, "Beam Shrink Value", nameof(ASBeam.ShrinkValue), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Coordinate System at System Start", nameof(ASBeam.SysCSStart), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Is Cross Section Mirrored", nameof(ASBeam.IsCrossSectionMirrored), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Angle (Radians)", nameof(ASBeam.Angle), LevelEnum.Default, eUnitType.kAngle);
      InsertItem(dictionary, objectASType, "Profile Name", nameof(ASBeam.ProfName));
      InsertItem(dictionary, objectASType, "Coordinate System at System End", nameof(ASBeam.SysCSEnd), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Beam Runname", nameof(ASBeam.Runname), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Coordinate System at Physical Start", nameof(ASBeam.PhysCSStart), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Coordinate System at Physical Mid", nameof(ASBeam.PhysCSMid), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Offsets", nameof(ASBeam.Offsets));
      InsertItem(dictionary, objectASType, "Length", nameof(ASBeam.GetLength), eUnitType.kDistance);
      InsertItem(dictionary, "Weight", GetWeight, eUnitType.kWeight);
      InsertItem(dictionary, "Weight (Exact)", GetWeightExact, eUnitType.kWeight);
      InsertItem(dictionary, objectASType, "Weight (Per Meter)", nameof(ASBeam.GetWeightPerMeter), eUnitType.kWeight);
      InsertItem(dictionary, objectASType, "Start Point", nameof(ASBeam.GetPointAtStart));
      InsertItem(dictionary, objectASType, "End Point", nameof(ASBeam.GetPointAtEnd));
      InsertItem(dictionary, "Beam Points", GetListPoints);
      InsertItem(dictionary, "Line", GetLine);
      InsertItem(dictionary, objectASType, "Paint Area", nameof(ASBeam.GetPaintArea), eUnitType.kArea);
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

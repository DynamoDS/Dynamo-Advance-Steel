using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.AdvanceSteel.Profiles;
using Autodesk.DesignScript.Runtime;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Beams
{
  /// <summary>
  /// Advance Steel straight beams
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class StraightBeam : GraphicObject
  {
    internal StraightBeam(Autodesk.DesignScript.Geometry.Point ptStart, Autodesk.DesignScript.Geometry.Point ptEnd, Autodesk.DesignScript.Geometry.Vector vOrientation, 
                          string modelRole, string sectionName, int refAxis, bool crossSectionMirror)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Point3d beamStart = Utils.ToAstPoint(ptStart, true);
          Point3d beamEnd = Utils.ToAstPoint(ptEnd, true);
          Vector3d refVect = Utils.ToAstVector3d(vOrientation, true);

          if (string.IsNullOrEmpty(sectionName))
          {
            ProfileName profName = new ProfileName();
            ProfilesManager.GetProfTypeAsDefault("I", out profName);
            sectionName = profName.Name;
          }

          Autodesk.AdvanceSteel.Modelling.StraightBeam beam = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {

            beam = new Autodesk.AdvanceSteel.Modelling.StraightBeam(sectionName, beamStart, beamEnd, refVect);
            if (!string.IsNullOrEmpty(modelRole))
            {
              beam.Role = modelRole;
            }
            if (Beam.eRefAxis.IsDefined(typeof(Beam.eRefAxis), refAxis) == true)
            {
              beam.RefAxis = (Beam.eRefAxis)refAxis;
            }
            beam.SetCrossSectionMirrored(crossSectionMirror, false);
            beam.WriteToDb();

          }
          else
          {
            beam = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.StraightBeam;

            if (beam != null && beam.IsKindOf(FilerObject.eObjectType.kStraightBeam))
            {
              string sectionType = Utils.SplitSectionName(sectionName)[0];
              string sectionSize = Utils.SplitSectionName(sectionName)[1];
              Utils.AdjustBeamEnd(beam, beamStart);
              beam.SetSysStart(beamStart);
              beam.SetSysEnd(beamEnd);
              beam.ChangeProfile(sectionType, sectionSize);
              if (!string.IsNullOrEmpty(modelRole))
              {
                beam.Role = modelRole;
              }
              if (Beam.eRefAxis.IsDefined(typeof(Beam.eRefAxis), refAxis) == true)
              {
                beam.RefAxis = (Beam.eRefAxis)refAxis;
              }
              beam.SetCrossSectionMirrored(crossSectionMirror, false);
              Utils.SetOrientation(beam, refVect);
            }
            else
              throw new System.Exception("Not a straight Beam");
          }
          Handle = beam.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(beam);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel straight beam between two points
    /// </summary>
    /// <param name="start">Input Start point of Beam</param>
    /// <param name="end">Input End point of Beam</param>
    /// <param name="orientation">Section orientation</param>
    /// <returns></returns>
    public static StraightBeam ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Point end, Autodesk.DesignScript.Geometry.Vector orientation)
    {
      return new StraightBeam(start, end, orientation, "", "", -1, false);
    }

    /// <summary>
    /// Create an Advance Steel straight beam between two points from a Dynamo Line
    /// </summary>
    /// <param name="line">Inpu Dynamo Line to get start and end points from</param>
    /// <param name="orientation">Section orientation</param>
    /// <returns></returns>
    public static StraightBeam ByLine(Autodesk.DesignScript.Geometry.Line line, Autodesk.DesignScript.Geometry.Vector orientation)
    {
      return new StraightBeam(line.StartPoint, line.EndPoint, orientation, "", "", -1, false);
    }

    /// <summary>
    /// Create an Advance Steel straight beam from a Point, direction and length
    /// </summary>
    /// <param name="start">Input Start point of Beam</param>
    /// <param name="direction">Input vector direction of beam</param>
    /// <param name="orientation">Section orientation</param>
    /// <param name="length">Input Beam Length relative to Start Point</param>
    /// <returns></returns>
    public static StraightBeam ByStartPointDirectionLength(Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Vector direction, 
                                                            Autodesk.DesignScript.Geometry.Vector orientation, [DefaultArgument("1000;")]double length)
    {
      Vector3d columnDirection = Utils.ToAstVector3d(direction, true).Normalize();
      Point3d tempPoint = Utils.ToAstPoint(start, true);
      Point3d end = tempPoint.Add(columnDirection * length);
      return new StraightBeam(start, Utils.ToDynPoint(end, true), orientation, "", "", -1, false);
    }

    /// <summary>
    /// Create an Advance Steel straight beam from a Point, direction and length and Ref Axis
    /// </summary>
    /// <param name="start">Input Start point of Beam</param>
    /// <param name="direction">Input vector direction of beam</param>
    /// <param name="orientation">Section orientation</param>
    /// <param name="refAxis">Input Beam reference axis UpperLeft = 0, UpperSys = 1, UpperRight = 2, MidLeft = 3, SysSys = 4, MidRight = 5, LowerLeft = 6, LowerSys = 7, LowerRight = 8, ContourCenter = 9</param>
    /// <param name="length">Input Beam Length relative to Start Point</param>
    /// <returns></returns>
    public static StraightBeam ByStartPointDirectionLength(Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Vector direction, 
                                                           Autodesk.DesignScript.Geometry.Vector orientation, [DefaultArgument("5;")]int refAxis, [DefaultArgument("1000;")]double length)
    {
      Vector3d columnDirection = Utils.ToAstVector3d(direction, true).Normalize();
      Point3d tempPoint = Utils.ToAstPoint(start, true);
      Point3d end = tempPoint.Add(columnDirection * length);
      return new StraightBeam(start, Utils.ToDynPoint(end, true), orientation, "", "", refAxis, false);
    }

    /// <summary>
    /// Create an Advance Steel straight beam between two points - inc ModelRole and Section Size
    /// </summary>
    /// <param name="start">Input Start point of Beam</param>
    /// <param name="end">Input End point of Beam</param>
    /// <param name="orientation">Section orientation</param>
    /// <param name="modelRole">Input Beam Model Role - Key Column of Model Table</param>
    /// <param name="sectionName">Input Beam Section size</param>
    /// <param name="refAxis">Input Beam reference axis UpperLeft = 0, UpperSys = 1, UpperRight = 2, MidLeft = 3, SysSys = 4, MidRight = 5, LowerLeft = 6, LowerSys = 7, LowerRight = 8, ContourCenter = 9</param>
    /// <param name="crossSectionMirror">Input Beam Mirror Option</param>
    /// <returns></returns>
    public static StraightBeam ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Point end, Autodesk.DesignScript.Geometry.Vector orientation,
                                                    [DefaultArgument("Horizontal_Beam;")]string modelRole, string sectionName, [DefaultArgument("5;")]int refAxis, [DefaultArgument("false;")]bool crossSectionMirror)
    {
      return new StraightBeam(start, end, orientation, modelRole, sectionName, refAxis, crossSectionMirror);
    }

    /// <summary>
    /// Create an Advance Steel straight beam between two points from a Dynamo Line - inc ModelRole and Section Size
    /// </summary>
    /// <param name="line">Inpu Dynamo Line to get start and end points from</param>
    /// <param name="orientation">Section orientation</param>
    /// <param name="modelRole">Input Beam Model Role - Key Column of Model Table</param>
    /// <param name="sectionName">Input Beam Section size</param>
    /// <param name="refAxis">Input Beam reference axis UpperLeft = 0, UpperSys = 1, UpperRight = 2, MidLeft = 3, SysSys = 4, MidRight = 5, LowerLeft = 6, LowerSys = 7, LowerRight = 8, ContourCenter = 9</param>
    /// <param name="crossSectionMirror">Input Beam Mirror Option</param>
    /// <returns></returns>
    public static StraightBeam ByLine(Autodesk.DesignScript.Geometry.Line line, [DefaultArgument("Autodesk.DesignScript.Geometry.Vector.ZAxis();")]Autodesk.DesignScript.Geometry.Vector orientation,
                                      [DefaultArgument("Horizontal_Beam;")]string modelRole, string sectionName, [DefaultArgument("5;")]int refAxis, [DefaultArgument("false;")]bool crossSectionMirror)
    {
      return new StraightBeam(line.StartPoint, line.EndPoint, orientation, modelRole, sectionName, refAxis, crossSectionMirror);
    }

    /// <summary>
    /// Create an Advance Steel straight beam from a Point, direction and length - inc ModelRole and Section Size
    /// </summary>
    /// <param name="start">Input Start point of Beam</param>
    /// <param name="direction">Input vector direction of beam</param>
    /// <param name="orientation">Section orientation</param>
    /// <param name="length">Input Beam Length relative to Start Point</param>
    /// <param name="modelRole">Input Beam Model Role - Key Column of Model Table</param>
    /// <param name="sectionName">Input Beam Section size</param>
    /// <param name="refAxis">Input Beam reference axis UpperLeft = 0, UpperSys = 1, UpperRight = 2, MidLeft = 3, SysSys = 4, MidRight = 5, LowerLeft = 6, LowerSys = 7, LowerRight = 8, ContourCenter = 9</param>
    /// <param name="crossSectionMirror">Input Beam Mirror Option</param>
    /// <returns></returns>
    public static StraightBeam ByStartPointDirectionLength(Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Vector direction, Autodesk.DesignScript.Geometry.Vector orientation, double length,
                                                            [DefaultArgument("Horizontal_Beam;")]string modelRole, string sectionName, [DefaultArgument("5;")]int refAxis, [DefaultArgument("false;")]bool crossSectionMirror)
    {
      Vector3d columnDirection = Utils.ToAstVector3d(direction, true).Normalize();
      Point3d tempPoint = Utils.ToAstPoint(start, true);
      Point3d end = tempPoint.Add(columnDirection * length);
      return new StraightBeam(start, Utils.ToDynPoint(end, true), orientation, modelRole, sectionName, refAxis, crossSectionMirror);
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var beam = Utils.GetObject(Handle) as Beam;

          Point3d asPt1 = beam.GetPointAtStart(0);
          Point3d asPt2 = beam.GetPointAtEnd(0);

          using (var pt1 = Utils.ToDynPoint(beam.GetPointAtStart(0), true))
          using (var pt2 = Utils.ToDynPoint(beam.GetPointAtEnd(0), true))
          {
            var line = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(pt1, pt2);
            return line;
          }
        }
      }
    }
  }
}
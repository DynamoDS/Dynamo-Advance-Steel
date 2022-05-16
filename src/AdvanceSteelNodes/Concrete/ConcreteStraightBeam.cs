using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.AdvanceSteel.Profiles;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using ASConcreteBeam = Autodesk.AdvanceSteel.Modelling.ConcreteBeam;

namespace AdvanceSteel.Nodes.Concrete
{
  /// <summary>
  /// Advance Steel Concrete Straight Beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class ConcreteStraightBeam : GraphicObject
  {
    private ConcreteStraightBeam(string concName,
                              Autodesk.DesignScript.Geometry.Point ptStart,
                              Autodesk.DesignScript.Geometry.Point ptEnd,
                              Autodesk.DesignScript.Geometry.Vector vOrientation,
                              List<Property> concreteProperties)
    {
      SafeInit(() => InitConcreteStraightBeam(concName, ptStart, ptEnd, vOrientation, concreteProperties));
    }

    private ConcreteStraightBeam(ASConcreteBeam beam)
    {
      SafeInit(() => SetHandle(beam));
    }

    internal static ConcreteStraightBeam FromExisting(ASConcreteBeam beam)
    {
      return new ConcreteStraightBeam(beam)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitConcreteStraightBeam(string concName,
                              Autodesk.DesignScript.Geometry.Point ptStart,
                              Autodesk.DesignScript.Geometry.Point ptEnd,
                              Autodesk.DesignScript.Geometry.Vector vOrientation,
                              List<Property> concreteProperties)
    {
      List<Property> defaultData = concreteProperties.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> postWriteDBData = concreteProperties.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();

      Point3d beamStart = Utils.ToAstPoint(ptStart, true);
      Point3d beamEnd = Utils.ToAstPoint(ptEnd, true);
      Vector3d refVect = Utils.ToAstVector3d(vOrientation, true);

      ASConcreteBeam concBeam = SteelServices.ElementBinder.GetObjectASFromTrace<ASConcreteBeam>();
      if (concBeam == null)
      {
        concBeam = new ASConcreteBeam(concName, beamStart, beamEnd, refVect);
        if (defaultData != null)
        {
          Utils.SetParameters(concBeam, defaultData);
        }

        concBeam.WriteToDb();
      }
      else
      {
        if (!concBeam.IsKindOf(FilerObject.eObjectType.kConcreteBeam))
          throw new System.Exception("Not a Concrete Straight Beam");

        Utils.AdjustBeamEnd(concBeam, beamStart);
        concBeam.SetSysStart(beamStart);
        concBeam.SetSysEnd(beamEnd);
        concBeam.ProfName = concName;

        Utils.SetOrientation(concBeam, refVect);

        if (defaultData != null)
        {
          Utils.SetParameters(concBeam, defaultData);
        }
      }

      SetHandle(concBeam);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(concBeam, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(concBeam);
    }

    /// <summary>
    /// Create an Advance Steel Concrete Straight Beam/Column between two points
    /// </summary>
    /// <param name="concName"> Concrete Profile Name</param>
    /// <param name="start"> Input Start point of Beam</param>
    /// <param name="end"> Input End point of Beam</param>
    /// <param name="orientation"> Section orientation</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns name="beam"> beam</returns>
    public static ConcreteStraightBeam ByStartPointEndPoint(string concName, Autodesk.DesignScript.Geometry.Point start,
                                                        Autodesk.DesignScript.Geometry.Point end,
                                                        Autodesk.DesignScript.Geometry.Vector orientation,
                                                        [DefaultArgument("null")] List<Property> additionalConcParameters)
    {
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new ConcreteStraightBeam(concName, start, end, orientation, additionalConcParameters);
    }

    /// <summary>
    /// Create an Advance Steel Concrete Beam/Column by a point in a direction by length
    /// </summary>
    /// <param name="concName"> Concrete Profile Name</param>
    /// <param name="start"> Input Start point of Beam</param>
    /// <param name="direction"> Dynamo Vector to define length direction</param>
    /// <param name="orientation"> Section orientation</param>
    /// <param name="length"> Length value in the direction from the start point</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns name="beam"> beam</returns>
    public static ConcreteStraightBeam ByStartPointDirectionLength(string concName, Autodesk.DesignScript.Geometry.Point start,
                                                                Autodesk.DesignScript.Geometry.Vector direction,
                                                                Autodesk.DesignScript.Geometry.Vector orientation,
                                                                double length,
                                                                [DefaultArgument("null")] List<Property> additionalConcParameters)
    {
      Vector3d columnDirection = Utils.ToAstVector3d(direction, true).Normalize();
      Point3d tempPoint = Utils.ToAstPoint(start, true);
      Point3d end = tempPoint.Add(columnDirection * length);
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new ConcreteStraightBeam(concName, start, Utils.ToDynPoint(end, true), orientation, additionalConcParameters);
    }

    /// <summary>
    /// Create an Advance Steel Concrete Beam/Column by Dynamo Line Geometry
    /// </summary>
    /// <param name="concName"> Concrete Profile Name</param>
    /// <param name="line"> Input Dynamo Line</param>
    /// <param name="orientation">Section orientation</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns name="beam"> beam</returns>
    public static ConcreteStraightBeam ByLine(string concName, Autodesk.DesignScript.Geometry.Line line,
                                          Autodesk.DesignScript.Geometry.Vector orientation,
                                          [DefaultArgument("null")] List<Property> additionalConcParameters)
    {
      Autodesk.DesignScript.Geometry.Point start = line.StartPoint;
      Autodesk.DesignScript.Geometry.Point end = line.EndPoint;
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new ConcreteStraightBeam(concName, start, end, orientation, additionalConcParameters);
    }

    private static List<Property> PreSetDefaults(List<Property> listOfProps)
    {
      if (listOfProps == null)
      {
        listOfProps = new List<Property>() { };
      }
      return listOfProps;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      ASConcreteBeam beam = Utils.GetObject(Handle) as ASConcreteBeam;

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
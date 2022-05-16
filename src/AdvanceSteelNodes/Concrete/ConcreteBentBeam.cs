using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Profiles;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using ASConcreteBentBeam = Autodesk.AdvanceSteel.Modelling.ConcreteBentBeam;

namespace AdvanceSteel.Nodes.Concrete
{
  /// <summary>
  /// Advance Steel Concrete Bent Beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class ConcreteBentBeam : GraphicObject
  {
    private ConcreteBentBeam(string concName,
                          Autodesk.DesignScript.Geometry.Point ptStart,
                          Autodesk.DesignScript.Geometry.Point ptEnd,
                          Autodesk.DesignScript.Geometry.Point ptOnArc,
                          Autodesk.DesignScript.Geometry.Vector vOrientation,
                          List<Property> concreteProperties)
    {
      SafeInit(() => InitConcreteBentBeam(concName, ptStart, ptEnd, ptOnArc, vOrientation, concreteProperties));
    }

    private ConcreteBentBeam(ASConcreteBentBeam beam)
    {
      SafeInit(() => SetHandle(beam));
    }

    internal static ConcreteBentBeam FromExisting(ASConcreteBentBeam beam)
    {
      return new ConcreteBentBeam(beam)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitConcreteBentBeam(string concName,
                          Autodesk.DesignScript.Geometry.Point ptStart,
                          Autodesk.DesignScript.Geometry.Point ptEnd,
                          Autodesk.DesignScript.Geometry.Point ptOnArc,
                          Autodesk.DesignScript.Geometry.Vector vOrientation,
                          List<Property> concreteProperties)
    {
      List<Property> defaultData = concreteProperties.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> postWriteDBData = concreteProperties.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();

      Point3d beamStart = (ptStart == null ? new Point3d() : Utils.ToAstPoint(ptStart, true));
      Point3d beamEnd = (ptEnd == null ? new Point3d() : Utils.ToAstPoint(ptEnd, true));
      Vector3d refVect = Utils.ToAstVector3d(vOrientation, true);
      Point3d pointOnArc = Utils.ToAstPoint(ptOnArc, true);

      ASConcreteBentBeam concBentBeam = SteelServices.ElementBinder.GetObjectASFromTrace<ASConcreteBentBeam>();
      if (concBentBeam == null)
      {
        concBentBeam = new ASConcreteBentBeam(concName, refVect, beamStart, pointOnArc, beamEnd);
        if (defaultData != null)
        {
          Utils.SetParameters(concBentBeam, defaultData);
        }

        concBentBeam.WriteToDb();
      }
      else
      {
        if (!concBentBeam.IsKindOf(FilerObject.eObjectType.kConcreteBentBeam))
          throw new System.Exception("Not a Bent Concrete Beam");

        concBentBeam.SetSystemline(beamStart, pointOnArc, beamEnd);
        concBentBeam.SetSysStart(beamStart);
        concBentBeam.SetSysEnd(beamEnd);
        concBentBeam.ProfName = concName;
        Utils.SetOrientation(concBentBeam, refVect);

        if (defaultData != null)
        {
          Utils.SetParameters(concBentBeam, defaultData);
        }
      }

      SetHandle(concBentBeam);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(concBentBeam, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(concBentBeam);
    }

    /// <summary>
    /// Create an Advance Steel Concrete Bent Beam between two points and a point on an arc
    /// </summary>
    /// <param name="concName"> Concrete Profile Name</param>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    /// <param name="ptOnArc">Point on arc</param>
    /// <param name="orientation">Section orientation</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns name="beam"> beam</returns>
    public static ConcreteBentBeam ByStartPointEndPointOnArc(string concName, Autodesk.DesignScript.Geometry.Point start,
                                                          Autodesk.DesignScript.Geometry.Point end,
                                                          Autodesk.DesignScript.Geometry.Point ptOnArc,
                                                          Autodesk.DesignScript.Geometry.Vector orientation,
                                                          [DefaultArgument("null")] List<Property> additionalConcParameters)
    {
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new ConcreteBentBeam(concName, start, end, ptOnArc, orientation, additionalConcParameters);
    }

    /// <summary>
    /// Create an Advance Steel Concrete Bent Beam from a Dynamo Arc
    /// </summary>
    /// <param name="concName"> Concrete Profile Name</param>
    /// <param name="arc"> Dynamo Arc to define beam</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns name="beam"> beam</returns>
    public static ConcreteBentBeam ByArc(string concName,
                                      Autodesk.DesignScript.Geometry.Arc arc,
                                      [DefaultArgument("null")] List<Property> additionalConcParameters)
    {
      Autodesk.DesignScript.Geometry.Point start = arc.StartPoint;
      Autodesk.DesignScript.Geometry.Point end = arc.EndPoint;
      Autodesk.DesignScript.Geometry.Point ptOnArc = arc.PointAtChordLength();
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new ConcreteBentBeam(concName, start, end, ptOnArc, arc.Normal, additionalConcParameters);
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
      var beam = Utils.GetObject(Handle) as ASConcreteBentBeam;
      var midPointOnArc = beam.CenterPoint;

      using (var start = Utils.ToDynPoint(beam.GetPointAtStart(0), true))
      using (var end = Utils.ToDynPoint(beam.GetPointAtEnd(0), true))
      using (var ptOnArc = Utils.ToDynPoint(midPointOnArc, true))
      {
        var arc = Autodesk.DesignScript.Geometry.Arc.ByThreePoints(start, ptOnArc, end);
        return arc;
      }
    }

  }
}
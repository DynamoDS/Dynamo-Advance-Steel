using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Profiles;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace AdvanceSteel.Nodes.Concrete
{
  /// <summary>
  /// Advance Steel Concrete Bent Beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class ConcBentBeam : GraphicObject
  {
    internal ConcBentBeam()
    {
    }

    internal ConcBentBeam(string concName, 
                          Autodesk.DesignScript.Geometry.Point ptStart, 
                          Autodesk.DesignScript.Geometry.Point ptEnd, 
                          Autodesk.DesignScript.Geometry.Point ptOnArc, 
                          Autodesk.DesignScript.Geometry.Vector vOrientation,
                          List<ASProperty> concreteProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          List<ASProperty> defaultData = concreteProperties.Where(x => x.PropLevel == ".").ToList<ASProperty>();
          List<ASProperty> postWriteDBData = concreteProperties.Where(x => x.PropLevel == "Z_PostWriteDB").ToList<ASProperty>();

          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Point3d beamStart = (ptStart == null ? new Point3d() : Utils.ToAstPoint(ptStart, true));
          Point3d beamEnd = (ptEnd == null ? new Point3d() : Utils.ToAstPoint(ptEnd, true));
          Vector3d refVect = Utils.ToAstVector3d(vOrientation, true);
          Point3d pointOnArc = Utils.ToAstPoint(ptOnArc, true);

          Autodesk.AdvanceSteel.Modelling.ConcreteBentBeam concBentBeam = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            concBentBeam = new Autodesk.AdvanceSteel.Modelling.ConcreteBentBeam(concName, refVect, beamStart, pointOnArc, beamEnd);
            if (defaultData != null)
            {
              Utils.SetParameters(concBentBeam, defaultData);
            }

            concBentBeam.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(concBentBeam, postWriteDBData);
            }
          }
          else
          {
            concBentBeam = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.ConcreteBentBeam;

            if (concBentBeam != null && concBentBeam.IsKindOf(FilerObject.eObjectType.kConcreteBentBeam))
            {
              concBentBeam.SetSystemline(beamStart, pointOnArc, beamEnd);
              concBentBeam.SetSysStart(beamStart);
              concBentBeam.SetSysEnd(beamEnd);
              concBentBeam.ProfName = concName;
              Utils.SetOrientation(concBentBeam, refVect);

              if (defaultData != null)
              {
                Utils.SetParameters(concBentBeam, defaultData);
              }

              if (postWriteDBData != null)
              {
                Utils.SetParameters(concBentBeam, postWriteDBData);
              }
            }
            else
              throw new System.Exception("Not a Bent Concrete Beam");
          }

          Handle = concBentBeam.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(concBentBeam);
        }
      }
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
    /// <returns></returns>
    public static ConcBentBeam ByStartPointEndPointOnArc(string concName, Autodesk.DesignScript.Geometry.Point start, 
                                                          Autodesk.DesignScript.Geometry.Point end, 
                                                          Autodesk.DesignScript.Geometry.Point ptOnArc, 
                                                          Autodesk.DesignScript.Geometry.Vector orientation,
                                                          [DefaultArgument("null")]List<ASProperty> additionalConcParameters)
    {
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new ConcBentBeam(concName, start, end, ptOnArc, orientation, additionalConcParameters);
    }

    /// <summary>
    /// Create an Advance Steel Concrete Bent Beam from a Dynamo Arc
    /// </summary>
    /// <param name="concName"> Concrete Profile Name</param>
    /// <param name="arc"> Dynamo Arc to define beam</param>
    /// <param name="additionalConcParameters"> Optional Input  Build Properties </param>
    /// <returns></returns>
    public static ConcBentBeam ByArc(string concName, 
                                      Autodesk.DesignScript.Geometry.Arc arc,
                                      [DefaultArgument("null")]List<ASProperty> additionalConcParameters)
    {
      Autodesk.DesignScript.Geometry.Point start = arc.StartPoint;
      Autodesk.DesignScript.Geometry.Point end = arc.EndPoint;
      Autodesk.DesignScript.Geometry.Point ptOnArc = arc.PointAtChordLength();
      additionalConcParameters = PreSetDefaults(additionalConcParameters);
      return new ConcBentBeam(concName, start, end, ptOnArc, arc.Normal, additionalConcParameters);
    }

    private static List<ASProperty> PreSetDefaults(List<ASProperty> listOfProps)
    {
      if (listOfProps == null)
      {
        listOfProps = new List<ASProperty>() { };
      }
      return listOfProps;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var beam = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.ConcreteBentBeam;
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

  }
}
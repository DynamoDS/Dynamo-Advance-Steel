using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Features
{
  /// <summary>
  /// Advance Steel Cope on beam
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class BeamCope : GraphicObject
  {
    internal BeamCope()
    {
    }

    internal BeamCope(AdvanceSteel.Nodes.SteelDbObject element,
                      int end, int side,
                      int cnrType, double radius,
                      List<Property> beamFeatureProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          List<Property> defaultData = beamFeatureProperties.Where(x => x.Level == ".").ToList<Property>();
          List<Property> postWriteDBData = beamFeatureProperties.Where(x => x.Level == "Z_PostWriteDB").ToList<Property>();

          double length = 0;
          double depth = 0;

          length = (double)defaultData.FirstOrDefault<Property>(x => x.Name == "ReferenceLength").Value;
          depth = (double)defaultData.FirstOrDefault<Property>(x => x.Name == "ReferenceDepth").Value;

          string existingFeatureHandle = SteelServices.ElementBinder.GetHandleFromTrace();

          string elementHandle = element.Handle;
          FilerObject obj = Utils.GetObject(elementHandle);
          BeamNotch2Ortho beamFeat = null;
          if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kBeam))
          {
            if (string.IsNullOrEmpty(existingFeatureHandle) || Utils.GetObject(existingFeatureHandle) == null)
            {

              beamFeat = new BeamNotch2Ortho((Beam.eEnd)end, (Beam.eSide)side, length, depth);
              beamFeat.SetCorner((BeamNotch.eBeamNotchCornerType)cnrType, radius);

              AtomicElement atomic = obj as AtomicElement;

              if (defaultData != null)
              {
                Utils.SetParameters(beamFeat, defaultData);
              }

              atomic.AddFeature(beamFeat);

              if (postWriteDBData != null)
              {
                Utils.SetParameters(beamFeat, postWriteDBData);
              }

            }
            else
            {
              beamFeat = Utils.GetObject(existingFeatureHandle) as BeamNotch2Ortho;
              if (beamFeat != null && beamFeat.IsKindOf(FilerObject.eObjectType.kBeamNotch2Ortho))
              {
                beamFeat.End = (Beam.eEnd)end;
                beamFeat.Side = (Beam.eSide)side;
                beamFeat.SetCorner((BeamNotch.eBeamNotchCornerType)cnrType, radius);

                if (defaultData != null)
                {
                  Utils.SetParameters(beamFeat, defaultData);
                }

                if (postWriteDBData != null)
                {
                  Utils.SetParameters(beamFeat, postWriteDBData);
                }

              }
              else
                throw new System.Exception("Not a Beam Feature");
            }
          }
          else
            throw new System.Exception("No Input Element found");

          Handle = beamFeat.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(beamFeat);
        }
      }
    }

    internal BeamCope(AdvanceSteel.Nodes.SteelDbObject element,
                  int end, int side,
                  int cnrType, double radius,
                  int rotationType,
                  List<Property> beamFeatureProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          List<Property> defaultData = beamFeatureProperties.Where(x => x.Level == ".").ToList<Property>();
          List<Property> postWriteDBData = beamFeatureProperties.Where(x => x.Level == "Z_PostWriteDB").ToList<Property>();

          double length = 0;
          double depth = 0;

          length = (double)defaultData.FirstOrDefault<Property>(x => x.Name == "ReferenceLength").Value;
          depth = (double)defaultData.FirstOrDefault<Property>(x => x.Name == "ReferenceDepth").Value;

          string existingFeatureHandle = SteelServices.ElementBinder.GetHandleFromTrace();

          string elementHandle = element.Handle;
          FilerObject obj = Utils.GetObject(elementHandle);
          BeamNotchEx beamFeat = null;
          if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kBeam))
          {
            if (string.IsNullOrEmpty(existingFeatureHandle) || Utils.GetObject(existingFeatureHandle) == null)
            {

              beamFeat = new BeamNotchEx((Beam.eEnd)end, (Beam.eSide)side, length, depth);
              beamFeat.XRotation = (BeamNotchEx.eXRotation)rotationType;
              beamFeat.SetCorner((BeamNotch.eBeamNotchCornerType)cnrType, radius);

              AtomicElement atomic = obj as AtomicElement;

              if (defaultData != null)
              {
                Utils.SetParameters(beamFeat, defaultData);
              }

              atomic.AddFeature(beamFeat);

              if (postWriteDBData != null)
              {
                Utils.SetParameters(beamFeat, postWriteDBData);
              }

            }
            else
            {
              beamFeat = Utils.GetObject(existingFeatureHandle) as BeamNotchEx;
              if (beamFeat != null && beamFeat.IsKindOf(FilerObject.eObjectType.kBeamNotchEx))
              {
                beamFeat.End = (Beam.eEnd)end;
                beamFeat.Side = (Beam.eSide)side;
                beamFeat.XRotation = (BeamNotchEx.eXRotation)rotationType;
                beamFeat.SetCorner((BeamNotch.eBeamNotchCornerType)cnrType, radius);

                if (defaultData != null)
                {
                  Utils.SetParameters(beamFeat, defaultData);
                }

                if (postWriteDBData != null)
                {
                  Utils.SetParameters(beamFeat, postWriteDBData);
                }

              }
              else
                throw new System.Exception("Not a Beam Feature");
            }
          }
          else
            throw new System.Exception("No Input Element found");

          Handle = beamFeat.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(beamFeat);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel Square Cope feature
    /// </summary>
    /// <param name="element"> Input Plate</param>
    /// <param name="end"> Input Beam End for Cope 1 - Start, 2 - End</param>
    /// <param name="side"> Input Beam Side for Cope 1 - Upper, 2 - Lower</param>
    /// <param name="length"> Input Length of Cope</param>
    /// <param name="depth"> Input depth of Cope</param>
    /// <param name="cornerRadius"> Input Cope radius at Corner</param>
    /// <param name="cornerType"> 0 - Straight, 1 - Round, 2 - Boring Hole</param>
    /// <param name="xRotationType"> 0 - Around Notch, 1 - Around Beam</param>
    /// <param name="additionalBeamFeatureParameters"> Optional Input Beam Feature Build Properties </param>
    /// <returns></returns>
    public static BeamCope ByRectangularRotatedCope(AdvanceSteel.Nodes.SteelDbObject element,
                                    int end, int side,
                                    double length, double depth,
                                    [DefaultArgument("0")] double cornerRadius,
                                    [DefaultArgument("0")] int cornerType,
                                    [DefaultArgument("0")] int xRotationType,
                                    [DefaultArgument("null")] List<Property> additionalBeamFeatureParameters)
    {
      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters, Utils.ToInternalUnits(length, true), Utils.ToInternalUnits(depth, true));
      return new BeamCope(element, end, side, cornerType, Utils.ToInternalUnits(cornerRadius, true), xRotationType, additionalBeamFeatureParameters);
    }

    /// <summary>
    /// Create an Advance Steel rotated Cope feature
    /// </summary>
    /// <param name="element"> Input Plate</param>
    /// <param name="end"> Input Beam End for Cope 1 - Start, 2 - End</param>
    /// <param name="side"> Input Beam Side for Cope 1 - Upper, 2 - Lower</param>
    /// <param name="length"> Input Length of Cope</param>
    /// <param name="depth"> Input depth of Cope</param>
    /// <param name="cornerRadius"> Input Cope radius at Corner</param>
    /// <param name="cornerType"> 0 - Straight, 1 - Round, 2 - Boring Hole</param>
    /// <param name="additionalBeamFeatureParameters"> Optional Input Beam Feature Build Properties </param>
    /// <returns></returns>
    public static BeamCope ByRectangularOrthoCope(AdvanceSteel.Nodes.SteelDbObject element,
                                    int end, int side,
                                    double length, double depth,
                                    [DefaultArgument("0")] double cornerRadius,
                                    [DefaultArgument("0")] int cornerType,
                                    [DefaultArgument("null")] List<Property> additionalBeamFeatureParameters)
    {
      additionalBeamFeatureParameters = PreSetDefaults(additionalBeamFeatureParameters, Utils.ToInternalUnits(length, true), Utils.ToInternalUnits(depth, true));
      return new BeamCope(element, end, side, cornerType, Utils.ToInternalUnits(cornerRadius, true), additionalBeamFeatureParameters);
    }


    private static List<Property> PreSetDefaults(List<Property> listBeamFeatureData, double length = 0, double depth = 0)
    {
      if (listBeamFeatureData == null)
      {
        listBeamFeatureData = new List<Property>() { };
      }
      if (length > 0) Utils.CheckListUpdateOrAddValue(listBeamFeatureData, "ReferenceLength", length, ".");
      if (depth > 0) Utils.CheckListUpdateOrAddValue(listBeamFeatureData, "ReferenceDepth", depth, ".");
      return listBeamFeatureData;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var beamFeat = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.BeamNotch;

          Autodesk.AdvanceSteel.Geometry.Matrix3d matrix = beamFeat.CS;
          var poly = Autodesk.DesignScript.Geometry.Rectangle.ByWidthLength(Utils.ToDynCoordinateSys(matrix, true),
                                                                  Utils.FromInternalUnits(beamFeat.ReferenceLength, true),
                                                                  Utils.FromInternalUnits(beamFeat.ReferenceDepth, true));

          return poly;
        }
      }
    }
  }
}
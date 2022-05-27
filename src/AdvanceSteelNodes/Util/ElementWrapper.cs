using AdvanceSteel.Nodes.Beams;
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvanceSteel.Nodes.Concrete;
using AdvanceSteel.Nodes.Gratings;
using AdvanceSteel.Nodes.Plates;
using AdvanceSteel.Nodes.Miscellaneous;
using AdvanceSteel.Nodes.ConnectionObjects.Anchors;
using AdvanceSteel.Nodes.ConnectionObjects.Bolts;
using AdvanceSteel.Nodes.ConnectionObjects.ShearStuds;
using AdvanceSteel.Nodes.ConnectionObjects.Welds;
using AdvanceSteel.Nodes.Features;

using ASStraightBeam = Autodesk.AdvanceSteel.Modelling.StraightBeam;
using ASCompoundStraightBeam = Autodesk.AdvanceSteel.Modelling.CompoundStraightBeam;
using ASPolyBeam = Autodesk.AdvanceSteel.Modelling.PolyBeam;
using ASUnfoldedStraightBeam = Autodesk.AdvanceSteel.Modelling.UnfoldedStraightBeam;
using ASBeamTapered = Autodesk.AdvanceSteel.Modelling.BeamTapered;
using ASBentBeam = Autodesk.AdvanceSteel.Modelling.BentBeam;
using ASConcreteBentBeam = Autodesk.AdvanceSteel.Modelling.ConcreteBentBeam;
using ASConcreteBeam = Autodesk.AdvanceSteel.Modelling.ConcreteBeam;
using ASFootingIsolated = Autodesk.AdvanceSteel.Modelling.FootingIsolated;
using ASWall = Autodesk.AdvanceSteel.Modelling.Wall;
using ASSlab = Autodesk.AdvanceSteel.Modelling.Slab;
using ASPlate = Autodesk.AdvanceSteel.Modelling.Plate;
using ASGrating = Autodesk.AdvanceSteel.Modelling.Grating;
using ASCamera = Autodesk.AdvanceSteel.ConstructionHelper.Camera;
using ASGrid = Autodesk.AdvanceSteel.Modelling.Grid;
using ASSpecialPart = Autodesk.AdvanceSteel.Modelling.SpecialPart;
using ASAnchorPattern = Autodesk.AdvanceSteel.Modelling.AnchorPattern;
using ASCircleScrewBoltPattern = Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern;
using ASConnector = Autodesk.AdvanceSteel.Modelling.Connector;
using ASFinitRectScrewBoltPattern = Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern;
using ASInfinitMidScrewBoltPattern = Autodesk.AdvanceSteel.Modelling.InfinitMidScrewBoltPattern;
using ASWeldLine = Autodesk.AdvanceSteel.Modelling.WeldLine;
using ASWeldPoint = Autodesk.AdvanceSteel.Modelling.WeldPoint;
using ASBeamNotch2Ortho = Autodesk.AdvanceSteel.Modelling.BeamNotch2Ortho;
using ASBeamNotchEx = Autodesk.AdvanceSteel.Modelling.BeamNotchEx;
using ASBeamShortening = Autodesk.AdvanceSteel.Modelling.BeamShortening;
using ASBeamMultiContourNotch = Autodesk.AdvanceSteel.Modelling.BeamMultiContourNotch;
using ASPlateFeatContour = Autodesk.AdvanceSteel.Modelling.PlateFeatContour;
using ASPlateContourNotch = Autodesk.AdvanceSteel.Modelling.PlateContourNotch;
using ASPlateFeatVertFillet = Autodesk.AdvanceSteel.Modelling.PlateFeatVertFillet;
using ASConnectionHolePlate = Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;
using ASConnectionHoleBeam = Autodesk.AdvanceSteel.Modelling.ConnectionHoleBeam;
using Autodesk.AdvanceSteel.ConstructionTypes;

namespace AdvanceSteel.Nodes
{
  [SupressImportIntoVM]
  public static class ElementWrapper
  {
    /// <summary>
    /// Wrapping the AS object (FilerObject)
    /// </summary>
    /// <param name="filerObject"></param>
    /// <returns></returns>
    public static SteelDbObject ToDSType(this FilerObject filerObject)
    {
      // cast to dynamic to dispatch to the appropriate wrapping method
      dynamic dynamicElement = filerObject;
      return ElementWrapper.Wrap(dynamicElement);
    }

    /// <summary>
    /// Specific dispatch to AS StraightBeam
    /// </summary>
    /// <param name="beam"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASStraightBeam beam)
    {
      return StraightBeam.FromExisting(beam);
    }

    /// <summary>
    /// Specific dispatch to AS CompoundStraightBeam
    /// </summary>
    /// <param name="beam"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASCompoundStraightBeam beam)
    {
      return CompoundBeam.FromExisting(beam);
    }

    /// <summary>
    /// Specific dispatch to AS PolyBeam
    /// </summary>
    /// <param name="beam"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASPolyBeam beam)
    {
      return PolyBeam.FromExisting(beam);
    }

    /// <summary>
    /// Specific dispatch to AS UnfoldedStraightBeam
    /// </summary>
    /// <param name="beam"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASUnfoldedStraightBeam beam)
    {
      return UnFoldedBeam.FromExisting(beam);
    }

    /// <summary>
    /// Specific dispatch to AS BeamTapered
    /// </summary>
    /// <param name="beam"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASBeamTapered beam)
    {
      return TaperedBeam.FromExisting(beam);
    }

    /// <summary>
    /// Specific dispatch to AS BentBeam
    /// </summary>
    /// <param name="beam"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASBentBeam beam)
    {
      return BentBeam.FromExisting(beam);
    }

    /// <summary>
    /// Specific dispatch to AS ConcreteBentBeam
    /// </summary>
    /// <param name="concreteBeam"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASConcreteBentBeam concreteBeam)
    {
      return ConcreteBentBeam.FromExisting(concreteBeam);
    }

    /// <summary>
    /// Specific dispatch to AS ConcreteBeam
    /// </summary>
    /// <param name="concreteBeam"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASConcreteBeam concreteBeam)
    {
      return ConcreteStraightBeam.FromExisting(concreteBeam);
    }

    /// <summary>
    /// Specific dispatch to AS FootingIsolated
    /// </summary>
    /// <param name="padFooting"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASFootingIsolated padFooting)
    {
      return Footings.FromExisting(padFooting);
    }

    /// <summary>
    /// Specific dispatch to AS Wall
    /// </summary>
    /// <param name="wall"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASWall wall)
    {
      return Walls.FromExisting(wall);
    }

    /// <summary>
    /// Specific dispatch to AS Slab
    /// </summary>
    /// <param name="slab"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASSlab slab)
    {
      return Slabs.FromExisting(slab);
    }

    /// <summary>
    /// Specific dispatch to AS Plate
    /// </summary>
    /// <param name="plate"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASPlate plate)
    {
      return Plate.FromExisting(plate);
    }

    /// <summary>
    /// Specific dispatch to AS Grating
    /// </summary>
    /// <param name="grating"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASGrating grating)
    {
      switch (grating.GratingType)
      {
        case Autodesk.AdvanceSteel.Modelling.Grating.eGratingType.kStandard:
          return StandardGrating.FromExisting(grating);
        case Autodesk.AdvanceSteel.Modelling.Grating.eGratingType.kVariable:
          return VariableGrating.FromExisting(grating);
        case Autodesk.AdvanceSteel.Modelling.Grating.eGratingType.kBar:
          return BarGrating.FromExisting(grating);
        default:
          throw new NotImplementedException("Grating not implemented");
      }
    }

    /// <summary>
    /// Specific dispatch to AS Camera
    /// </summary>
    /// <param name="camera"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASCamera camera)
    {
      return Camera.FromExisting(camera);
    }

    /// <summary>
    /// Specific dispatch to AS Grid
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASGrid grid)
    {
      return Grid.FromExisting(grid);
    }

    /// <summary>
    /// Specific dispatch to AS SpecialPart
    /// </summary>
    /// <param name="specialPart"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASSpecialPart specialPart)
    {
      return SpecialPart.FromExisting(specialPart);
    }

    /// <summary>
    /// Specific dispatch to AS AnchorPattern
    /// </summary>
    /// <param name="anchor"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASAnchorPattern anchor)
    {
      switch (anchor.ArrangerType)
      {
        case Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kCircle:
          return CircularAnchorPattern.FromExisting(anchor);
        case Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kRectangular:
          return RectangularAnchorPattern.FromExisting(anchor);
        default:
          throw new NotImplementedException("Anchor not implemented");
      }
    }

    /// <summary>
    /// Specific dispatch to AS CircleScrewBoltPattern
    /// </summary>
    /// <param name="bolt"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASCircleScrewBoltPattern bolt)
    {
      return CircularBoltPattern.FromExisting(bolt);
    }

    /// <summary>
    /// Specific dispatch to AS AnchorPattern
    /// </summary>
    /// <param name="shearStuds"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASConnector shearStuds)
    {
      switch (shearStuds.Arranger.Type)
      {
        case Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kCircle:
          return CircularShearStudsPattern.FromExisting(shearStuds);
        case Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kRectangular:
          return RectangularShearStudsPattern.FromExisting(shearStuds);
        default:
          throw new NotImplementedException("Shear Studs not implemented");
      }
    }

    /// <summary>
    /// Specific dispatch to AS FinitRectScrewBoltPattern
    /// </summary>
    /// <param name="bolt"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASFinitRectScrewBoltPattern bolt)
    {
      return RectangularBoltPattern.FromExisting(bolt);
    }

    /// <summary>
    /// Specific dispatch to AS InfinitMidScrewBoltPattern
    /// </summary>
    /// <param name="bolt"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASInfinitMidScrewBoltPattern bolt)
    {
      return RectangularBoltPattern.FromExisting(bolt);
    }

    /// <summary>
    /// Specific dispatch to AS WeldLine
    /// </summary>
    /// <param name="wed"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASWeldLine wed)
    {
      return WeldLine.FromExisting(wed);
    }

    /// <summary>
    /// Specific dispatch to AS WeldPoint
    /// </summary>
    /// <param name="wed"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASWeldPoint wed)
    {
      return WeldPoint.FromExisting(wed);
    }

    /// <summary>
    /// Specific dispatch to AS BeamNotch2Ortho
    /// </summary>
    /// <param name="beamFeat"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASBeamNotch2Ortho beamFeat)
    {
      return BeamCope.FromExisting(beamFeat);
    }

    /// <summary>
    /// Specific dispatch to AS BeamNotchEx
    /// </summary>
    /// <param name="beamFeat"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASBeamNotchEx beamFeat)
    {
      return BeamCope.FromExisting(beamFeat);
    }

    /// <summary>
    /// Specific dispatch to AS BeamShortening
    /// </summary>
    /// <param name="beamFeat"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASBeamShortening beamFeat)
    {
      return BeamPlaneCut.FromExisting(beamFeat);
    }

    /// <summary>
    /// Specific dispatch to AS BeamMultiContourNotch
    /// </summary>
    /// <param name="beamFeat"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASBeamMultiContourNotch beamFeat)
    {
      return BeamPolycut.FromExisting(beamFeat);
    }

    /// <summary>
    /// Specific dispatch to AS PlateFeatContour
    /// </summary>
    /// <param name="plateFeat"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASPlateFeatContour plateFeat)
    {
      return PlatePolycut.FromExisting(plateFeat);
    }

    /// <summary>
    /// Specific dispatch to AS PlateContourNotch
    /// </summary>
    /// <param name="plateFeat"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASPlateContourNotch plateFeat)
    {
      return PlatePolycut.FromExisting(plateFeat);
    }

    /// <summary>
    /// Specific dispatch to AS PlateFeatVertFillet
    /// </summary>
    /// <param name="plateFeat"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASPlateFeatVertFillet plateFeat)
    {
      return PlateVertexCut.FromExisting(plateFeat);
    }

    /// <summary>
    /// Specific dispatch to AS ConnectionHolePlate
    /// </summary>
    /// <param name="hole"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASConnectionHolePlate hole)
    {
      return PlateHoles.FromExisting(hole);
    }

    /// <summary>
    /// Specific dispatch to AS ConnectionHoleBeam
    /// </summary>
    /// <param name="hole"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(ASConnectionHoleBeam hole)
    {
      return BeamHoles.FromExisting(hole);
    }

    /// <summary>
    /// Specific dispatch to AS UserAutoConstructionObject
    /// </summary>
    /// <param name="userAutoConstructionObject"></param>
    /// <returns></returns>
    private static SteelDbObject Wrap(UserAutoConstructionObject userAutoConstructionObject)
    {
      return Joint.FromExisting(userAutoConstructionObject);
    }

  }
}
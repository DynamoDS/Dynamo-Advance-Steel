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
using AdvanceSteel.Nodes.ConnectionObjects.Anchors;

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
        default: //case Autodesk.AdvanceSteel.Modelling.Grating.eGratingType.kBar:
          return BarGrating.FromExisting(grating);
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

  }
}
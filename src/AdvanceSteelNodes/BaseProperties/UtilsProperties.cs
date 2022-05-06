using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Connection;
using Autodesk.AdvanceSteel.ConstructionHelper;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;

namespace AdvanceSteel.Nodes
{
  public static class UtilsProperties
  {

    public static readonly Dictionary<eObjectType, SteelTypeData> SteelObjectPropertySets = new Dictionary<eObjectType, SteelTypeData>()
    {
      { eObjectType.kAnchorPattern, new SteelTypeData(typeof(AnchorPattern), "Anchor Pattern") },
      { eObjectType.kBeam, new SteelTypeData(typeof(Beam), "Any Beam") },
      { eObjectType.kBeamNotch, new SteelTypeData(typeof(BeamNotch), "Beam Cope") },
      { eObjectType.kBeamNotchEx, new SteelTypeData(typeof(BeamNotchEx), "Beam Cope Rotated") },
      { eObjectType.kBentBeamBase, new SteelTypeData(typeof(BentBeamBase), "Bent Beam") },
      { eObjectType.kBeamMultiContourNotch, new SteelTypeData(typeof(BeamMultiContourNotch), "Beam Polycut") },
      { eObjectType.kBeamShortening, new SteelTypeData(typeof(BeamShortening), "Beam Shortening") },
      { eObjectType.kBeamTapered, new SteelTypeData(typeof(BeamTapered), "Tapered Beam") },
      { eObjectType.kBoltPattern, new SteelTypeData(typeof(BoltPattern), "Any Bolt") },
      { eObjectType.kCamera, new SteelTypeData(typeof(Camera), "Camera") },
      { eObjectType.kCircleScrewBoltPattern, new SteelTypeData(typeof(CircleScrewBoltPattern), "Circular Bolt") },
      { eObjectType.kCompoundStraightBeam, new SteelTypeData(typeof(CompoundStraightBeam), "Compound Beam") },
      { eObjectType.kConcreteBentBeam, new SteelTypeData(typeof(ConcreteBentBeam), "Concrete Bent Beam") },
      { eObjectType.kConcreteBeam, new SteelTypeData(typeof(ConcreteBeam), "Concrete Straight Beam") },
      { eObjectType.kGrating, new SteelTypeData(typeof(Grating), "Grating")},
      { eObjectType.kFootingIsolated, new SteelTypeData(typeof(FootingIsolated), "Isolated Footings") },
      { eObjectType.kPlate, new SteelTypeData(typeof(Plate), "Plate") },
      { eObjectType.kFoldedPlate, new SteelTypeData(typeof(FoldedPlate), "Folded Plate") },
      { eObjectType.kPlateBase, new SteelTypeData(typeof(PlateBase), "Any Plate") },
      { eObjectType.kPolyBeam, new SteelTypeData(typeof(PolyBeam), "Poly Beam") },
      { eObjectType.kPlateFeatContour, new SteelTypeData(typeof(PlateFeatContour), "Plate Polycut") },
      { eObjectType.kPlateFeatVertFillet, new SteelTypeData(typeof(PlateFeatVertFillet), "Plate Corner Cut") },
      { eObjectType.kPlateFeatEdge, new SteelTypeData(typeof(PlateFeatEdge), "Plate Edge Cut") },
      { eObjectType.kScrewBoltPattern, new SteelTypeData(typeof(ScrewBoltPattern), "Screw Bolt") },
      { eObjectType.kCountableScrewBoltPattern, new SteelTypeData(typeof(CountableScrewBoltPattern), "Countable Bolt") },
      { eObjectType.kFinitRectScrewBoltPattern, new SteelTypeData(typeof(FinitRectScrewBoltPattern), "Rectangular Bolt") },
      { eObjectType.kInfinitMidScrewBoltPattern, new SteelTypeData(typeof(InfinitMidScrewBoltPattern), "Center Bolt") },
      { eObjectType.kInfinitRectScrewBoltPattern, new SteelTypeData(typeof(InfinitRectScrewBoltPattern), "Corner Bolt") },
      { eObjectType.kStraightBeam, new SteelTypeData(typeof(StraightBeam), "Straight Beam") },
      { eObjectType.kSpecialPart, new SteelTypeData(typeof(SpecialPart), "Special Part") },
      { eObjectType.kSlab, new SteelTypeData(typeof(Slab), "Slabs") },
      { eObjectType.kConnector, new SteelTypeData(typeof(Connector), "Shear Studs") },
      { eObjectType.kUnfoldedStraightBeam, new SteelTypeData(typeof(UnfoldedStraightBeam), "Unfolded Straight Beam") },
      { eObjectType.kWall, new SteelTypeData(typeof(Wall), "Walls") },
      { eObjectType.kWeldPattern, new SteelTypeData(typeof(WeldPattern), "Any Weld") },
      { eObjectType.kWeldStraight, new SteelTypeData(typeof(WeldLine), "Weld Line") },
      { eObjectType.kWeldLevel, new SteelTypeData (typeof(WeldPoint), "Weld Point") },
      { eObjectType.kUserAutoConstructionObject, new SteelTypeData (typeof(UserAutoConstructionObject), "Joint") },
      { eObjectType.kFilerObject, new SteelTypeData (typeof(FilerObject), "Filer Object") },
      { eObjectType.kConstructionElem, new SteelTypeData (typeof(ConstructionElement), "Construction Element") },
      { eObjectType.kActConstructionElem, new SteelTypeData (typeof(ActiveConstructionElement), "Active Construction Element") },
      { eObjectType.kAtomicElem, new SteelTypeData (typeof(AtomicElement), "Atomic Element") },
      { eObjectType.kConnectionHoleFeature, new SteelTypeData (typeof(ConnectionHoleFeature), "Any Hole") },
      { eObjectType.kConnectionHoleBeam, new SteelTypeData (typeof(ConnectionHoleBeam), "Beam Hole") },
    };

  }
}
using Autodesk.AdvanceSteel.Arrangement;
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

    internal static readonly Dictionary<Type, SteelTypeData> SteelObjectPropertySets = new Dictionary<Type, SteelTypeData>()
    {
      { typeof(AnchorPattern), new SteelTypeData("Anchor Pattern") },
      { typeof(Beam), new SteelTypeData("Any Beam") },
      { typeof(BeamNotch), new SteelTypeData("Beam Cope") },
      { typeof(BeamNotchEx), new SteelTypeData("Beam Cope Rotated") },
      { typeof(BentBeam), new SteelTypeData("Bent Beam") },
      { typeof(BeamMultiContourNotch), new SteelTypeData("Beam Polycut") },
      { typeof(BeamShortening), new SteelTypeData("Beam Shortening") },
      { typeof(BeamTapered), new SteelTypeData("Tapered Beam") },
      { typeof(BoltPattern), new SteelTypeData("Any Bolt") },
      { typeof(Camera), new SteelTypeData("Camera") },
      { typeof(CircleScrewBoltPattern), new SteelTypeData("Circular Bolt") },
      { typeof(CompoundStraightBeam), new SteelTypeData("Compound Beam") },
      { typeof(ConcreteBentBeam), new SteelTypeData("Concrete Bent Beam") },
      { typeof(ConcreteBeam), new SteelTypeData("Concrete Straight Beam") },
      { typeof(Grating), new SteelTypeData("Grating")},
      { typeof(FootingIsolated), new SteelTypeData("Isolated Footings") },
      { typeof(Plate), new SteelTypeData("Plate") },
      { typeof(FoldedPlate), new SteelTypeData("Folded Plate") },
      { typeof(PlateBase), new SteelTypeData("Any Plate") },
      { typeof(PolyBeam), new SteelTypeData("Poly Beam") },
      { typeof(PlateFeatContour), new SteelTypeData("Plate Polycut") },
      { typeof(PlateFeatVertFillet), new SteelTypeData("Plate Corner Cut") },
      { typeof(PlateFeatEdge), new SteelTypeData("Plate Edge Cut") },
      { typeof(ScrewBoltPattern), new SteelTypeData("Screw Bolt") },
      { typeof(CountableScrewBoltPattern), new SteelTypeData("Countable Bolt") },
      { typeof(FinitRectScrewBoltPattern), new SteelTypeData("Rectangular Bolt") },
      { typeof(InfinitMidScrewBoltPattern), new SteelTypeData("Center Bolt") },
      { typeof(InfinitRectScrewBoltPattern), new SteelTypeData("Corner Bolt") },
      { typeof(StraightBeam), new SteelTypeData("Straight Beam") },
      { typeof(SpecialPart), new SteelTypeData("Special Part") },
      { typeof(Slab), new SteelTypeData("Slabs") },
      { typeof(Connector), new SteelTypeData("Shear Studs") },
      { typeof(UnfoldedStraightBeam), new SteelTypeData("Unfolded Straight Beam") },
      { typeof(Wall), new SteelTypeData("Walls") },
      { typeof(WeldPattern), new SteelTypeData("Any Weld") },
      { typeof(WeldLine), new SteelTypeData("Weld Line") },
      { typeof(WeldPoint), new SteelTypeData ("Weld Point") },
      { typeof(UserAutoConstructionObject), new SteelTypeData ("Joint") },
      { typeof(FilerObject), new SteelTypeData ("Filer Object") },
      { typeof(ConstructionElement), new SteelTypeData ("Construction Element") },
      { typeof(ActiveConstructionElement), new SteelTypeData ("Active Construction Element") },
      { typeof(AtomicElement), new SteelTypeData ("Atomic Element") },
      { typeof(ConnectionHoleFeature), new SteelTypeData ("Any Hole") },
      { typeof(ConnectionHoleBeam), new SteelTypeData ("Beam Hole") },
      { typeof(Arranger), new SteelTypeData ("Arranger") }
    };

    private static bool dictionaryLoaded = false;

    /// <summary>
    /// Load all properties of each ASType
    /// </summary>
    public static void LoadASTypeDictionary()
    {
      if (dictionaryLoaded)
      {
        return;
      }

      var assemblyTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();

      var listIASProperties = assemblyTypes.Where(x => !x.IsAbstract && x.GetInterfaces().Contains(typeof(IASProperties))).Select(x => Activator.CreateInstance(x) as IASProperties);

      //Fill specific properties
      foreach (var item in SteelObjectPropertySets)
      {
        IASProperties asProperties = listIASProperties.FirstOrDefault(x => x.GetObjectType.Equals(item.Key));

        if (asProperties == null)
        {
          throw new NotImplementedException(item.Key.ToString() + " not implemented");
        }

        item.Value.ASType = item.Key;
        item.Value.SetPropertiesSpecific(asProperties.BuildPropertyList());
      }

      //Fill all properties
      foreach (var item in SteelObjectPropertySets)
      {
        IEnumerable<SteelTypeData> steelTypeDataList = SteelObjectPropertySets.Where(x => item.Key.IsSubclassOf(x.Key) || item.Key.IsEquivalentTo(x.Key)).Select(x => x.Value);
        foreach (var steelTypeData in steelTypeDataList)
        {
          item.Value.AddPropertiesAll(steelTypeData.PropertiesSpecific);
        }

        item.Value.OrderDictionaryPropertiesAll();
      }

      dictionaryLoaded = true;
    }

  }
}
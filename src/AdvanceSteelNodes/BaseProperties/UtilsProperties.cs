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
    #region Load dictionary

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

    #endregion

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
      { typeof(PlateContourNotch), new SteelTypeData("Plate Notch") },
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

    internal static Property GetProperty(Type objectType, string keyValue)
    {
      var dictionaryProperties = GetAllPropertiesWithoutClone(objectType);

      if (dictionaryProperties.TryGetValue(keyValue, out Property retValue))
      {
        return new Property(retValue);
      }

      throw new System.Exception(string.Format("Property '{0}' not found", keyValue));
    }

    internal static Property GetPropertyByMemberName(Type objectType, string memberName)
    {
      var dictionaryProperties = GetAllPropertiesWithoutClone(objectType);

      var itemKeyValue = dictionaryProperties.FirstOrDefault(x => x.Value.MemberName.Equals(memberName));

      if (itemKeyValue.Value == null)
      {
        throw new System.Exception(string.Format("Property of member '{0}' not found", memberName));
      }

      return new Property(itemKeyValue.Value);
    }

    internal static Dictionary<string, Property> GetAllPropertiesWithoutClone(Type objectType)
    {
      if (!CheckType(objectType))
      {
        throw new Exception(string.Format("Properties not found for type '{0}'", objectType));
      }

      return UtilsProperties.SteelObjectPropertySets[objectType].PropertiesAll;
    }

    public static string GetDescriptionObject(Type objectType)
    {
      return UtilsProperties.SteelObjectPropertySets[objectType].Description;
    }

    internal static bool CheckType(Type objectType)
    {
      return UtilsProperties.SteelObjectPropertySets.ContainsKey(objectType);
    }

    public static Dictionary<string, Property> GetAllProperties(Type objectType)
    {
      var dictionaryProperties = GetAllPropertiesWithoutClone(objectType);

      Dictionary<string, Property> ret = new Dictionary<string, Property>() { };
      foreach (KeyValuePair<string, Property> item in dictionaryProperties)
      {
        ret.Add(item.Key, new Property(item.Value));
      }

      return ret;
    }

    public static void CheckListUpdateOrAddValue(Type objectType, List<Property> listOfPropertyData, string propName, object propValue)
    {
      InitializeProperties(objectType, listOfPropertyData);

      var property = listOfPropertyData.FirstOrDefault<Property>(props => props.MemberName == propName);
      if (property == null)
      {
        property = GetPropertyByMemberName(objectType, propName);
        listOfPropertyData.Add(property);
      }

      property.InternalValue = propValue;
    }

    private static void InitializeProperties(Type typeObject, List<Property> listOfPropertyData)
    {
      foreach (var property in listOfPropertyData)
      {
        property.InitializePropertyIfNeeded(typeObject);
      }
    }

    #region Set Parameters Methods

    public static void SetParameters(Autodesk.AdvanceSteel.Arrangement.Arranger objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    public static void SetParameters(FilerObject objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.SetToObject(objToMod);
        }
      }
    }

    #endregion

  }
}
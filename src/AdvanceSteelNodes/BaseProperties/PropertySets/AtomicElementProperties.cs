using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;

namespace AdvanceSteel.Nodes
{
  public class AtomicElementProperties : BaseProperties, IASProperties
  {
    public override eObjectType GetObjectType => eObjectType.kAtomicElem;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Volume", nameof(AtomicElement.Volume), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Used For Numbering - Assembly", nameof(AtomicElement.AssemblyUsedForNumbering));
      InsertItem(dictionary, objectASType, "Used For Numbering - Note", nameof(AtomicElement.NoteUsedForNumbering));
      InsertItem(dictionary, objectASType, "Used For Numbering - Role", nameof(AtomicElement.RoleUsedForNumbering));
      InsertItem(dictionary, objectASType, "Used For BOM - Single Part", nameof(AtomicElement.SinglePartUsedForBOM));
      InsertItem(dictionary, objectASType, "Used For BOM - Main Part", nameof(AtomicElement.MainPartUsedForBOM));
      InsertItem(dictionary, objectASType, "Used For Collision Check - Single Part", nameof(AtomicElement.SinglePartUsedForCollisionCheck));
      InsertItem(dictionary, objectASType, "Used For Collision Check - Main Part", nameof(AtomicElement.MainPartUsedForCollisionCheck));
      InsertItem(dictionary, objectASType, "Structural Member", nameof(AtomicElement.StructuralMember));
      InsertItem(dictionary, objectASType, "Used For Numbering - Holes", nameof(AtomicElement.HolesUsedForNumbering));
      InsertItem(dictionary, objectASType, "MainPart Number", nameof(AtomicElement.MainPartNumber));
      InsertItem(dictionary, objectASType, "SinglePart Number", nameof(AtomicElement.SinglePartNumber));
      InsertItem(dictionary, objectASType, "Preliminary Part Prefix", nameof(AtomicElement.PreliminaryPartPrefix));
      InsertItem(dictionary, objectASType, "Preliminary Part Number", nameof(AtomicElement.PreliminaryPartNumber));
      InsertItem(dictionary, objectASType, "Preliminary Part Position Number", nameof(AtomicElement.PreliminaryPartPositionNumber));
      InsertItem(dictionary, objectASType, "Used For Numbering - Item Number", nameof(AtomicElement.ItemNumberUsedForNumbering));
      InsertItem(dictionary, objectASType, "Used For Numbering - Dennotation", nameof(AtomicElement.DennotationUsedForNumbering));
      InsertItem(dictionary, objectASType, "Used For Numbering - Coating", nameof(AtomicElement.CoatingUsedForNumbering));
      InsertItem(dictionary, objectASType, "Used For Numbering - Material", nameof(AtomicElement.MaterialUsedForNumbering));
      InsertItem(dictionary, objectASType, "Unwind Start Factor", nameof(AtomicElement.UnwindStartFactor));
      InsertItem(dictionary, objectASType, "Denotation", nameof(AtomicElement.Denotation));
      InsertItem(dictionary, objectASType, "Assembly", nameof(AtomicElement.Assembly));
      InsertItem(dictionary, objectASType, "Note", nameof(AtomicElement.Note));
      InsertItem(dictionary, objectASType, "Item Number", nameof(AtomicElement.ItemNumber));
      InsertItem(dictionary, objectASType, "Specific Gravity", nameof(AtomicElement.SpecificGravity));
      InsertItem(dictionary, objectASType, "Coating", nameof(AtomicElement.Coating));
      InsertItem(dictionary, objectASType, "Number Of Holes", nameof(AtomicElement.NumberOfHoles), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Is Attached Part", nameof(AtomicElement.IsAttachedPart), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Is Main Part", nameof(AtomicElement.IsMainPart));
      InsertItem(dictionary, objectASType, "Main Part Prefix", nameof(AtomicElement.MainPartPrefix));
      InsertItem(dictionary, objectASType, "Single Part Prefix", nameof(AtomicElement.SinglePartPrefix));
      InsertItem(dictionary, objectASType, "Used For Numbering - SinglePart", nameof(AtomicElement.SinglePartUsedForNumbering));
      InsertItem(dictionary, objectASType, "Used For Numbering - MainPart", nameof(AtomicElement.MainPartUsedForNumbering));
      InsertItem(dictionary, objectASType, "Explicit Quantity", nameof(AtomicElement.ExplicitQuantity));
      InsertItem(dictionary, objectASType, "Material Description", nameof(AtomicElement.MaterialDescription), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Coating Description", nameof(AtomicElement.CoatingDescription), LevelEnum.Default);
      InsertItem(dictionary, objectASType, "Material", nameof(AtomicElement.Material));
      InsertItem(dictionary, objectASType, "Unwind", nameof(AtomicElement.Unwind));

      //Functions

      InsertItem(dictionary, "Balance Point", GetBalancePoint);
      InsertItem(dictionary, objectASType, "Main Part Position", nameof(AtomicElement.GetMainPartPositionNumber));
      InsertItem(dictionary, objectASType, "Model Quantity", nameof(AtomicElement.GetQuantityInModel));
      InsertItem(dictionary, objectASType, "Single Part Position", nameof(AtomicElement.GetSinglePartPositionNumber));
      InsertItem(dictionary, objectASType, "Features Number", nameof(AtomicElement.NumFeatures));
      InsertItem(dictionary, "Holes (Properties)", GetHoles);
      InsertItem(dictionary, "Numbering - Valid Single Part", HasValidSPNumber);
      InsertItem(dictionary, "Numbering - Valid Main Part", HasValidMPNumber);

      return dictionary;
    }

    private object GetBalancePoint(object atomicElement)
    {
      //it's necessary round the balance point because it has different returns at the last decimals 
      ((AtomicElement)atomicElement).GetBalancepoint(out var point, out var weigth);
      return new Point3d(Round(point.x), Round(point.y), Round(point.z));
    }

    private double Round(double value)
    {
      return Math.Round(value, 3, MidpointRounding.AwayFromZero);
    }

    private object GetHoles(object atomicElement)
    {
      var holes = HolesUtils.GetHoles((AtomicElement)atomicElement);

      List<Dictionary<string, object>> listHolesDetails = new List<Dictionary<string, object>>();

      foreach (var hole in holes)
      {
        hole.CS.GetCoordSystem(out var point, out var vectorX, out var vectorY, out var vectorZ);

        Dictionary<string, object> holeProperties = new Dictionary<string, object>();
        holeProperties.Add("Diameter", hole.Hole.Diameter.FromInternalDistanceUnits());
        holeProperties.Add("Center", point.ToDynPoint());
        holeProperties.Add("Normal", vectorZ.ToDynVector());

        listHolesDetails.Add(holeProperties);
      }

      return listHolesDetails;
    }

    private object HasValidSPNumber(object atomicElement)
    {
      ((AtomicElement)atomicElement).GetNumberingStatus(out bool hasValidSPNumber, out bool hasValidMPNumber);
      return hasValidSPNumber;
    }

    private object HasValidMPNumber(object atomicElement)
    {
      ((AtomicElement)atomicElement).GetNumberingStatus(out bool hasValidSPNumber, out bool hasValidMPNumber);
      return hasValidMPNumber;
    }
  }
}

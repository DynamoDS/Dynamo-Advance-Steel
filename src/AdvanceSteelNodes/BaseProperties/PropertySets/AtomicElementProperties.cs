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
  public class AtomicElementProperties : BaseProperties<AtomicElement>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Volume", nameof(AtomicElement.Volume), LevelEnum.Default);
      InsertItem(dictionary, "Used For Numbering - Assembly", nameof(AtomicElement.AssemblyUsedForNumbering));
      InsertItem(dictionary, "Used For Numbering - Note", nameof(AtomicElement.NoteUsedForNumbering));
      InsertItem(dictionary, "Used For Numbering - Role", nameof(AtomicElement.RoleUsedForNumbering));
      InsertItem(dictionary, "Used For BOM - Single Part", nameof(AtomicElement.SinglePartUsedForBOM));
      InsertItem(dictionary, "Used For BOM - Main Part", nameof(AtomicElement.MainPartUsedForBOM));
      InsertItem(dictionary, "Used For Collision Check - Single Part", nameof(AtomicElement.SinglePartUsedForCollisionCheck));
      InsertItem(dictionary, "Used For Collision Check - Main Part", nameof(AtomicElement.MainPartUsedForCollisionCheck));
      InsertItem(dictionary, "Structural Member", nameof(AtomicElement.StructuralMember));
      InsertItem(dictionary, "Used For Numbering - Holes", nameof(AtomicElement.HolesUsedForNumbering));
      InsertItem(dictionary, "MainPart Number", nameof(AtomicElement.MainPartNumber));
      InsertItem(dictionary, "SinglePart Number", nameof(AtomicElement.SinglePartNumber));
      InsertItem(dictionary, "Preliminary Part Prefix", nameof(AtomicElement.PreliminaryPartPrefix));
      InsertItem(dictionary, "Preliminary Part Number", nameof(AtomicElement.PreliminaryPartNumber));
      InsertItem(dictionary, "Preliminary Part Position Number", nameof(AtomicElement.PreliminaryPartPositionNumber));
      InsertItem(dictionary, "Used For Numbering - Item Number", nameof(AtomicElement.ItemNumberUsedForNumbering));
      InsertItem(dictionary, "Used For Numbering - Dennotation", nameof(AtomicElement.DennotationUsedForNumbering));
      InsertItem(dictionary, "Used For Numbering - Coating", nameof(AtomicElement.CoatingUsedForNumbering));
      InsertItem(dictionary, "Used For Numbering - Material", nameof(AtomicElement.MaterialUsedForNumbering));
      InsertItem(dictionary, "Unwind Start Factor", nameof(AtomicElement.UnwindStartFactor));
      InsertItem(dictionary, "Denotation", nameof(AtomicElement.Denotation));
      InsertItem(dictionary, "Assembly", nameof(AtomicElement.Assembly));
      InsertItem(dictionary, "Note", nameof(AtomicElement.Note));
      InsertItem(dictionary, "Item Number", nameof(AtomicElement.ItemNumber));
      InsertItem(dictionary, "Specific Gravity", nameof(AtomicElement.SpecificGravity));
      InsertItem(dictionary, "Coating", nameof(AtomicElement.Coating));
      InsertItem(dictionary, "Number Of Holes", nameof(AtomicElement.NumberOfHoles), LevelEnum.Default);
      InsertItem(dictionary, "Is Attached Part", nameof(AtomicElement.IsAttachedPart), LevelEnum.Default);
      InsertItem(dictionary, "Is Main Part", nameof(AtomicElement.IsMainPart));
      InsertItem(dictionary, "Main Part Prefix", nameof(AtomicElement.MainPartPrefix));
      InsertItem(dictionary, "Single Part Prefix", nameof(AtomicElement.SinglePartPrefix));
      InsertItem(dictionary, "Used For Numbering - SinglePart", nameof(AtomicElement.SinglePartUsedForNumbering));
      InsertItem(dictionary, "Used For Numbering - MainPart", nameof(AtomicElement.MainPartUsedForNumbering));
      InsertItem(dictionary, "Explicit Quantity", nameof(AtomicElement.ExplicitQuantity));
      InsertItem(dictionary, "Material Description", nameof(AtomicElement.MaterialDescription), LevelEnum.Default);
      InsertItem(dictionary, "Coating Description", nameof(AtomicElement.CoatingDescription), LevelEnum.Default);
      InsertItem(dictionary, "Material", nameof(AtomicElement.Material));
      InsertItem(dictionary, "Unwind", nameof(AtomicElement.Unwind));

      //Functions

      InsertItem(dictionary, "Balance Point", GetBalancePoint);
      InsertItem(dictionary, "Main Part Position", nameof(AtomicElement.GetMainPartPositionNumber));
      InsertItem(dictionary, "Model Quantity", nameof(AtomicElement.GetQuantityInModel));
      InsertItem(dictionary, "Single Part Position", nameof(AtomicElement.GetSinglePartPositionNumber));
      InsertItem(dictionary, "Features Number", nameof(AtomicElement.NumFeatures));
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

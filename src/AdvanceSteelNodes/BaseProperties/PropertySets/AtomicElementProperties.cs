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

      InsertProperty(dictionary, "Volume", nameof(AtomicElement.Volume), LevelEnum.Default);
      InsertProperty(dictionary, "Used For Numbering - Assembly", nameof(AtomicElement.AssemblyUsedForNumbering));
      InsertProperty(dictionary, "Used For Numbering - Note", nameof(AtomicElement.NoteUsedForNumbering));
      InsertProperty(dictionary, "Used For Numbering - Role", nameof(AtomicElement.RoleUsedForNumbering));
      InsertProperty(dictionary, "Used For BOM - Single Part", nameof(AtomicElement.SinglePartUsedForBOM));
      InsertProperty(dictionary, "Used For BOM - Main Part", nameof(AtomicElement.MainPartUsedForBOM));
      InsertProperty(dictionary, "Used For Collision Check - Single Part", nameof(AtomicElement.SinglePartUsedForCollisionCheck));
      InsertProperty(dictionary, "Used For Collision Check - Main Part", nameof(AtomicElement.MainPartUsedForCollisionCheck));
      InsertProperty(dictionary, "Structural Member", nameof(AtomicElement.StructuralMember));
      InsertProperty(dictionary, "Used For Numbering - Holes", nameof(AtomicElement.HolesUsedForNumbering));
      InsertProperty(dictionary, "MainPart Number", nameof(AtomicElement.MainPartNumber));
      InsertProperty(dictionary, "SinglePart Number", nameof(AtomicElement.SinglePartNumber));
      InsertProperty(dictionary, "Preliminary Part Prefix", nameof(AtomicElement.PreliminaryPartPrefix));
      InsertProperty(dictionary, "Preliminary Part Number", nameof(AtomicElement.PreliminaryPartNumber));
      InsertProperty(dictionary, "Preliminary Part Position Number", nameof(AtomicElement.PreliminaryPartPositionNumber));
      InsertProperty(dictionary, "Used For Numbering - Item Number", nameof(AtomicElement.ItemNumberUsedForNumbering));
      InsertProperty(dictionary, "Used For Numbering - Dennotation", nameof(AtomicElement.DennotationUsedForNumbering));
      InsertProperty(dictionary, "Used For Numbering - Coating", nameof(AtomicElement.CoatingUsedForNumbering));
      InsertProperty(dictionary, "Used For Numbering - Material", nameof(AtomicElement.MaterialUsedForNumbering));
      InsertProperty(dictionary, "Unwind Start Factor", nameof(AtomicElement.UnwindStartFactor));
      InsertProperty(dictionary, "Denotation", nameof(AtomicElement.Denotation));
      InsertProperty(dictionary, "Assembly", nameof(AtomicElement.Assembly));
      InsertProperty(dictionary, "Note", nameof(AtomicElement.Note));
      InsertProperty(dictionary, "Item Number", nameof(AtomicElement.ItemNumber));
      InsertProperty(dictionary, "Specific Gravity", nameof(AtomicElement.SpecificGravity));
      InsertProperty(dictionary, "Coating", nameof(AtomicElement.Coating));
      InsertProperty(dictionary, "Number Of Holes", nameof(AtomicElement.NumberOfHoles), LevelEnum.Default);
      InsertProperty(dictionary, "Is Attached Part", nameof(AtomicElement.IsAttachedPart), LevelEnum.Default);
      InsertProperty(dictionary, "Is Main Part", nameof(AtomicElement.IsMainPart));
      InsertProperty(dictionary, "Main Part Prefix", nameof(AtomicElement.MainPartPrefix));
      InsertProperty(dictionary, "Single Part Prefix", nameof(AtomicElement.SinglePartPrefix));
      InsertProperty(dictionary, "Used For Numbering - SinglePart", nameof(AtomicElement.SinglePartUsedForNumbering));
      InsertProperty(dictionary, "Used For Numbering - MainPart", nameof(AtomicElement.MainPartUsedForNumbering));
      InsertProperty(dictionary, "Explicit Quantity", nameof(AtomicElement.ExplicitQuantity));
      InsertProperty(dictionary, "Material Description", nameof(AtomicElement.MaterialDescription), LevelEnum.Default);
      InsertProperty(dictionary, "Coating Description", nameof(AtomicElement.CoatingDescription), LevelEnum.Default);
      InsertProperty(dictionary, "Material", nameof(AtomicElement.Material));
      InsertProperty(dictionary, "Unwind", nameof(AtomicElement.Unwind));

      //Functions

      InsertCustomProperty(dictionary, "Balance Point", nameof(AtomicElementProperties.GetBalancePoint), null);
      InsertProperty(dictionary, "Main Part Position", nameof(AtomicElement.GetMainPartPositionNumber));
      InsertProperty(dictionary, "Model Quantity", nameof(AtomicElement.GetQuantityInModel));
      InsertProperty(dictionary, "Single Part Position", nameof(AtomicElement.GetSinglePartPositionNumber));
      InsertProperty(dictionary, "Features Number", nameof(AtomicElement.NumFeatures));
      InsertCustomProperty(dictionary, "Holes (Properties)", nameof(AtomicElementProperties.GetHoles), null);
      InsertCustomProperty(dictionary, "Numbering - Valid Single Part", nameof(AtomicElementProperties.HasValidSPNumber), null);
      InsertCustomProperty(dictionary, "Numbering - Valid Main Part", nameof(AtomicElementProperties.HasValidMPNumber), null);

      return dictionary;
    }

    private Point3d GetBalancePoint(AtomicElement atomicElement)
    {
      //it's necessary round the balance point because it has different returns at the last decimals 
      atomicElement.GetBalancepoint(out var point, out var weigth);
      return new Point3d(Round(point.x), Round(point.y), Round(point.z));
    }

    private double Round(double value)
    {
      return Math.Round(value, 3, MidpointRounding.AwayFromZero);
    }

    private List<Dictionary<string, object>> GetHoles(AtomicElement atomicElement)
    {
      var holes = HolesUtils.GetHoles(atomicElement);

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

    private bool HasValidSPNumber(AtomicElement atomicElement)
    {
      atomicElement.GetNumberingStatus(out bool hasValidSPNumber, out bool hasValidMPNumber);
      return hasValidSPNumber;
    }

    private bool HasValidMPNumber(AtomicElement atomicElement)
    {
      atomicElement.GetNumberingStatus(out bool hasValidSPNumber, out bool hasValidMPNumber);
      return hasValidMPNumber;
    }
  }
}

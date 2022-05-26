using Autodesk.AdvanceSteel.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;
using static Autodesk.AdvanceSteel.DotNetRoots.Units.Unit;

namespace AdvanceSteel.Nodes
{
  public class FoldedPlateProperties : BaseProperties<FoldedPlate>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Main Fold Id", nameof(FoldedPlate.MainFoldId));
      InsertCustomProperty(dictionary, "Fold Plates", nameof(FoldedPlateProperties.GetFoldPlates), null);

      return dictionary;
    }

    private static List<Dictionary<string, object>> GetFoldPlates(FoldedPlate foldedPlate)
    {
      List<Dictionary<string, object>> listHolesDetails = new List<Dictionary<string, object>>();

      foreach (var foldPlate in foldedPlate.GetAllFolds())
      {
        Dictionary<string, object> foldPlateDictionary = new Dictionary<string, object>();
        foldPlateDictionary.Add("Id", foldPlate.Id);
        foldPlateDictionary.Add("Diameter", foldPlate.Normal.ToDynVector());

        listHolesDetails.Add(foldPlateDictionary);
      }

      return listHolesDetails;
    }
  }
}

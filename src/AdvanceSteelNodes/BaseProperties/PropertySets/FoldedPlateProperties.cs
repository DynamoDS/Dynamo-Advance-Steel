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

      InsertItem(dictionary, "Main Fold Id", nameof(FoldedPlate.MainFoldId));
      InsertItem(dictionary, "Fold Plates", GetFoldPlates);

      return dictionary;
    }

    private object GetFoldPlates(object foldedPlate)
    {
      FoldedPlate asFoldedPlate = (FoldedPlate)foldedPlate;

      List<Dictionary<string, object>> listHolesDetails = new List<Dictionary<string, object>>();

      foreach (var foldPlate in asFoldedPlate.GetAllFolds())
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

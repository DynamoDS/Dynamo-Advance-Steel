using Autodesk.AdvanceSteel.Arrangement;
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
  public class ArrangerProperties : BaseProperties<Arranger>, IASProperties
  {
    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertProperty(dictionary, "Arranger No of Holes in the Y Direction", nameof(Arranger.Ny), LevelEnum.Arranger);
      InsertProperty(dictionary, "Arranger No of Holes in the X Direction", nameof(Arranger.Nx), LevelEnum.Arranger);
      InsertProperty(dictionary, "Arranger Spacing of holes in the y Direction", nameof(Arranger.Dy), LevelEnum.Arranger);
      InsertProperty(dictionary, "Arranger Spacing of holes in the X Direction", nameof(Arranger.Dx), LevelEnum.Arranger);
      InsertProperty(dictionary, "Arranger Hole Pattern Width in the Y Direction", nameof(Arranger.Wy), LevelEnum.Arranger);
      InsertProperty(dictionary, "Arranger Hole Pattern Width in the X Direction", nameof(Arranger.Wx), LevelEnum.Arranger);
      InsertProperty(dictionary, "Arranger Width", nameof(Arranger.Width), LevelEnum.Arranger, eUnitType.kDistance);
      InsertProperty(dictionary, "Arranger Length", nameof(Arranger.Length), LevelEnum.Arranger, eUnitType.kDistance);
      InsertProperty(dictionary, "Arranger Number Of Elements", nameof(Arranger.NumberOfElements), LevelEnum.Arranger);
      InsertProperty(dictionary, "Arranger Radius", nameof(Arranger.Radius), LevelEnum.Arranger);
      InsertProperty(dictionary, "Arranger Center", nameof(Arranger.Center), LevelEnum.Arranger);
      InsertProperty(dictionary, "Arranger Origin", nameof(Arranger.Origin), LevelEnum.Arranger);

      return dictionary;
    }
  }
}
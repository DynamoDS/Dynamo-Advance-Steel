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
  public class ArrangerProperties : BaseProperties, IASProperties
  {
    public override Type GetObjectType => typeof(Arranger);

    public override Dictionary<string, Property> BuildPropertyList()
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, "Arranger No of Holes in the Y Direction", nameof(Arranger.Ny), LevelEnum.Arranger);
      InsertItem(dictionary, "Arranger No of Holes in the X Direction", nameof(Arranger.Nx), LevelEnum.Arranger);
      InsertItem(dictionary, "Arranger Spacing of holes in the y Direction", nameof(Arranger.Dy), LevelEnum.Arranger);
      InsertItem(dictionary, "Arranger Spacing of holes in the X Direction", nameof(Arranger.Dx), LevelEnum.Arranger);
      InsertItem(dictionary, "Arranger Hole Pattern Width in the Y Direction", nameof(Arranger.Wy), LevelEnum.Arranger);
      InsertItem(dictionary, "Arranger Hole Pattern Width in the X Direction", nameof(Arranger.Wx), LevelEnum.Arranger);
      InsertItem(dictionary, "Arranger Width", nameof(Arranger.Width), LevelEnum.Arranger, eUnitType.kDistance);
      InsertItem(dictionary, "Arranger Length", nameof(Arranger.Length), LevelEnum.Arranger, eUnitType.kDistance);
      InsertItem(dictionary, "Arranger Number Of Elements", nameof(Arranger.NumberOfElements), LevelEnum.Arranger);
      InsertItem(dictionary, "Arranger Radius", nameof(Arranger.Radius), LevelEnum.Arranger);
      InsertItem(dictionary, "Arranger Center", nameof(Arranger.Center), LevelEnum.Arranger);
      InsertItem(dictionary, "Arranger Origin", nameof(Arranger.Origin), LevelEnum.Arranger);

      return dictionary;
    }
  }
}
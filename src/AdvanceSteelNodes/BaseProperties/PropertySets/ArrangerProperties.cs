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
    public override eObjectType GetObjectType => eObjectType.kConnectionHoleFeature;

    public override Dictionary<string, Property> BuildPropertyList(Type objectASType)
    {
      Dictionary<string, Property> dictionary = new Dictionary<string, Property>();

      InsertItem(dictionary, objectASType, "Arranger No of Holes in the Y Direction", nameof(Arranger.Ny), LevelEnum.Arranger);
      InsertItem(dictionary, objectASType, "Arranger No of Holes in the X Direction", nameof(Arranger.Nx), LevelEnum.Arranger);
      InsertItem(dictionary, objectASType, "Arranger Spacing of holes in the y Direction", nameof(Arranger.Dy), LevelEnum.Arranger);
      InsertItem(dictionary, objectASType, "Arranger Spacing of holes in the X Direction", nameof(Arranger.Dx), LevelEnum.Arranger);
      InsertItem(dictionary, objectASType, "Arranger Hole Pattern Width in the Y Direction", nameof(Arranger.Wy), LevelEnum.Arranger);
      InsertItem(dictionary, objectASType, "Arranger Hole Pattern Width in the X Direction", nameof(Arranger.Wx), LevelEnum.Arranger);
      InsertItem(dictionary, objectASType, "Arranger Width", nameof(Arranger.Width), LevelEnum.Arranger, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Arranger Length", nameof(Arranger.Length), LevelEnum.Arranger, eUnitType.kDistance);
      InsertItem(dictionary, objectASType, "Arranger Number Of Elements", nameof(Arranger.NumberOfElements), LevelEnum.Arranger);
      InsertItem(dictionary, objectASType, "Arranger Radius", nameof(Arranger.Radius), LevelEnum.Arranger);
      InsertItem(dictionary, objectASType, "Arranger Center", nameof(Arranger.Center), LevelEnum.Arranger);
      InsertItem(dictionary, objectASType, "Arranger Origin", nameof(Arranger.Origin), LevelEnum.Arranger);

      return dictionary;
    }
  }
}
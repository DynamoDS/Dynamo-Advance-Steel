using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;

namespace AdvanceSteel.Nodes.Util
{
  /// <summary>
  /// This node could be used to tag Advance Steel elements from Dynamo and search for them in DWG
  /// </summary>
  public class UserAttribute
  {
    /// <summary>
    /// do not expose it to the gui
    /// </summary>
    internal UserAttribute()
    {
    }

    /// <summary>
    /// This node could be used to tag Advance Steel elements from Dynamo and search for them in DWG
    /// </summary>
    /// <param name="element"> Advance steel element</param>
    /// <param name="AttIdx"> The index of the user atribute. Is a number between 1 and 10</param>
    /// <param name="value"> The value</param>
    /// <returns></returns>
    public static void SetUserAttribute(AdvanceSteel.Nodes.Object element, int AttIdx, string value)
    {
      if (AttIdx < 1 || AttIdx > 10)
        throw new System.Exception("AttIdx is not in rage 1-10");

      //lock the document and start transaction
      using (var _CADAccess = new AdvanceSteel.Services.ObjectAccess.CADContext())
      {
        string handle = element.Handle;

        FilerObject obj = Utils.GetObject(handle);
        AtomicElement atomic = obj as AtomicElement;

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kAtomicElem))
        {
          //[1, 10] ->[0 ,9]
          AttIdx = AttIdx - 1;

          atomic.SetUserAttribute(AttIdx, value);
        }
        else
          throw new System.Exception("Failed to set attribute");
      }
    }
  }
}
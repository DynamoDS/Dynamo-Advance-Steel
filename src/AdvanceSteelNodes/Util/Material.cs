using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using System;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Util
{
  /// <summary>
  /// This node can be used to assign Material to Advance Steel elements from Dynamo
  /// </summary>
  public class Material
  {
    internal Material()
    {
    }
    /// <summary>
    /// This node can set the Material for Advance Steel elements from Dynamo
    /// </summary>
    /// <param name="element">Advance Steel element</param>
    /// <param name="materialName">Material</param>
    /// <returns></returns>
    [Obsolete]
    public static void SetMaterial(AdvanceSteel.Nodes.SteelDbObject element, string materialName)
    {
      //lock the document and start transaction
      using (var ctx = new SteelServices.DocContext())
      {
        string handle = element.Handle;

        FilerObject obj = Utils.GetObject(handle);

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kAtomicElem))
        {
          AtomicElement atomic = obj as AtomicElement;
          atomic.Material = materialName;
        }
        else
          throw new System.Exception("Failed to set material");
      }
    }
  }
}
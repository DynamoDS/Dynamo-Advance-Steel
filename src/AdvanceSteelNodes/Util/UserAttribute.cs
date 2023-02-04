using System;
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Util
{
  /// <summary>
  /// Custom User attributes for Advance Steel elements
  /// </summary>
  public class UserAttribute
  {
    internal UserAttribute()
    {
    }
    /// <summary>
    /// This node can set User attributes for Advance Steel elements from Dynamo
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <param name="AttIdx">The index of the User attribute. Is a number between 1 and 10</param>
    /// <param name="value">Attribute value</param>
    /// <returns></returns>
    [Obsolete("UserAttribute.SetUserAttribute is obsolete, please use SteelObject.SetUserAttribute instead.")]
    public static void SetUserAttribute(AdvanceSteel.Nodes.SteelDbObject steelObject, int AttIdx, string value)
    {
      if (AttIdx < 1 || AttIdx > 10)
        throw new System.Exception("Attribute index is not in the range from 1 to 10");

      using (var ctx = new SteelServices.DocContext())
      {
        string handle = steelObject.Handle;

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

    /// <summary>
    /// This node can get User attributes for Advance Steel elements from Dynamo
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <param name="AttIdx">The index of the User attribute. Is a number between 1 and 10</param>
    /// <returns></returns>
    [Obsolete("UserAttribute.GetUserAttribute is obsolete, please use SteelObject.GetUserAttribute instead.")]
    public static string GetUserAttribute(AdvanceSteel.Nodes.SteelDbObject steelObject, int AttIdx)
    {
      string ret = string.Empty;

      if (AttIdx < 1 || AttIdx > 10)
        throw new System.Exception("Attribute index is not in the range from 1 to 10");

      using (var ctx = new SteelServices.DocContext())
      {
        string handle = steelObject.Handle;

        FilerObject obj = Utils.GetObject(handle);
        AtomicElement atomic = obj as AtomicElement;

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kAtomicElem))
        {
          //[1, 10] ->[0 ,9]
          AttIdx = AttIdx - 1;

          ret = atomic.GetUserAttribute(AttIdx);
        }
        else
          throw new System.Exception("Failed to get attribute");
      }

      return ret;
    }
  }
}
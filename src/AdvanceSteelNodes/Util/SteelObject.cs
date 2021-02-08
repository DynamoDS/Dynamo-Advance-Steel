using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Util
{
  /// <summary>
  /// Generic Function for Advance Steel elements
  /// </summary>
  public class SteelObject
  {
    internal SteelObject()
    {
    }

    /// <summary>
    /// Get Quantity in Model
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <returns></returns>
    public static int GetQuantityInModel(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      int ret = -1;
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kAtomicElem))
            {
              AtomicElement selectedObj = filerObj as AtomicElement;
              ret = (int)selectedObj.GetQuantityInModel();
            }
            else
              throw new System.Exception("Not a BEAM Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object or Point is null");
      }
      return ret;
    }

    /// <summary>
    /// This node can set User attributes for Advance Steel elements from Dynamo
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <param name="AttIdx">The index of the User attribute. Is a number between 1 and 10</param>
    /// <param name="value">Attribute value</param>
    /// <returns></returns>
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

    /// <summary>
    /// Get Beam Weight based on display code
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <param name="weightCode">1 = Standard, 2 = Exact, 3 = Fast</param>
    /// <returns></returns>
    public static double GetWeight(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                    int weightCode)
    {
      double ret = 0;
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          ret = Utils.GetWeight(steelObject.Handle, weightCode);
        }
        else
          throw new System.Exception("Steel Object or Point is null");
      }
      return Utils.FromInternalWeightUnits(ret, true);
    }

    /// <summary>
    /// Get Beam Paint Area
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <returns></returns>
    public static double GetPaintArea(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      double ret = 0;
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          ret = Utils.GetPaintArea(steelObject.Handle);
        }
        else
          throw new System.Exception("Steel Object or Point is null");
      }
      return Utils.FromInternalAreaUnits(ret, true);
    }
  }
}
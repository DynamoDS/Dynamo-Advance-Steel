using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.DocumentManagement;
using Autodesk.DesignScript.Runtime;
using Dynamo.Applications.AdvanceSteel.Services;
using System;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.ConstructionTypes;

namespace AdvanceSteel.Nodes
{
  [IsVisibleInDynamoLibrary(false)]
  public interface IASProperty
  {    
    object PropValue
    {
      get;
      set;
    }

    string PropLevel
    {
      get;
      set;
    }

    string PropName
    {
      get;
      set;
    }
    
  //Mod 2 = 0 & Mod 3 = 0
  //[IsVisibleInDynamoLibrary(false)]
  //public enum eProperty_Data_Ops : int
  //{
  //  Set = 2,
  //  Get = 3,
  //  Set_Get = 6
  //}
    int PropertyDataOp
    {
      get;
      set;
    }

    void UpdateASObject(object asObject);

    bool hasValidValue();
  }

  [IsVisibleInDynamoLibrary(false)]
  public static class ePropertyDataOperator
  {
    public static readonly int Set = 2;
    public static readonly int Get = 3;
    public static readonly int Set_Get = 6;
  }
}
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

    void UpdateASObject(object asObject);

    bool hasValidValue();
  }
}
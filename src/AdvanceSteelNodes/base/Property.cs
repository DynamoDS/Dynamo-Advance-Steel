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
  public class Property : IASProperty
  {

    private string _propName;
    private string _propLevel =".";
    private System.Type _objectValueType;
    private object _objectValue;
    private int _propertyDataOp = 6;// eProperty_Data_Ops.Set_Get;
    
    public Property(string propName, System.Type propType, string propLevel = ".", int propertyDataOp = 6)
    {
      PropName = propName;
      _objectValueType = propType;
      PropLevel = propLevel;
      PropertyDataOp = propertyDataOp;
    }

    public Property(string propName, object propValue, System.Type propType, string propLevel = ".", int propertyDataOp = 6)
    {
      PropName = propName;
      _objectValueType = propType;
      PropValue = propValue;
      PropLevel = propLevel;
      PropertyDataOp = propertyDataOp;
    }

    public int PropertyDataOp
    {
      get
      {
        return _propertyDataOp;
      }
      set
      {
        _propertyDataOp = value;
      }
    }

    public object PropValue
    {
      get
      {
        return _objectValue;
      }
      set
      {
        _objectValue = value;
      }
    }

    public string PropName
    {
      get
      {
        return _propName;
      }
      set
      {
        _propName = value;
      }
    }

    public string PropLevel
    {
      get
      {
        return _propLevel;
      }
      set
      {
        _propLevel = value;
      }
    }

    public void UpdateASObject(object asObjectToUpdate)
    {
      if (hasValidValue())
      {
        if (asObjectToUpdate != null)
        {
          asObjectToUpdate.GetType().GetProperty(PropName).SetValue(asObjectToUpdate, PropValue);
        }
      }
    }

    public bool hasValidValue()
    {
      bool retValue = false;
      if (PropValue != null)
      {
        if (_objectValueType == typeof(int))
        {
          if (typeof(int).Equals(PropValue.GetType())) { retValue = true; }
          if (typeof(Int32).Equals(PropValue.GetType())) { retValue = true; }
          if (typeof(Int64).Equals(PropValue.GetType())) { retValue = true; }
          if (retValue)
          {
            PropValue = Convert.ToInt32(PropValue);
          }
        }
        else
        {
          retValue = _objectValueType.Equals(PropValue.GetType());
        }
      }

      return retValue;
    }

  }
}
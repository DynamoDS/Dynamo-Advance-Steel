using System.Collections.Generic;
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.DocumentManagement;
using Autodesk.DesignScript.Runtime;
using Dynamo.Applications.AdvanceSteel.Services;
using System;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.ConstructionTypes;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;

namespace AdvanceSteel.Nodes
{
  [IsVisibleInDynamoLibrary(false)]
  public class ASProperty : IASProperty
  {
    private string _propName;
    private string _propLevel = ".";
    private System.Type _objectValueType;
    private object _objectValue;
    private int _propertyDataOp = ePropertyDataOperator.Set_Get;
    private List<eObjectType> _elementType;

    public ASProperty(string propName, System.Type propType, string propLevel = ".", int propertyDataOp = 6)
    {
      PropName = propName;
      _objectValueType = propType;
      PropLevel = propLevel;
      PropertyDataOp = propertyDataOp;
    }

    public ASProperty(string propName, object propValue, System.Type propType, string propLevel = ".", int propertyDataOp = 6)
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

    public List<eObjectType> ElementTypeList
    {
      set
      {
        _elementType = value;
      }
      get
      {
        return _elementType;
      }
    }

    public T GetFormatedValue<T>()
    {
      if (PropValue == null) { return default(T); }
      return (T)PropValue;
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

    public override string ToString()
    {
      return PropName?.ToString() + " = " + PropValue?.ToString();
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

    public bool EvaluateValueFromSteelDBObject(SteelDbObject steelObject)
    {
      bool ret = false;
      if (steelObject != null)
      {
        FilerObject fObj = Utils.GetObject(steelObject.Handle);
        try
        {
          PropValue = fObj.GetType().GetProperty(PropName).GetValue(fObj, null);
          ret = true;
        }
        catch (Exception)
        {
          throw new System.Exception("Object Has no Property - " + PropName);
        }
      }
      else
      {
        throw new System.Exception("Invalid / Empty SteelDBObject");
      }
      return ret;
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
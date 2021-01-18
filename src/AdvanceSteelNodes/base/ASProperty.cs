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
  public class ASProperty
  {
    private string _propName;
    private string _propLevel = ".";
    private System.Type _objectValueType;
    private object _objectValue;
    private int _propertyDataOp = ePropertyDataOperator.Set_Get;
    private List<eObjectType> _elementType;

    public ASProperty(string name, System.Type propType, string level = ".", int dataOperator = 6)
    {
      Name = name;
      _objectValueType = propType;
      Level = level;
      DataOperator = dataOperator;
    }

    public ASProperty(string propName, object propValue, System.Type propType, string propLevel = ".", int dataOperator = 6)
    {
      Name = propName;
      _objectValueType = propType;
      Value = propValue;
      Level = propLevel;
      DataOperator = dataOperator;
    }

    public int DataOperator
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
      if (Value == null) { return default(T); }
      return (T)Value;
    }

    public object Value
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

    public string Name
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

    public string Level
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
      return Name?.ToString() + " = " + Value?.ToString();
    }

    public void UpdateASObject(object asObjectToUpdate)
    {
      if (hasValidValue())
      {
        if (asObjectToUpdate != null)
        {
          asObjectToUpdate.GetType().GetProperty(Name).SetValue(asObjectToUpdate, Value);
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
          Value = fObj.GetType().GetProperty(Name).GetValue(fObj, null);
          ret = true;
        }
        catch (Exception)
        {
          throw new System.Exception("Object Has no Property - " + Name);
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
      if (Value != null)
      {
        if (_objectValueType == typeof(int))
        {
          if (typeof(int).Equals(Value.GetType())) { retValue = true; }
          if (typeof(Int32).Equals(Value.GetType())) { retValue = true; }
          if (typeof(Int64).Equals(Value.GetType())) { retValue = true; }
          if (retValue)
          {
            Value = Convert.ToInt32(Value);
          }
        }
        else
        {
          retValue = _objectValueType.Equals(Value.GetType());
        }
      }

      return retValue;
    }

  }

  [IsVisibleInDynamoLibrary(false)]
  public static class ePropertyDataOperator
  {
    public static readonly int Set = 2;
    public static readonly int Get = 3;
    public static readonly int Set_Get = 6;
  }
}
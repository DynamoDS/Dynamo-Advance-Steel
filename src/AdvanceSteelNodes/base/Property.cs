using System.Collections.Generic;
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.DocumentManagement;
using Autodesk.DesignScript.Runtime;
using Dynamo.Applications.AdvanceSteel.Services;
using System;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.ConstructionTypes;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;

namespace AdvanceSteel.Nodes
{
  /// <summary>
  /// Key value data structure used to get or set steel object properties
  /// </summary>
  public class Property
  {
    private string _name;
    private System.Type _valueType;
    private object _value;
    private bool _isReadOnly;

    internal List<eObjectType> ElementTypeList { get; set; }
    internal int DataOperator { get; }
    internal string Level { get; }

    internal Property(string name, System.Type propType, string level = ".", bool isReadOnly = false)
    {
      Name = name;
      _valueType = propType;
      Level = level;
      _isReadOnly = isReadOnly;
    }

    internal Property(string name, object value, System.Type propType, string level = ".", bool isReadOnly = false)
    {
      Name = name;
      _valueType = propType;
      Value = value;
      Level = level;
      _isReadOnly = isReadOnly;
    }

    /// <summary>
    /// Get the value from the property
    /// </summary>
    public object Value
    {
      get
      {
        return _value;
      }
      internal set
      {
        if (_valueType == typeof(int) && IsInteger(value))
          _value = Convert.ToInt32(value);
        else if (_valueType == typeof(double) && IsInteger(value))
          _value = Convert.ToDouble(Value);
        else if (_valueType.Equals(value.GetType()))
          _value = value;
        else
          throw new System.Exception("invalid value");          
      }
    }

    /// <summary>
    /// Get the name of the property
    /// </summary>
    public string Name
    {
      get
      {
        return _name;
      }
      internal set
      {
        _name = value;
      }
    }

    /// <summary>
    /// Check if this property is readonly
    /// </summary>
    public bool IsReadOnly
    {
      get
      {
        return _isReadOnly;
      }
    }

    /// <summary>
    /// Create a Property object
    /// </summary>
    /// <param name="name"> Name from property list node for a particular steel object type</param>
    /// <param name="value"> native data to store in the property object</param>
    /// <returns></returns>
    public static Property ByNameAndValue(string name, object value)
    {
      Property selectedProperty = Utils.GetProperty(name);
      if (selectedProperty != null)
      {
         selectedProperty.Value = value;
      }
      else
        throw new System.Exception("No property found for the given name");
      return selectedProperty;
    }

    /// <summary>
    /// Get a property from a steel object
    /// </summary>
    /// <param name="steelObject"> Steel object</param>
    /// <param name="propertyName"> Name of the property</param>
    /// <returns></returns>
    public static Property GetObjectProperty(SteelDbObject steelObject, string propertyName)
    {
      Property ret = null;
      using (var ctx = new SteelServices.DocContext())
      {
        FilerObject filerObj = Utils.GetObject(steelObject.Handle);

        Property extractionProperty = Utils.GetProperty(propertyName);
        if (extractionProperty != null)
        {
          if (extractionProperty.EvaluateFromObject(filerObj))
          {
            ret = extractionProperty;
          }
        }
        else
        {
          throw new System.Exception("No property found for the given name");
        }
      }
      return ret;
    }
    /// <summary>
    /// Set a property for a Steel Object
    /// </summary>
    /// <param name="steelObject"> Steel Object</param>
    /// <param name="property"> Property object</param>
    /// <returns></returns>
    public static SteelDbObject SetObjectProperty(SteelDbObject steelObject, Property property)
    {
      if (property.IsReadOnly)
        throw new System.Exception("property is readonly");

      using (var ctx = new SteelServices.DocContext())
      {
        FilerObject fo = Utils.GetObject(steelObject.Handle);
        if (fo != null)
        {
          property.SetToObject(fo);
          return steelObject;
        }
        else
          throw new System.Exception("No Advance Steel Object Found");
      }
    }

    /// <summary>
    /// Get all properties from a steel object
    /// </summary>
    /// <param name="steelObject"> Steel object</param>
    /// <returns></returns>
    public static List<Property> GetObjectProperties(SteelDbObject steelObject)
    {
      List<Property> ret = new List<Property>() { };
      using (var ctx = new SteelServices.DocContext())
      {
        Dictionary<string, Property> allProperties = Utils.GetAllProperties();
        FilerObject filerObj = Utils.GetObject(steelObject.Handle);

        foreach (KeyValuePair<string, Property> prop in allProperties)
        {
          if (prop.Value.ElementTypeList.Contains(filerObj.Type()))
          {
            Property extractionProperty = prop.Value;
            if (extractionProperty != null)
            {
              if (extractionProperty.EvaluateFromObject(filerObj))
              {
                ret.Add(extractionProperty);
              }
            }
          }
        }
      }
      return ret;
    }

    public override string ToString()
    {
      return Name?.ToString() + " = " + Value?.ToString();
    }

    internal bool SetToObject(object objectToUpdate)
    {
      if (objectToUpdate != null && !IsReadOnly)
      {
        objectToUpdate.GetType().GetProperty(Name).SetValue(objectToUpdate, Value);
        return true;
      }

      return false;
    }

    internal bool EvaluateFromObject(object objectToUpdateFrom)
    {
      if (objectToUpdateFrom != null)
      {
        try
        {
          Value = objectToUpdateFrom.GetType().GetProperty(Name).GetValue(objectToUpdateFrom, null);
          return true;
        }
        catch (Exception)
        {
          throw new System.Exception("Object Has no Property - " + Name);
        }
      }
      else
      {
        throw new System.Exception("Null object");
      }
    }

    internal static bool IsInteger(object value)
    {
      if (value != null)
      {
        if (
          typeof(int).Equals(value.GetType()) ||
          typeof(Int32).Equals(value.GetType()) ||
          typeof(Int64).Equals(value.GetType())
          )
        {
          return true;
        }
      }

      return false;
    }
  }
}
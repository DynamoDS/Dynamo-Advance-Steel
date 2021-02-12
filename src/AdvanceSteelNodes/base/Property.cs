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
using Autodesk.AdvanceSteel.DotNetRoots.Units;

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

    internal List<eObjectType> ElementTypeList { get; set; } = new List<eObjectType>() { };
    internal string Level { get; }
    internal Unit.eUnitType? UnitType { get; }

    internal Property(string name, System.Type valueType, string level = ".", bool isReadOnly = false)
    {
      _isReadOnly = isReadOnly;
      _name = name;
      _valueType = valueType;
      
      Level = level;
    }

    internal Property(string name, System.Type valueType, Unit.eUnitType unitType, string level = ".", bool isReadOnly = false)
    {
      _isReadOnly = isReadOnly;
      _name = name;
      _valueType = valueType;

      Level = level;
      UnitType = unitType;
    }
    internal Property(string name, object internalValue, System.Type valueType, string level = ".", bool isReadOnly = false)
    {
      _isReadOnly = isReadOnly;
      _name = name;
      _valueType = valueType;
      
      InternalValue = internalValue;
      Level = level;
    }

    /// <summary>
    /// Get the value from the property
    /// </summary>
    /// <param name="property">The input property</param>
    /// <returns name="Value">The value of the property</returns>
    public object Value
    {
      get
      {
        return ConvertValueFromASToDyn(InternalValue);
      }
    }

    /// <summary>
    /// Get the name of the property
    /// </summary>
    /// <param name="property">The input property</param>
    /// <returns name="Name">The name of the property</returns>
    public string Name
    {
      get
      {
        return _name;
      }
    }

    /// <summary>
    /// Check if this property is readonly
    /// </summary>
    /// <param name="property">The input property</param>
    /// <returns name="IsReadOnly">The read status of the property</returns>
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
    /// <param name="propertyName"> Name from property list node for a particular steel object type</param>
    /// <param name="value"> Native data to store in the property object</param>
    /// <returns name="property"> property</returns>
    public static Property ByNameAndValue(string propertyName, object value)
    {
      Property selectedProperty = Utils.GetProperty(propertyName);
      if (selectedProperty != null)
      {
         selectedProperty.InternalValue = selectedProperty.ConvertValueFromDynToAS(value);
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
    /// <returns name="property">The desired property</returns>
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
    /// <returns name="steelObject"> The updated steel object</returns>
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
    /// <returns name="properties"> List with all properties extracted from the input object</returns>
    public static List<Property> GetObjectProperties(SteelDbObject steelObject)
    {
      List<Property> ret = new List<Property>() { };
      using (var ctx = new SteelServices.DocContext())
      {
        FilerObject filerObj = Utils.GetObject(steelObject.Handle);
        Dictionary<string, Property> allProperties = Utils.GetAllProperties(filerObj);

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

    #region internal stuff

    /// <summary>
    /// This property keeps the property value in the internal units and uses AS API classes: like Matrix3d, Point3d, Vector3d
    /// </summary>
    internal object InternalValue
    {
      get
      {
        return _value;
      }
      set
      {
        object valueToSet = value;
        if (_valueType == typeof(int) && IsInteger(valueToSet))
          _value = Convert.ToInt32(valueToSet);
        else if (_valueType == typeof(double) && IsInteger(valueToSet))
          _value = Convert.ToDouble(valueToSet);
        else if (_valueType.Equals(valueToSet.GetType()))
          _value = valueToSet;
        else
          throw new System.Exception("invalid value");
      }
    }

    internal bool SetToObject(object objectToUpdate)
    {
      if (IsReadOnly)
        throw new System.Exception("Cannot set readonly property: " + Name?.ToString());

      if (objectToUpdate != null && !IsReadOnly)
      {
        objectToUpdate.GetType().GetProperty(Name).SetValue(objectToUpdate, InternalValue);
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
          InternalValue = objectToUpdateFrom.GetType().GetProperty(Name).GetValue(objectToUpdateFrom, null);
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

    internal object ConvertValueFromDynToAS(object val)
    {
      if (val == null)
        return null;

      if (val.GetType() == typeof(Autodesk.DesignScript.Geometry.Point))
      {
        return Utils.ToAstPoint(val as Autodesk.DesignScript.Geometry.Point, true);
      }
      else if (val.GetType() == typeof(Autodesk.DesignScript.Geometry.Vector))
      {
        return Utils.ToAstVector3d(val as Autodesk.DesignScript.Geometry.Vector, true);
      }
      else if (val.GetType() == typeof(Autodesk.DesignScript.Geometry.CoordinateSystem))
      {
        return Utils.ToAstMatrix3d(val as Autodesk.DesignScript.Geometry.CoordinateSystem, true);
      }
      else if ((IsDouble(val) || IsInteger(val)) && this.UnitType.HasValue)
      {
        return Utils.ToInternalUnits(Convert.ToDouble(val), this.UnitType.Value, true);
      }
      else
      {
        return val;
      }
    }

    internal object ConvertValueFromASToDyn(object val)
    {
      if (val == null)
        return null;

      if (val.GetType() == typeof(Point3d))
      {
        return Utils.ToDynPoint(val as Point3d, true);
      }
      else if (val.GetType() == typeof(Vector3d))
      {
        return Utils.ToDynVector(val as Vector3d, true);
      }
      else if (val.GetType() == typeof(Matrix3d))
      {
        return Utils.ToDynCoordinateSys(val as Matrix3d, true);
      }
      else if ((IsDouble(val) || IsInteger(val)) && this.UnitType.HasValue)
      {
        return Utils.FromInternalUnits(Convert.ToDouble(val), this.UnitType.Value, true);
      }
      else
      {
        return val;
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

    internal static bool IsDouble(object value)
    {
      if (value != null)
      {
        return value.GetType() == typeof(double);
      }

      return false;
    }
    #endregion
  }
}
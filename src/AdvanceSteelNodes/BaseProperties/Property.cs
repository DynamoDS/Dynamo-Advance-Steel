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
using System.Reflection;
using static Autodesk.AdvanceSteel.DotNetRoots.Units.Unit;

namespace AdvanceSteel.Nodes
{
  /// <summary>
  /// Key value data structure used to get or set steel object properties
  /// </summary>
  public class Property
  {
    private Type _objectType;
    private string _description;
    private string _memberName;
    private MemberInfo _memberInfo;
    private System.Type _valueType;
    private Func<object, object> _funcGetValue;

    private object _value;
    private bool _read;
    private bool _write;

    private bool _inicialized;

    internal LevelEnum Level { get; }
    private eUnitType? UnitType { get; }

    internal Property(Property existingProperty)
    {
      _read = existingProperty._read;
      _write = existingProperty._write;
      _objectType = existingProperty._objectType;
      _description = existingProperty._description;
      _memberInfo = existingProperty._memberInfo;
      _memberName = existingProperty._memberName;
      _valueType = existingProperty._valueType;
      _funcGetValue = existingProperty._funcGetValue;

      _inicialized = existingProperty._inicialized;

      UnitType = existingProperty.UnitType;
      Level = existingProperty.Level;
    }

    internal Property(string description)
    {
      _description = description;
    }

    internal Property(string memberName, object value)
    {
      _memberName = memberName;
      Value = value;
    }

    internal Property(Type objectASType, string description, string memberName, LevelEnum level = LevelEnum.NoDefinition, eUnitType? unitType = null)
    {
      _objectType = objectASType;
      _description = description;
      _memberName = memberName;
      
      _inicialized = true;
      UnitType = unitType;
      Level = level;

      PropertyInfo property = objectASType.GetProperty(memberName);
      if (property != null)
      {
        _valueType = property.PropertyType;
        _read = property.CanRead;
        _write = property.CanWrite;

        _memberInfo = property;
        return;
      }

      MethodInfo method = objectASType.GetMethod(memberName, new Type[] { });
      if (method != null)
      {
        _valueType = method.ReturnType;
        _read = true;
        _write = false;

        _memberInfo = method;
        return;
      }

      throw new Exception(string.Format("'{0}' is not a property nor method with no arguments", memberName));
    }

    internal Property(Type objectType, string description, Func<object, object> funcGetValue, LevelEnum level = LevelEnum.NoDefinition, eUnitType? unitType = null)
    {
      _inicialized = true;

      _objectType = objectType;
      _description = description;
      UnitType = unitType;

      _funcGetValue = funcGetValue;
      _read = true;
      _write = false;

      Level = level;

      //Same name for description and membername
      _memberName = description;
    }

    /// <summary>
    /// Get the value from the property
    /// </summary>
    /// <returns name="Value">The value of the property</returns>
    public object Value
    {
      get
      {
        return ConvertToDynamoObject(InternalValue);
      }
      set
      {
        InternalValue = ConvertFromDynamoObject(value);
      }
    }

    /// <summary>
    /// Get the name of the property
    /// </summary>
    /// <returns name="Name">The name of the property</returns>
    public string Name
    {
      get
      {
        return _description;
      }
    }

    /// <summary>
    /// Get the name of property member
    /// </summary>
    /// <returns name="MemberName">The name of property member</returns>
    public string MemberName
    {
      get
      {
        return _memberName;
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
      //Store value and name property in a not inicialized property
      var property = new Property(propertyName);
      property.Value = value;

      return property;
    }

    /// <summary>
    /// Get a property from a steel object
    /// </summary>
    /// <param name="steelObject"> Steel object</param>
    /// <param name="propertyName"> Name of the property</param>
    /// <returns name="property">The desired property</returns>
    public static Property GetObjectProperty(SteelDbObject steelObject, string propertyName)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        FilerObject filerObj = Utils.GetObject(steelObject.Handle);

        Property extractionProperty = Utils.GetProperty(filerObj, propertyName);
        extractionProperty.EvaluateFromObject(filerObj);

        return extractionProperty;
      }
    }

    /// <summary>
    /// Set a property for a Steel Object
    /// </summary>
    /// <param name="steelObject"> Steel Object</param>
    /// <param name="property"> Property object</param>
    /// <returns name="steelObject"> The updated steel object</returns>
    public static SteelDbObject SetObjectProperty(SteelDbObject steelObject, Property property)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        FilerObject fo = Utils.GetObject(steelObject.Handle);
       
        property.SetToObject(fo);
        return steelObject;
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
        Dictionary<string, Property> allProperties = Utils.GetAllProperties(filerObj.GetType());

        foreach (KeyValuePair<string, Property> prop in allProperties)
        {
          Property extractionProperty = prop.Value;
          extractionProperty.EvaluateFromObject(filerObj);

          ret.Add(extractionProperty);
        }
      }

      return ret;
    }

    public override string ToString()
    {
      return _description?.ToString() + " = " + Value?.ToString();
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
        HasValidValue(value);
        _value = value;
      }
    }

    internal void SetToObject(object objectToUpdate)
    {
      if (objectToUpdate == null)
      {
        throw new System.Exception("No Advance Steel Object Found");
      }

      Property property = this;

      if (!this._inicialized)
      {
        if (string.IsNullOrEmpty(this.MemberName))
        {
          property = Utils.GetProperty(objectToUpdate, this._description);
        }

        else
        {
          property = Utils.GetPropertyByMemberName(objectToUpdate, this.MemberName);
        }
      }

      property.SetASObjectProperty(objectToUpdate, InternalValue);
    }

    private void SetASObjectProperty(object asObject, object value)
    {
      if (!(_memberInfo is PropertyInfo))
      {
        throw new System.Exception(string.Format("Set property not found: {0}", _description?.ToString()));
      }

      if (!_write)
      {
        throw new System.Exception(string.Format("Cannot set readonly property: {0}", _description?.ToString()));
      }

      HasValidValue(value);

      PropertyInfo propertyInfo = _memberInfo as PropertyInfo;
      propertyInfo.SetValue(asObject, value);
    }

    internal void EvaluateFromObject(object objectToUpdateFrom)
    {
      if (objectToUpdateFrom == null)
      {
        throw new System.Exception("No Advance Steel Object Found");
      }

      try
      {
        InternalValue = GetASObjectProperty(objectToUpdateFrom);
      }
      catch (Exception)
      {
        throw new System.Exception(string.Format("Object has no Property - {0}", _description?.ToString()));
      }

    }

    private void HasValidValue(object value)
    {
      if (value == null || _valueType == null)
        return;

      if (_valueType == typeof(int) && IsInteger(value))
        return;

      if (_valueType == typeof(double) && IsDouble(value))
        return;

      if (_valueType.Equals(value.GetType()))
        return;

      throw new System.Exception(string.Format("This value type '{0}' is not valid for '{1}'", value.GetType().ToString(), _description));
    }

    private object GetASObjectProperty(object asObject)
    {
      if (_memberInfo is PropertyInfo)
      {
        PropertyInfo propertyInfo = _memberInfo as PropertyInfo;

        return propertyInfo.GetValue(asObject);
      }
      else if (_memberInfo is MethodInfo)
      {
        MethodInfo methodInfo = _memberInfo as MethodInfo;

        return methodInfo.Invoke(asObject, null);
      }
      else if (_funcGetValue != null)
      {
        return _funcGetValue(asObject);
      }
      else
      {
        throw new NotImplementedException();
      }
    }

    private object ConvertToDynamoObject(object objectAS)
    {
      dynamic objectASDynamic = objectAS;
      return ConvertToDyn(objectASDynamic);
    }

    private object ConvertToDyn(Point3d objectAS)
    {
      return objectAS.ToDynPoint();
    }

    private object ConvertToDyn(Vector3d objectAS)
    {
      return objectAS.ToDynVector();
    }

    private object ConvertToDyn(Vector2d objectAS)
    {
      return objectAS.ToDynVector();
    }

    private object ConvertToDyn(Plane objectAS)
    {
      return objectAS.ToDynPlane();
    }

    private object ConvertToDyn(Polyline3d objectAS)
    {
      return objectAS.ToDynPolyCurve();
    }

    private object ConvertToDyn(Matrix3d objectAS)
    {
      return objectAS.ToDynCoordinateSys();
    }

    private object ConvertToDyn(object objectAS)
    {
      if ((IsDouble(objectAS) || IsInteger(objectAS)) && UnitType.HasValue)
      {
        return System.Convert.ToDouble(objectAS).FromInternalUnits(this.UnitType.Value);
      }

      return objectAS;
    }

    private object ConvertFromDynamoObject(object objectDyn)
    {
      dynamic objectDynDynamic = objectDyn;
      return ConvertFromDyn(objectDynDynamic);
    }

    private object ConvertFromDyn(Autodesk.DesignScript.Geometry.Point objectDyn)
    {
      return objectDyn.ToAstPoint();
    }

    private object ConvertFromDyn(Autodesk.DesignScript.Geometry.Vector objectDyn)
    {
      return objectDyn.ToAstVector3d();
    }

    private object ConvertFromDyn(Autodesk.DesignScript.Geometry.CoordinateSystem objectDyn)
    {
      return objectDyn.ToAstMatrix3d();
    }

    private object ConvertFromDyn(object objectDyn)
    {
      if ((IsDouble(objectDyn) || IsInteger(objectDyn)) && UnitType.HasValue)
      {
        return System.Convert.ToDouble(objectDyn).ToInternalUnits(this.UnitType.Value);
      }

      return objectDyn;
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
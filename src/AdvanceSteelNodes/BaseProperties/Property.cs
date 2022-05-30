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
    private PropertyMethods _propertyMethods;

    private object _value;
    private bool _read;
    private bool _write;

    private bool _initialized;

    internal LevelEnum Level { get; private set; }
    private eUnitType? UnitType { get; set; }

    internal Property(Property existingProperty)
    {
      CopyProperty(existingProperty);
    }

    internal void CopyProperty(Property existingProperty)
    {
      _read = existingProperty._read;
      _write = existingProperty._write;
      _objectType = existingProperty._objectType;
      _description = existingProperty._description;
      _memberInfo = existingProperty._memberInfo;
      _memberName = existingProperty._memberName;
      _valueType = existingProperty._valueType;
      _propertyMethods = existingProperty._propertyMethods;

      _initialized = existingProperty._initialized;

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
      
      _initialized = true;
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

      throw new Exception(string.Format("'{0}' is not a property nor return method with 0 arguments nor void method with 1 argument", memberName));
    }

    internal Property(Type objectType, string description, PropertyMethods propertyMethods, LevelEnum level = LevelEnum.NoDefinition, eUnitType? unitType = null)
    {
      _initialized = true;

      _objectType = objectType;
      _description = description;
      UnitType = unitType;

      _propertyMethods = propertyMethods;
      _read = _propertyMethods.MethodInfoGet != null;
      _write = _propertyMethods.MethodInfoSet != null;

      if(_propertyMethods.MethodInfoGet != null)
      {
        _valueType = _propertyMethods.MethodInfoGet.ReturnType;
      }

      if (_propertyMethods.MethodInfoSet != null)
      {
        _valueType = _propertyMethods.MethodInfoSet.GetParameters()[1].ParameterType;
      }

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
    /// Check if this property is readonly
    /// </summary>
    /// <returns name="IsReadOnly">The read status of the property</returns>
    public bool IsReadOnly
    {
      get
      {
        return _read && !_write;
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

        Property extractionProperty = UtilsProperties.GetProperty(filerObj.GetType(), propertyName);
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
        Dictionary<string, Property> allProperties = UtilsProperties.GetAllProperties(filerObj.GetType());

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

      this.InitializePropertyIfNeeded(objectToUpdate.GetType());

      if (!objectToUpdate.GetType().IsSubclassOf(this._objectType) && !objectToUpdate.GetType().IsEquivalentTo(this._objectType))
      {
        throw new System.Exception(string.Format("Not '{0}' Object", _objectType.Name));
      }

      this.SetASObjectProperty(objectToUpdate);
    }

    internal void InitializePropertyIfNeeded(Type objectType)
    {
      if (this._initialized)
      {
        return;
      }

      Property newProperty;

      if (string.IsNullOrEmpty(this.MemberName))
      {
        newProperty = UtilsProperties.GetProperty(objectType, this._description);
      }
      else
      {
        newProperty = UtilsProperties.GetPropertyByMemberName(objectType, this.MemberName);
      }

      this.CopyProperty(newProperty);
    }

    private void SetASObjectProperty(object asObject)
    {
      if (!_write)
      {
        throw new System.Exception(string.Format("Cannot set readonly property: {0}", _description?.ToString()));
      }

      object value = this.InternalValue;

      HasValidValue(value);

      if (_memberInfo is PropertyInfo)
      {
        PropertyInfo propertyInfo = _memberInfo as PropertyInfo;
        propertyInfo.SetValue(asObject, value);
      }
      else if (_memberInfo is MethodInfo)
      {
        MethodInfo methodInfo = _memberInfo as MethodInfo;
        methodInfo.Invoke(asObject, new object[] { value });
      }
      else if (_propertyMethods?.MethodInfoSet != null)
      {
        _propertyMethods.MethodInfoSet.Invoke(null, new object[] { asObject, value});
      }
      else
      {
        throw new System.Exception(string.Format("Set property not found: {0}", _description?.ToString()));
      }

    }

    internal void EvaluateFromObject(object objectToUpdateFrom)
    {
      if (objectToUpdateFrom == null)
      {
        throw new System.Exception("No Advance Steel Object Found");
      }

      if(!objectToUpdateFrom.GetType().IsSubclassOf(_objectType) && !objectToUpdateFrom.GetType().IsEquivalentTo(_objectType))
      {
        throw new System.Exception(string.Format("Not '{0}' Object", _objectType.Name));
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
      else if (_propertyMethods?.MethodInfoGet != null)
      {
        return _propertyMethods.MethodInfoGet.Invoke(null, new object[] { asObject });
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

    private object ConvertFromDyn(Autodesk.DesignScript.Geometry.PolyCurve objectDyn)
    {
      return objectDyn.ToAstPolyline3dOpened();
    }

    private object ConvertFromDyn(Autodesk.DesignScript.Geometry.Plane objectDyn)
    {
      return objectDyn.ToAstPlane();
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
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

    internal Property(string memberName, object internalValue)
    {
      _memberName = memberName;
      InternalValue = internalValue;
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
    }

    /// <summary>
    /// Get the value from the property
    /// </summary>
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
    /// <returns name="Name">The name of the property</returns>
    public string Name
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
      return new Property(propertyName, value);
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
        Dictionary<string, Property> allProperties = Utils.GetAllProperties(filerObj);

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
        property = Utils.GetProperty(objectToUpdate, this.Name);
      }

      if (!property._write)
      {
        throw new System.Exception(string.Format("Cannot set readonly property: {0}", Name?.ToString()));
      }

      SetASObjectProperty(objectToUpdate, InternalValue);
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
        throw new System.Exception(string.Format("Object has no Property - {0}", Name?.ToString()));
      }

    }

    private void HasValidValue(object value)
    {
      if (_valueType == typeof(int) && IsInteger(value))
        return;
      else if (_valueType == typeof(double) && IsDouble(value))
        return;
      else if (_valueType.Equals(value.GetType()))
        return;
      else
        throw new System.Exception(string.Format("This value type '{0}' is not valid for '{1}'", value.GetType().ToString(), _description));
    }

    public void SetASObjectProperty(object asObject, object value)
    {
      if (!(_memberInfo is PropertyInfo))
      {
        throw new System.Exception(string.Format("Set property not found: {0}", Name?.ToString()));
      }

      HasValidValue(value);

      PropertyInfo propertyInfo = _memberInfo as PropertyInfo;
      propertyInfo.SetValue(asObject, value);
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
      return Convert(objectASDynamic);
    }

    private object Convert(Point3d objectAS)
    {
      return objectAS.ToDynPoint();
    }

    private object Convert(Vector3d objectAS)
    {
      return objectAS.ToDSVector();
    }

    private object Convert(Vector2d objectAS)
    {
      return objectAS.ToDSVector();
    }

    private object Convert(Plane objectAS)
    {
      return objectAS.ToDSPlane();
    }

    private object Convert(Polyline3d objectAS)
    {
      return objectAS.ToDSPolyCurve();
    }

    private object Convert(Matrix3d objectAS)
    {
      objectAS.GetCoordSystem(out ASPoint3d point, out ASVector3d vetorX, out ASVector3d vetorY, out ASVector3d vetorZ);

      //Try the vectors
      if (vetorX.IsZeroLength() || vetorY.IsZeroLength() || vetorZ.IsZeroLength())
      {
        throw new Exception(ResourceStrings.Nodes_ErrorConvertingCS);
      }

      return DSCoordinateSystem.ByOriginVectors(point.ToDSPoint(), vetorX.ToDSVector(), vetorY.ToDSVector(), vetorZ.ToDSVector());
    }

    private object Convert(double objectAS)
    {
      if (UnitType.HasValue)
      {
        ((double)objectAS).FromInternalUnits(UnitType.Value);
      }

      return objectAS;
    }

    private object Convert(object objectAS)
    {
      return objectAS;
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
        return Utils.ToInternalUnits(System.Convert.ToDouble(val), this.UnitType.Value, true);
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
        return Utils.FromInternalUnits(System.Convert.ToDouble(val), this.UnitType.Value, true);
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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AdvanceSteel.CADAccess;
using static Autodesk.AdvanceSteel.DotNetRoots.Units.Unit;

namespace AdvanceSteel.Nodes
{
  public abstract class BaseProperties<T> : IASProperties where T:class
  {
    public Type GetObjectType { get => typeof(T); }

    public abstract Dictionary<string, Property> BuildPropertyList();

    protected void InsertItem(Dictionary<string, Property> dictionary, string description, string memberName, LevelEnum level = LevelEnum.NoDefinition, eUnitType? unitType = null)
    {
      dictionary.Add(description, new Property(GetObjectType, description, memberName, level, unitType));
    }

    protected void InsertItem(Dictionary<string, Property> dictionary, string description, string memberName, eUnitType unitType)
    {
      InsertItem(dictionary, description, memberName, LevelEnum.NoDefinition, unitType);
    }

    protected void InsertItem(Dictionary<string, Property> dictionary, string description, Func<object, object> funcGetValue, LevelEnum level = LevelEnum.NoDefinition, eUnitType? unitType = null)
    {
      dictionary.Add(description, new Property(GetObjectType, description, funcGetValue, level, unitType));
    }

    protected void InsertItem(Dictionary<string, Property> dictionary, string description, Func<object, object> funcGetValue, eUnitType? unitType)
    {
      InsertItem(dictionary, description, funcGetValue, LevelEnum.NoDefinition, unitType);
    }

    protected T GetObjectAS(object objAS)
    {
      if (!(objAS is T))
      {
        throw new System.Exception(string.Format("Not '{0}' Object", typeof(T).Name));
      }

      return objAS as T;
    }

  }
}

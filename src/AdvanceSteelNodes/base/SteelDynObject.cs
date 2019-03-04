using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using System;
using Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes
{
  /// <summary>
  /// This is the equivalent of an Advance Steel object in Dynamo
  /// </summary>
  [IsVisibleInDynamoLibrary(false)]
  public abstract class SteelDynObject : IDisposable, IFormattable
  {
    [IsVisibleInDynamoLibrary(false)]
    public abstract void Dispose();


    [IsVisibleInDynamoLibrary(false)]
    public override string ToString()
    {
      return GetType().Name;
    }

    public virtual string ToString(string format, IFormatProvider formatProvider)
    {
      return ToString();
    }
  }
}
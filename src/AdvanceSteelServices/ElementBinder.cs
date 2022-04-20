using Autodesk.AdvanceSteel.CADAccess;
using System;
using System.Runtime.Serialization;

namespace Dynamo.Applications.AdvanceSteel.Services
{
  /// <summary>
  /// Tools to handle the binding and interaction
  /// </summary>
  public class ElementBinder
  {
    //maybe we need a separate key for Advance Steel
    //for the moment dynamo is using only this hardcoded key (see ..\Dynamo\src\Engine\ProtoCore\Lang\TraceUtils.cs)
    private const string REVIT_TRACE_ID = "{0459D869-0C72-447F-96D8-08A7FB92214B}-REVIT";

    //by default we let it true
    //we must to disable while we execute code from python
    public static bool IsEnabled = true;

    public static string GetHandleFromTrace()
    {
      ISerializable traceData = DynamoServices.TraceUtils.GetTraceData(REVIT_TRACE_ID);

      SerializableHandle tracedHandle = traceData as SerializableHandle;
      if (tracedHandle == null)
        return null; //There was no usable data in the trace cache

      return tracedHandle.Handle;
    }

    public static void SetElementForTrace(string handle)
    {
      SerializableHandle tracedHandle = new SerializableHandle();
      tracedHandle.Handle = handle;

      DynamoServices.TraceUtils.SetTraceData(REVIT_TRACE_ID, tracedHandle);
    }

    public static void CleanupAndSetElementForTrace(FilerObject newElement)
    {
      if (newElement == null) return;

      var oldHandle = GetHandleFromTrace();
      if (oldHandle != null && oldHandle != newElement.Handle)
      {
        //right now do not delete anything
        //
      }

      SetElementForTrace(newElement.Handle);
    }

    [CLSCompliant(false)]
    public static T GetObjectASFromTrace<T>() where T: FilerObject
    {
      string handle = GetHandleFromTrace();

      if (string.IsNullOrEmpty(handle))
        return null;

      T filerObject = FilerObject.GetFilerObjectByHandle(handle) as T;

      return filerObject;
    }

    public static void SetRawDataForTrace(ISerializable data)
    {
      if (!IsEnabled) return;

      DynamoServices.TraceUtils.SetTraceData(REVIT_TRACE_ID, data);
    }
  }
}
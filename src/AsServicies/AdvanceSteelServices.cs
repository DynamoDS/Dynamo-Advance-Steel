using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dynamo.Models;
using Dynamo.Nodes;



using System.Runtime.Serialization;
using Autodesk.AdvanceSteel.CADAccess;

namespace AdvanceSteelServices
{
    /// <summary>
    /// Holds a representation of a AutoCAD Handle that supports serialisation
    /// </summary>
    [Serializable]
    public class SerializableHandle : ISerializable
    {
        public String stringID { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("stringID", stringID, typeof(string));
        }

        public SerializableHandle()
        {
            stringID = "";
        }

        /// <summary>
        /// Ctor used by the serialisation engine
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public SerializableHandle(SerializationInfo info, StreamingContext context)
        {
            stringID = (string)info.GetValue("stringID", typeof(string));
        }
    }

    /// <summary>
    /// Tools to handle the binding and interaction 
    /// </summary>
    public class ElementBinder
    {
        //maybe we need a separate key for Advance Steel
        //for the moment dynamo is using only this hardcode key (see ..\Dynamo\src\Engine\ProtoCore\Lang\TraceUtils.cs)
        private const string REVIT_TRACE_ID = "{0459D869-0C72-447F-96D8-08A7FB92214B}-REVIT";

        //by default we let it true
        //we must to disable while we execute code from python(TO_DO)
        public static bool IsEnabled = true;

        public static string GetHandleFromTrace()
        {
            ISerializable traceData = DynamoServices.TraceUtils.GetTraceData(REVIT_TRACE_ID);

            SerializableHandle handle = traceData as SerializableHandle;
            if (handle == null)
                return null; //There was no usable data in the trace cache

            return handle.stringID;
        }
        public static void SetElementForTrace(String handle)
        {
            if (!IsEnabled) return;

            SerializableHandle hand = new SerializableHandle();
            hand.stringID = handle;

            DynamoServices.TraceUtils.SetTraceData(REVIT_TRACE_ID, hand);
        }

        public static void CleanupAndSetElementForTrace(FilerObject newElement)
        {
            if (!IsEnabled) return;
            if (newElement == null) return;

            var oldHandle = GetHandleFromTrace();
            if (oldHandle != null && oldHandle!= newElement.Handle)
            {
                //right now do not delete anything
                //
                //TO_DO
            }

            SetElementForTrace(newElement.Handle);
        }
    }
}
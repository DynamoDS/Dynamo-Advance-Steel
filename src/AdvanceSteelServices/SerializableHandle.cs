using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dynamo.Applications.AdvanceSteel.Services
{
  /// <summary>
  /// Holds a representation of a AutoCAD Handle that supports serialization
  /// </summary>
  [Serializable]
  public class SerializableHandle : ISerializable
  {
    public string Handle { get; set; }

    public SerializableHandle()
    {
      Handle = "";
    }

    /// <summary>
    /// Ctor used by the serialization engine
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    public SerializableHandle(SerializationInfo info, StreamingContext context)
    {
      Handle = (string)info.GetValue("Handle", typeof(string));
    }
    /// <summary>
    /// Populates a System.Runtime.Serialization.SerializationInfo with the data needed
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("Handle", Handle, typeof(string));
    }
  }

}

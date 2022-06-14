using Autodesk.DesignScript.Runtime;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvanceSteel.Nodes
{
  public abstract class AstDropDownBase : DSDropDownBase
  {
    protected AstDropDownBase(string value) : base(value)
    {
    }

    [JsonConstructor]
    public AstDropDownBase(string value, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(value, inPorts, outPorts)
    {
    }
  }
}
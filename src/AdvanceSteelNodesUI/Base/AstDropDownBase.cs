using Autodesk.DesignScript.Runtime;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvanceSteel.Nodes
{
  public abstract class AstDropDownBase : DSDropDownBase
  {

    protected const string SelectObjectTypeString = "Select Object Type...";
    protected AstDropDownBase(string outputName) : base(outputName)
    {
    }

    [JsonConstructor]
    public AstDropDownBase(string outputName, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts)
    {
    }
  }
}
using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;

namespace AdvanceSteel.Nodes
{
  [NodeName("BodyResolutions")]
  [NodeDescription("Lists all options for body resolution")]
  [NodeCategory("AdvanceSteel.Nodes.Util.Geometry")]
  [OutPortNames("bodyResolution")]
  [OutPortTypes("int")]
  [OutPortDescriptions("body resolution")]
  [IsDesignScriptCompatible]
  public class BodyResolution : ASListBase
  {
    protected override string GetListName => "Body Resolution";

    public BodyResolution() : base() { }

    [JsonConstructor]
    public BodyResolution(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }

    protected override List<DynamoDropDownItem> GetListDropDown()
    {
      var list = new List<DynamoDropDownItem>()
      {
        new DynamoDropDownItem("Normal", 0L),
        new DynamoDropDownItem("Detailed", 1L),
        new DynamoDropDownItem("Hull", 2L),
        new DynamoDropDownItem("UnNotched", 3L)
      };
      return list;
    }
  }
}

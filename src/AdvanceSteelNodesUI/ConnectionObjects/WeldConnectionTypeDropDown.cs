using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;


namespace AdvanceSteel.Nodes
{
  [NodeName("WeldConnectionTypes")]
  [NodeDescription("Lists the Advance Steel Weld connection type options")]
  [NodeCategory("AdvanceSteel.Nodes.ConnectionObjects")]
  [OutPortNames("weldType")]
  [OutPortTypes("int")]
  [OutPortDescriptions("weld connection type")]
  [IsDesignScriptCompatible]
  public class WeldConnectionType : ASListBase
  {
    protected override string GetListName => "Weld Type";

    public WeldConnectionType() : base() { }

    [JsonConstructor]
    public WeldConnectionType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }

    protected override List<DynamoDropDownItem> GetListDropDown()
    {
      var list = new List<DynamoDropDownItem>()
      {
        new DynamoDropDownItem("OnSite", 0L),
        new DynamoDropDownItem("InShop", 2L)
      };
      return list;
    }
  }
}

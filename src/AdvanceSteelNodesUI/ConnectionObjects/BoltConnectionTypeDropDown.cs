using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;

namespace AdvanceSteel.Nodes
{
  [NodeName("BoltConnectionTypes")]
  [NodeDescription("Lists the Advance Steel Bolt Connection type options")]
  [NodeCategory("AdvanceSteel.Nodes.ConnectionObjects")]
  [OutPortNames("boltType")]
  [OutPortTypes("int")]
  [OutPortDescriptions("bolt connection type")]
  [IsDesignScriptCompatible]
  public class BoltConnectionType : ASListBase
  {
    protected override string GetListName => "Bolt Type";

    public BoltConnectionType() : base() { }

    [JsonConstructor]
    public BoltConnectionType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }

    protected override List<DynamoDropDownItem> GetListDropDown()
    {
      var list = new List<DynamoDropDownItem>()
      {
        new DynamoDropDownItem("OnSite", 0L),
        new DynamoDropDownItem("Site Drill", 1L),
        new DynamoDropDownItem("InShop", 2L)
      };

      return list;
    }
  }
}

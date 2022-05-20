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
  public class BoltConnectionType : AstDropDownBase
  {
    private const string outputName = "boltType";

    public BoltConnectionType()
        : base(outputName)
    {
      InPorts.Clear();
      OutPorts.Clear();
      RegisterAllPorts();
    }

    [JsonConstructor]
    public BoltConnectionType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(outputName, inPorts, outPorts)
    {
    }

    protected override SelectionState PopulateItemsCore(string currentSelection)
    {
      Items.Clear();

      var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("Select Bolt Type...", -1L),
                new DynamoDropDownItem("OnSite", 0L),
                new DynamoDropDownItem("Site Drill", 1L),
                new DynamoDropDownItem("InShop", 2L)
            };

      Items.AddRange(newItems);

      SelectedIndex = 0;
      return SelectionState.Restore;
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (Items.Count == 0 ||
          SelectedIndex < 0 ||
          Items[SelectedIndex].Name == "Select Bolt Type...")
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
      }

      var intNode = AstFactory.BuildIntNode((long)Items[SelectedIndex].Item);
      var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);
      return new List<AssociativeNode> { assign };

    }
  }
}

using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;


namespace AdvanceSteel.Nodes
{
  [NodeName("HoleTypes")]
  [NodeDescription("Lists the Advance Steel Hole type options")]
  [NodeCategory("AdvanceSteel.Nodes.ConnectionObjects")]
  [OutPortNames("holeType")]
  [OutPortTypes("int")]
  [OutPortDescriptions("hole type")]
  [IsDesignScriptCompatible]
  public class HoleType : AstDropDownBase
  {
    private const string outputName = "holeType";

    public HoleType()
        : base(outputName)
    {
      InPorts.Clear();
      OutPorts.Clear();
      RegisterAllPorts();
    }

    [JsonConstructor]
    public HoleType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(outputName, inPorts, outPorts)
    {
    }

    protected override SelectionState PopulateItemsCore(string currentSelection)
    {
      Items.Clear();

      var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("Select Hole Type...", -1L),
                new DynamoDropDownItem("Hole", 1L),
                new DynamoDropDownItem("Slotted Hole", 2L),
                new DynamoDropDownItem("Counter Sunk Hole", 3L),
                new DynamoDropDownItem("Blind Hole", 4L),
                new DynamoDropDownItem("Threaded Hole", 5L),
                new DynamoDropDownItem("Sunk Hole", 6L),
                new DynamoDropDownItem("Punch Mark", 7L)
            };

      Items.AddRange(newItems);

      SelectedIndex = 0;
      return SelectionState.Restore;
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (Items.Count == 0 ||
          SelectedIndex < 0 ||
          Items[SelectedIndex].Name == "Select Hole Type...")
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
      }

      var intNode = AstFactory.BuildIntNode((long)Items[SelectedIndex].Item);
      var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);
      return new List<AssociativeNode> { assign };

    }
  }
}

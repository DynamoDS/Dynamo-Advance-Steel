using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;


namespace AdvanceSteel.Nodes
{
  [NodeName("HoleDirection")]
  [NodeDescription("Lists the Advance Steel Slotted Hole Direction options")]
  [NodeCategory("AdvanceSteel.Nodes.ConnectionObjects")]
  [OutPortNames("slotDirection")]
  [OutPortTypes("int")]
  [OutPortDescriptions("slotted hole direciton")]
  [IsDesignScriptCompatible]
  public class SlotDirection : AstDropDownBase
  {
    private const string outputName = "slotDirection";

    public SlotDirection()
        : base(outputName)
    {
      InPorts.Clear();
      OutPorts.Clear();
      RegisterAllPorts();
    }

    [JsonConstructor]
    public SlotDirection(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(outputName, inPorts, outPorts)
    {
    }

    protected override SelectionState PopulateItemsCore(string currentSelection)
    {
      Items.Clear();

      var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("Select Slotted Hole Direction...", -1L),
                new DynamoDropDownItem("Arc Along", 2L),
                new DynamoDropDownItem("X-Axis", 1L),
                new DynamoDropDownItem("Y-Axis", 2L)
            };
      
      Items.AddRange(newItems);

      SelectedIndex = 0;
      return SelectionState.Restore;
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (Items.Count == 0 ||
          SelectedIndex < 0 ||
          Items[SelectedIndex].Name == "Select Slotted Hole Direction...")
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
      }

      var intNode = AstFactory.BuildIntNode((long)Items[SelectedIndex].Item);
      var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);
      return new List<AssociativeNode> { assign };

    }
  }
}

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
                new DynamoDropDownItem("Select Hole Type...", -1),
                new DynamoDropDownItem("Hole", 1),
                new DynamoDropDownItem("Slotted Hole", 2),
                new DynamoDropDownItem("Counter Sunk Hole", 3),
                new DynamoDropDownItem("Blind Hole", 4),
                new DynamoDropDownItem("Threaded Hole", 5),
                new DynamoDropDownItem("Sunk Hole", 6),
                new DynamoDropDownItem("Punch Mark", 7)
            };

      Items.AddRange(newItems);

      SelectedIndex = 0;
      return SelectionState.Restore;
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (Items.Count == 0 ||
          Items[SelectedIndex].Name == "Select Hole Type..." ||
          SelectedIndex < 0)
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
      }

      var intNode = AstFactory.BuildIntNode((int)Items[SelectedIndex].Item);
      var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);
      return new List<AssociativeNode> { assign };

    }
  }
}

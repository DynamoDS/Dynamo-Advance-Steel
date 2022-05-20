using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;

namespace AdvanceSteel.Nodes
{
  [NodeName("AnchorOrientationTypes")]
  [NodeDescription("Lists the Advance Steel Anchor orientation type options")]
  [NodeCategory("AdvanceSteel.Nodes.ConnectionObjects")]
  [OutPortNames("orientationType")]
  [OutPortTypes("int")]
  [OutPortDescriptions("anchor orientation type")]
  [IsDesignScriptCompatible]
  public class AnchorOrientationType : AstDropDownBase
  {
    private const string outputName = "orientationType";

    public AnchorOrientationType()
        : base(outputName)
    {
      InPorts.Clear();
      OutPorts.Clear();
      RegisterAllPorts();
    }

    [JsonConstructor]
    public AnchorOrientationType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(outputName, inPorts, outPorts)
    {
    }

    protected override SelectionState PopulateItemsCore(string currentSelection)
    {
      Items.Clear();

      var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("Select Anchor Orientation...", -1L),
                new DynamoDropDownItem("Normal Orientation", 0L),
                new DynamoDropDownItem("Diagonal Inside", 1L),
                new DynamoDropDownItem("Diagonal Outside", 2L),
                new DynamoDropDownItem("All Outside", 3L),
                new DynamoDropDownItem("All Inside", 4L),
                new DynamoDropDownItem("Inside Rotated", 5L),
                new DynamoDropDownItem("Outside Rotated", 6L)
            };

      Items.AddRange(newItems);

      SelectedIndex = 0;
      return SelectionState.Restore;
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (Items.Count == 0 ||
          SelectedIndex < 0 ||
          Items[SelectedIndex].Name == "Select Anchor Orientation...")
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
      }

      var intNode = AstFactory.BuildIntNode((long)Items[SelectedIndex].Item);
      var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);
      return new List<AssociativeNode> { assign };

    }
  }
}
